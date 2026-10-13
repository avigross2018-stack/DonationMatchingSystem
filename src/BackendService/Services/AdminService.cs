using System.Data;
using BackendService.Data;
using BackendService.DTOs;
using BackendService.Exceptions;
using BackendService.Models;
using BackendService.Repos;
using Microsoft.EntityFrameworkCore;

namespace BackendService.Services
{
    public sealed class AdminService
    {
        private readonly IAdminRepo _adminRepo;
        private readonly IDonorRepo _donorRepo;
        private readonly IDonationRepo _donationRepo;
        private readonly IRedemptionRepo _redemptionRepo;
        private readonly IRecipientRepo _recipientRepo;
        private readonly IUserRepo _userRepo;
        private readonly IPopulationTypeRepo _populationTypeRepo;
        private readonly EligibilityService _eligibilityService;
        private readonly MatchingEventQueue _matchingQueue;
        private readonly ILogger<AdminService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public AdminService(
            IAdminRepo adminRepo,
            IDonorRepo donorRepo,
            IDonationRepo donationRepo,
            IRedemptionRepo redemptionRepo,
            IRecipientRepo recipientRepo,
            IUserRepo userRepo,
            IPopulationTypeRepo populationTypeRepo,
            EligibilityService eligibilityService,
            MatchingEventQueue matchingQueue,
            ILogger<AdminService> logger,
            IUnitOfWork unitOfWork)
        {
            _adminRepo = adminRepo;
            _donorRepo = donorRepo;
            _donationRepo = donationRepo;
            _redemptionRepo = redemptionRepo;
            _recipientRepo = recipientRepo;
            _userRepo = userRepo;
            _populationTypeRepo = populationTypeRepo;
            _eligibilityService = eligibilityService;
            _matchingQueue = matchingQueue;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<AdminProfileDto> GetCurrentAdminAsync(
            int userId,
            CancellationToken cancellationToken = default)
        {
            if (userId <= 0)
            {
                throw new ForbiddenException(
                    "The authenticated user identifier is invalid.");
            }

            var admin = await _adminRepo.GetByUserIdAsync(
                userId,
                cancellationToken);

            if (admin?.User is null)
            {
                throw new ForbiddenException(
                    "The authenticated user does not have an admin profile.");
            }

            if (!admin.User.IsActive)
            {
                throw new ForbiddenException(
                    "The administrator account is inactive.");
            }

            return new AdminProfileDto
            {
                AdminId = admin.Id,
                UserId = admin.User.Id,
                Name = admin.User.Name,
                Phone = admin.User.Phone,
                IsActive = admin.User.IsActive,
                CreatedAt = admin.User.CreatedAt
            };
        }

        public async Task<IReadOnlyList<AdminDonorDto>> GetDonorsAsync(
            CancellationToken cancellationToken = default)
        {
            var donors = await _donorRepo.GetAllForAdminAsync(
                cancellationToken);

            return donors
                .Select(donor => new AdminDonorDto
                {
                    Id = donor.Id,
                    UserId = donor.UserId,
                    Name = donor.User.Name,
                    Phone = donor.User.Phone,
                    IsActive = donor.User.IsActive,
                    CreatedAt = donor.User.CreatedAt
                })
                .ToList();
        }

        public async Task<AdminDonorDetailsDto> GetDonorByIdAsync(
            int donorId,
            CancellationToken cancellationToken = default)
        {
            if (donorId <= 0)
            {
                throw new BusinessValidationException(
                    "Donor ID must be positive.");
            }

            var donor = await _donorRepo.GetByIdForAdminAsync(
                donorId,
                cancellationToken);

            if (donor is null)
            {
                throw new ResourceNotFoundException(
                    $"Donor with ID {donorId} was not found.");
            }

            // Return donation history from newest to oldest.
            return new AdminDonorDetailsDto
            {
                Id = donor.Id,
                UserId = donor.UserId,
                Name = donor.User.Name,
                Phone = donor.User.Phone,
                IsActive = donor.User.IsActive,
                CreatedAt = donor.User.CreatedAt,
                Donations = donor.Donations
                    .OrderByDescending(donation => donation.CreatedAt)
                    .Select(donation => new AdminDonorDonationDto
                    {
                        Id = donation.Id,
                        PopulationTypeId = donation.PopulationTypeId,
                        PopulationTypeName = donation.PopulationType.Name,
                        TotalAmount = donation.TotalAmount,
                        RemainingAmount = donation.RemainingAmount,
                        MaxAmountPerRecipient =
                            donation.MaxAmountPerRecipient,
                        Description = donation.Description,
                        CreatedAt = donation.CreatedAt,
                        ExpirationDate = donation.ExpirationDate,
                        Status = donation.Status
                    })
                    .ToList()
            };
        }

        public async Task<AdminDonorDto> UpdateDonorAsync(
            int adminUserId,
            int donorId,
            UpdateDonorDto request,
            CancellationToken cancellationToken = default)
        {
            if (adminUserId <= 0)
            {
                throw new ForbiddenException(
                    "The authenticated user identifier is invalid.");
            }

            if (donorId <= 0)
            {
                throw new BusinessValidationException(
                    "Donor ID must be positive.");
            }

            var name = request.Name?.Trim();
            var phone = request.Phone?.Trim();

            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(phone))
            {
                throw new BusinessValidationException(
                    "Name and phone are required.");
            }

            // Verify that the caller is an active administrator.
            var admin = await _adminRepo.GetByUserIdAsync(
                adminUserId,
                cancellationToken);

            if (admin?.User is null || !admin.User.IsActive)
            {
                throw new ForbiddenException(
                    "An active admin profile is required.");
            }

            await using var transaction =
                await _unitOfWork.BeginTransactionAsync(
                    IsolationLevel.ReadCommitted,
                    cancellationToken);

            var donor = await _donorRepo.GetByIdForAdminAsync(
                donorId,
                cancellationToken);

            if (donor?.User is null)
            {
                throw new ResourceNotFoundException(
                    $"Donor with ID {donorId} was not found.");
            }

            // Lock the linked user before changing unique account fields.
            var user = await _userRepo.GetByIdForUpdateAsync(
                donor.UserId,
                cancellationToken);

            if (user is null)
            {
                throw new ResourceNotFoundException(
                    $"User with ID {donor.UserId} was not found.");
            }

            if (!string.Equals(user.Phone, phone, StringComparison.Ordinal))
            {
                var existingUser = await _userRepo.GetByPhoneAsync(phone);

                if (existingUser is not null && existingUser.Id != user.Id)
                {
                    throw new ConflictException(
                        "Phone number is already in use.");
                }
            }

            user.Name = name;
            user.Phone = phone;

            try
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
                when (ex.InnerException is MySqlConnector.MySqlException
                    { Number: 1062 })
            {
                // The unique phone constraint also protects concurrent updates.
                throw new ConflictException(
                    "Phone number is already in use.");
            }

            await transaction.CommitAsync(cancellationToken);

            return new AdminDonorDto
            {
                Id = donor.Id,
                UserId = user.Id,
                Name = user.Name,
                Phone = user.Phone,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<AdminDonorStatusDto> UpdateDonorStatusAsync(
            int adminUserId,
            int donorId,
            UpdateDonorStatusDto request,
            CancellationToken cancellationToken = default)
        {
            if (adminUserId <= 0)
            {
                throw new ForbiddenException(
                    "The authenticated user identifier is invalid.");
            }

            if (donorId <= 0)
            {
                throw new BusinessValidationException(
                    "Donor ID must be positive.");
            }

            if (!request.IsActive.HasValue)
            {
                throw new BusinessValidationException(
                    "The IsActive field is required.");
            }

            // Verify the authenticated admin account.
            var admin = await _adminRepo.GetByUserIdAsync(
                adminUserId,
                cancellationToken);

            if (admin?.User is null || !admin.User.IsActive)
            {
                throw new ForbiddenException(
                    "An active admin profile is required.");
            }

            await using var transaction =
                await _unitOfWork.BeginTransactionAsync(
                    IsolationLevel.ReadCommitted,
                    cancellationToken);

            var donor = await _donorRepo.GetTrackedByIdForAdminAsync(
                donorId,
                cancellationToken);

            if (donor?.User is null)
            {
                throw new ResourceNotFoundException(
                    $"Donor with ID {donorId} was not found.");
            }

            var cancelledDonationsCount = 0;
            var cancelledRedemptionsCount = 0;

            if (!request.IsActive.Value)
            {
                // Lock donations in a consistent order before cancelling them.
                var donations =
                    await _donationRepo.GetByDonorIdForUpdateAsync(
                        donor.Id,
                        cancellationToken);

                foreach (var donation in donations)
                {
                    if (donation.Status != "ACTIVE")
                    {
                        continue;
                    }

                    var pendingRedemptions =
                        await _redemptionRepo.GetPendingByDonationIdAsync(
                            donation.Id);

                    foreach (var redemption in pendingRedemptions)
                    {
                        if (redemption.Status != "PENDING")
                        {
                            continue;
                        }

                        redemption.Status = "CANCELLED";
                        cancelledRedemptionsCount++;
                    }

                    donation.Status = "CANCELLED";
                    cancelledDonationsCount++;
                }
            }

            // Reactivation does not restore previously cancelled donations.
            donor.User.IsActive = request.IsActive.Value;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new AdminDonorStatusDto
            {
                DonorId = donor.Id,
                UserId = donor.UserId,
                IsActive = donor.User.IsActive,
                CancelledDonationsCount = cancelledDonationsCount,
                CancelledRedemptionsCount = cancelledRedemptionsCount
            };
        }

        public async Task<IReadOnlyList<AdminRecipientDto>>
            GetRecipientsAsync(
                CancellationToken cancellationToken = default)
        {
            var recipients = await _recipientRepo.GetAllForAdminAsync(
                cancellationToken);

            return recipients
                .Select(MapToRecipientDto)
                .ToList();
        }

        public async Task<AdminRecipientDetailsDto> GetRecipientByIdAsync(
            int recipientId,
            CancellationToken cancellationToken = default)
        {
            if (recipientId <= 0)
            {
                throw new BusinessValidationException(
                    "Recipient ID must be positive.");
            }

            var recipient = await _recipientRepo.GetByIdForAdminAsync(
                recipientId,
                cancellationToken);

            if (recipient is null)
            {
                throw new ResourceNotFoundException(
                    $"Recipient with ID {recipientId} was not found.");
            }

            // Return redemption history from newest to oldest.
            return new AdminRecipientDetailsDto
            {
                Id = recipient.Id,
                UserId = recipient.UserId,
                Name = recipient.User.Name,
                Phone = recipient.User.Phone,
                IsActive = recipient.User.IsActive,
                PopulationTypeId = recipient.PopulationTypeId,
                PopulationTypeName = recipient.PopulationType.Name,
                IncomeAmount = recipient.IncomeAmount,
                AmountOfChildren = recipient.AmountOfChildren,
                EligibilityAmount = recipient.EligibilityAmount,
                RemainingEligibility = recipient.RemainingEligibility,
                RequestReason = recipient.RequestReason,
                Redemptions = recipient.Redemptions
                    .OrderByDescending(redemption => redemption.CreatedAt)
                    .Select(redemption => new AdminRecipientRedemptionDto
                    {
                        Id = redemption.Id,
                        DonationId = redemption.DonationId,
                        Status = redemption.Status,
                        Amount = redemption.Amount,
                        CreatedAt = redemption.CreatedAt,
                        RedeemedAt = redemption.RedeemedAt,
                        DonorName = redemption.Donation.Donor.User.Name,
                        DonationDescription =
                            redemption.Donation.Description,
                        DonationTotalAmount =
                            redemption.Donation.TotalAmount,
                        DonationStatus = redemption.Donation.Status
                    })
                    .ToList()
            };
        }

        public async Task<AdminRecipientDto> UpdateRecipientAsync(
            int adminUserId,
            int recipientId,
            UpdateAdminRecipientDto request,
            CancellationToken cancellationToken = default)
        {
            if (adminUserId <= 0)
            {
                throw new ForbiddenException(
                    "The authenticated user identifier is invalid.");
            }

            if (recipientId <= 0)
            {
                throw new BusinessValidationException(
                    "Recipient ID must be positive.");
            }

            var name = request.Name?.Trim();
            var phone = request.Phone?.Trim();
            var requestReason = request.RequestReason?.Trim();

            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(phone))
            {
                throw new BusinessValidationException(
                    "Name and phone are required.");
            }

            if (request.IncomeAmount < 0m ||
                request.IncomeAmount > 99_999_999.99m ||
                decimal.Round(request.IncomeAmount, 2) != request.IncomeAmount ||
                request.AmountOfChildren < 0)
            {
                throw new BusinessValidationException(
                    "Invalid income or children count.");
            }

            if (requestReason?.Length > 250)
            {
                throw new BusinessValidationException(
                    "Request reason cannot exceed 250 characters.");
            }

            // Verify the caller is an active administrator.
            var admin = await _adminRepo.GetByUserIdAsync(
                adminUserId,
                cancellationToken);

            if (admin?.User is null || !admin.User.IsActive)
            {
                throw new ForbiddenException(
                    "An active admin profile is required.");
            }

            decimal newRemainingEligibility;
            decimal newEligibility;
            bool shouldRematch;
            BackendService.Models.Recipient recipient;
            BackendService.Models.User user;
            string populationTypeName;

            await using (var transaction =
                await _unitOfWork.BeginTransactionAsync(
                    IsolationLevel.ReadCommitted,
                    cancellationToken))
            {
                // Lock the recipient before changing its eligibility or account details.
                var lockedRecipient =
                    await _recipientRepo.GetByIdForUpdateWithUserAsync(
                        recipientId,
                        cancellationToken);

                if (lockedRecipient?.User is null)
                {
                    throw new ResourceNotFoundException(
                        $"Recipient with ID {recipientId} was not found.");
                }

                recipient = lockedRecipient;

                // Explicitly lock the linked user before changing the unique phone number.
                var lockedUser = await _userRepo.GetByIdForUpdateAsync(
                    recipient.UserId,
                    cancellationToken);

                if (lockedUser is null)
                {
                    throw new ResourceNotFoundException(
                        $"User with ID {recipient.UserId} was not found.");
                }

                user = lockedUser;

                if (!string.Equals(user.Phone, phone, StringComparison.Ordinal))
                {
                    var existingUser = await _userRepo.GetByPhoneAsync(phone);

                    if (existingUser is not null && existingUser.Id != user.Id)
                    {
                        throw new ConflictException(
                            "Phone number is already in use.");
                    }
                }

                var populationType = await _populationTypeRepo.GetByIdAsync(
                    recipient.PopulationTypeId,
                    cancellationToken);

                if (populationType is null)
                {
                    throw new ResourceNotFoundException(
                        "Recipient population type was not found.");
                }

                populationTypeName = populationType.Name;

                newEligibility = _eligibilityService.CalculateEligibility(
                    populationType.BaseEligibilityAmount,
                    request.IncomeAmount,
                    request.AmountOfChildren);

                var completedTotal =
                    await _redemptionRepo.GetTotalCompletedByRecipientAsync(
                        recipient.Id,
                        cancellationToken);

                newRemainingEligibility =
                    Math.Max(0m, newEligibility - completedTotal);

                shouldRematch =
                    user.IsActive &&
                    newRemainingEligibility > recipient.RemainingEligibility;

                user.Name = name;
                user.Phone = phone;
                recipient.IncomeAmount = request.IncomeAmount;
                recipient.AmountOfChildren = request.AmountOfChildren;
                recipient.RequestReason = string.IsNullOrWhiteSpace(requestReason)
                    ? null
                    : requestReason;
                recipient.EligibilityAmount = newEligibility;
                recipient.RemainingEligibility = newRemainingEligibility;

                try
                {
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
                catch (DbUpdateException ex)
                    when (ex.InnerException is MySqlConnector.MySqlException
                        { Number: 1062 })
                {
                    // The unique phone constraint protects concurrent updates too.
                    throw new ConflictException(
                        "Phone number is already in use.");
                }

                await transaction.CommitAsync(cancellationToken);
            }

            if (shouldRematch)
            {
                await EnqueueRecipientRematchAsync(
                    recipientId,
                    cancellationToken);
            }

            return new AdminRecipientDto
            {
                Id = recipient.Id,
                UserId = recipient.UserId,
                Name = user.Name,
                Phone = user.Phone,
                IsActive = user.IsActive,
                PopulationTypeId = recipient.PopulationTypeId,
                PopulationTypeName = populationTypeName,
                IncomeAmount = recipient.IncomeAmount,
                AmountOfChildren = recipient.AmountOfChildren,
                EligibilityAmount = newEligibility,
                RemainingEligibility = newRemainingEligibility
            };
        }

        private async Task EnqueueRecipientRematchAsync(
            int recipientId,
            CancellationToken cancellationToken)
        {
            try
            {
                await _matchingQueue.EnqueueAsync(
                    new MatchingEvent(
                        MatchingEventType.RecipientRegistered,
                        recipientId),
                    cancellationToken);
            }
            catch (OperationCanceledException ex)
                when (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning(
                    ex,
                    "Matching event enqueue timed out for recipient {RecipientId}",
                    recipientId);
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning(
                    "Request was cancelled after saving recipient {RecipientId}",
                    recipientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to enqueue matching event for recipient {RecipientId}",
                    recipientId);
            }
        }

        public async Task<AdminRecipientStatusDto>
            UpdateRecipientStatusAsync(
                int adminUserId,
                int recipientId,
                UpdateRecipientStatusDto request,
                CancellationToken cancellationToken = default)
        {
            if (adminUserId <= 0)
            {
                throw new ForbiddenException(
                    "The authenticated user identifier is invalid.");
            }

            if (recipientId <= 0)
            {
                throw new BusinessValidationException(
                    "Recipient ID must be positive.");
            }

            if (!request.IsActive.HasValue)
            {
                throw new BusinessValidationException(
                    "The IsActive field is required.");
            }

            // Verify the authenticated admin account.
            var admin = await _adminRepo.GetByUserIdAsync(
                adminUserId,
                cancellationToken);

            if (admin?.User is null || !admin.User.IsActive)
            {
                throw new ForbiddenException(
                    "An active admin profile is required.");
            }

            await using var transaction =
                await _unitOfWork.BeginTransactionAsync(
                    IsolationLevel.ReadCommitted,
                    cancellationToken);

            var cancelledRedemptionsCount = 0;

            if (!request.IsActive.Value)
            {
                // Lock all related donations before locking the recipient.
                var donationIds =
                    await _redemptionRepo.GetPendingDonationIdsByRecipientIdAsync(
                        recipientId,
                        cancellationToken);

                foreach (var donationId in donationIds.OrderBy(id => id))
                {
                    var donation =
                        await _donationRepo.GetByIdForUpdateAsync(
                            donationId,
                            cancellationToken);

                    if (donation is null)
                    {
                        throw new ResourceNotFoundException(
                            $"Donation with ID {donationId} was not found.");
                    }
                }
            }

            // Lock the recipient after acquiring donation locks.
            var recipient =
                await _recipientRepo.GetByIdForUpdateWithUserAsync(
                    recipientId,
                    cancellationToken);

            if (recipient?.User is null)
            {
                throw new ResourceNotFoundException(
                    $"Recipient with ID {recipientId} was not found.");
            }

            if (!request.IsActive.Value)
            {
                var pendingRedemptions =
                    await _redemptionRepo.GetPendingByRecipientIdForUpdateAsync(
                        recipientId,
                        cancellationToken);

                foreach (var redemption in pendingRedemptions)
                {
                    if (redemption.Status != "PENDING")
                    {
                        continue;
                    }

                    redemption.Status = "CANCELLED";
                    cancelledRedemptionsCount++;
                }
            }

            recipient.User.IsActive = request.IsActive.Value;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new AdminRecipientStatusDto
            {
                RecipientId = recipient.Id,
                UserId = recipient.UserId,
                IsActive = recipient.User.IsActive,
                CancelledRedemptionsCount = cancelledRedemptionsCount
            };
        }

        private static AdminDonorDto MapToDonorDto(
            BackendService.Models.Donor donor)
        {
            return new AdminDonorDto
            {
                Id = donor.Id,
                UserId = donor.UserId,
                Name = donor.User.Name,
                Phone = donor.User.Phone,
                IsActive = donor.User.IsActive,
                CreatedAt = donor.User.CreatedAt
            };
        }

        private static AdminRecipientDto MapToRecipientDto(
            BackendService.Models.Recipient recipient)
        {
            return new AdminRecipientDto
            {
                Id = recipient.Id,
                UserId = recipient.UserId,
                Name = recipient.User.Name,
                Phone = recipient.User.Phone,
                IsActive = recipient.User.IsActive,
                PopulationTypeId = recipient.PopulationTypeId,
                PopulationTypeName = recipient.PopulationType.Name,
                IncomeAmount = recipient.IncomeAmount,
                AmountOfChildren = recipient.AmountOfChildren,
                EligibilityAmount = recipient.EligibilityAmount,
                RemainingEligibility = recipient.RemainingEligibility
            };
        }
    }
}