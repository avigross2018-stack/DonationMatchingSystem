using BackendService.Data;
using BackendService.DTOs;
using BackendService.Exceptions;
using BackendService.Models;
using BackendService.Repos;
using Microsoft.AspNetCore.Identity;

namespace BackendService.Services
{
    public class AuthService
    {
        private readonly IUserRepo _userRepo;
        private readonly IDonorRepo _donorRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly JwtTokenService _jwtTokenService;
        private readonly IAdminRepo _adminRepo;
        private readonly IRecipientRepo _recipientRepo;
        private readonly IPopulationTypeRepo _populationTypeRepo;
        private readonly EligibilityService _eligibilityService;
        private readonly MatchingEventQueue _matchingQueue;
        private readonly ILogger<AuthService> _logger;
        public AuthService(
            IUserRepo userRepo,
            IDonorRepo donorRepo,
            IAdminRepo adminRepo,
            IUnitOfWork unitOfWork,
            IPasswordHasher<User> passwordHasher,
            JwtTokenService jwtTokenService,
            IRecipientRepo recipientRepo,
            IPopulationTypeRepo populationTypeRepo,
            EligibilityService eligibilityService,
            MatchingEventQueue matchingQueue,
            ILogger<AuthService> logger
        )
        {
            _userRepo = userRepo;
            _donorRepo = donorRepo;
            _adminRepo = adminRepo;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
            _recipientRepo = recipientRepo;
            _populationTypeRepo = populationTypeRepo;
            _eligibilityService = eligibilityService;
            _matchingQueue = matchingQueue;
            _logger = logger;
        }

        public async Task<int> RegisterDonorAsync(RegisterDonorDto dto)
        {
            var existingUser = await _userRepo.GetByPhoneAsync(dto.Phone);

            if (existingUser != null)
            {
                throw new ConflictException(
                    "A user with this phone number already exists."
                );
            }

            var user = new User
            {
                Name = dto.Name,
                Phone = dto.Phone,
                UserType = "DONOR",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash = _passwordHasher.HashPassword(
                user,
                dto.Password
            );

            var donor = new Donor
            {
                User = user
            };

            await _userRepo.AddAsync(user);
            await _donorRepo.AddAsync(donor);

            await _unitOfWork.SaveChangesAsync();

            return user.Id;
        }
        
        // Creates the initial admin account if it does not exist yet.
        // Returns true if a new admin was created, false if the phone already exists.
        public async Task<bool> EnsureAdminExistsAsync(
            string name,
            string phone,
            string password)
        {
            // Fail fast on missing config values
            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException(
                    "Admin name, phone and password are required.");
            }

            // Idempotent: if the phone is already registered, do nothing
            var existingUser = await _userRepo.GetByPhoneAsync(phone);

            if (existingUser != null)
            {
                return false;
            }

            var user = new User
            {
                Name = name,
                Phone = phone,
                UserType = "ADMIN",   // becomes the role claim in the JWT
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            // Never store the plain password, only the hash
            user.PasswordHash = _passwordHasher.HashPassword(user, password);

            // Linking via the navigation property lets EF set UserId automatically
            var admin = new Admin
            {
                User = user
            };

            await _userRepo.AddAsync(user);
            await _adminRepo.AddAsync(admin);

            // Single SaveChanges: User and Admin are saved together or not at all
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<string?> LoginAsync(LoginDto dto)
        {
            var user = await _userRepo.GetByPhoneAsync(dto.Phone);

            if (user == null || !user.IsActive)
            {
                return null;
            }

            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                dto.Password
            );

            if(result == PasswordVerificationResult.Failed)
            {
                return null;
            }

            return _jwtTokenService.GenerateToken(user);
        }

        public async Task<int> RegisterRecipientAsync(RegisterRecipientDto dto)
        {
            // Check whether phone number already exists
            var existingUser = await _userRepo.GetByPhoneAsync(dto.Phone);

            if (existingUser != null)
            {
                throw new ConflictException(
                    "A user with this phone number already exists.");
            }

            // Validate population type
            var populationType = await _populationTypeRepo
                .GetByIdAsync(dto.PopulationTypeId);

            if (populationType == null || !populationType.IsActive)
            {
                throw new BusinessValidationException(
                    "Population type is invalid or inactive.");
            }

            // Validate eligibility inputs
            if (dto.IncomeAmount < 0 || dto.AmountOfChildren < 0 ||
                decimal.Round(dto.IncomeAmount, 2) != dto.IncomeAmount)
            {
                throw new BusinessValidationException(
                    "Income must be non-negative with at most two decimal places, and children count cannot be negative.");
            }

            // Calculate eligibility
            decimal eligibility = _eligibilityService.CalculateEligibility(
                populationType.BaseEligibilityAmount,
                dto.IncomeAmount,
                dto.AmountOfChildren);

            // Create user
            var user = new User
            {
                Name = dto.Name,
                Phone = dto.Phone,
                UserType = "RECIPIENT",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash = _passwordHasher.HashPassword(
                user,
                dto.Password);

            // Create recipient profile
            var recipient = new Recipient
            {
                User = user,
                PopulationTypeId = dto.PopulationTypeId,
                IncomeAmount = dto.IncomeAmount,
                AmountOfChildren = dto.AmountOfChildren,
                RequestReason = dto.RequestReason,
                EligibilityAmount = eligibility,
                RemainingEligibility = eligibility
            };

            // Save user and recipient together
            await _userRepo.AddAsync(user);
            await _recipientRepo.AddAsync(recipient);

            await _unitOfWork.SaveChangesAsync();

            // Send matching event after successful save
            try
            {
                await _matchingQueue.EnqueueAsync(
                    new MatchingEvent(
                        MatchingEventType.RecipientRegistered,
                        recipient.Id));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to enqueue matching event for recipient {RecipientId}",
                    recipient.Id);
            }

            return user.Id;

        }

        public async Task ChangePasswordAsync(
            int userId,
            ChangePasswordDto dto)
        {
            var user = await _userRepo.GetByIdAsync(userId);

            if (user == null || !user.IsActive)
            {
                throw new ForbiddenException(
                    "User account is inactive or unavailable.");
            }

            var verificationResult =
                _passwordHasher.VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    dto.CurrentPassword);

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                throw new BusinessValidationException(
                    "Current password is incorrect.");
            }

            if (dto.CurrentPassword == dto.NewPassword)
            {
                throw new BusinessValidationException(
                    "New password must be different from current password.");
            }

            user.PasswordHash = _passwordHasher.HashPassword(
                user,
                dto.NewPassword);

            await _unitOfWork.SaveChangesAsync();
        }

    }
}