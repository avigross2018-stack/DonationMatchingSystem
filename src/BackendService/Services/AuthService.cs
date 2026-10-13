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
        public AuthService(
            IUserRepo userRepo,
            IDonorRepo donorRepo,
            IAdminRepo adminRepo,
            IUnitOfWork unitOfWork,
            IPasswordHasher<User> passwordHasher,
            JwtTokenService jwtTokenService
        )
        {
            _userRepo = userRepo;
            _donorRepo = donorRepo;
            _adminRepo = adminRepo;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
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
    }
}