using System.Data;
using BackendService.Data;
using BackendService.DTOs;
using BackendService.Exceptions;
using BackendService.Models;
using BackendService.Repos;

namespace BackendService.Services
{
    public sealed class AdminRedemptionService
    {
        private static readonly HashSet<string> SupportedStatuses =
            new(StringComparer.Ordinal)
            {
                "PENDING",
                "COMPLETED",
                "CANCELLED",
                "REJECTED"
            };

        private readonly IAdminRepo _adminRepo;
        private readonly IRedemptionRepo _redemptionRepo;
        private readonly IDonationRepo _donationRepo;
        private readonly IRecipientRepo _recipientRepo;
        private readonly IDonorRepo _donorRepo;
        private readonly IUnitOfWork _unitOfWork;

        public AdminRedemptionService(
            IAdminRepo adminRepo,
            IRedemptionRepo redemptionRepo,
            IDonationRepo donationRepo,
            IRecipientRepo recipientRepo,
            IDonorRepo donorRepo,
            IUnitOfWork unitOfWork)
        {
            _adminRepo = adminRepo;
            _redemptionRepo = redemptionRepo;
            _donationRepo = donationRepo;
            _recipientRepo = recipientRepo;
            _donorRepo = donorRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<AdminRedemptionsPageDto> GetRedemptionsAsync(
            int? populationTypeId,
            string? status,
            DateTime? fromDate,
            DateTime? toDate,
            int page = 1,
            int pageSize = 50,
            CancellationToken cancellationToken = default)
        {
            if (populationTypeId.HasValue &&
                populationTypeId.Value <= 0)
            {
                throw new BusinessValidationException(
                    "Population type ID must be positive.");
            }

            if (page <= 0)
            {
                throw new BusinessValidationException(
                    "Page must be greater than zero.");
            }

            if (pageSize is < 1 or > 100)
            {
                throw new BusinessValidationException(
                    "Page size must be between 1 and 100.");
            }

            if ((long)(page - 1) * pageSize > int.MaxValue)
            {
                throw new BusinessValidationException(
                    "The requested page is too large.");
            }

            var normalizedStatus = string.IsNullOrWhiteSpace(status)
                ? null
                : status.Trim().ToUpperInvariant();

            if (normalizedStatus is not null &&
                !SupportedStatuses.Contains(normalizedStatus))
            {
                throw new BusinessValidationException(
                    "Status must be PENDING, COMPLETED, CANCELLED, or REJECTED.");
            }

            var normalizedFromDate = fromDate?.Date;
            var normalizedToDate = toDate?.Date;

            if (normalizedFromDate.HasValue &&
                normalizedToDate.HasValue &&
                normalizedFromDate.Value > normalizedToDate.Value)
            {
                throw new BusinessValidationException(
                    "From date cannot be later than to date.");
            }

            var (items, totalCount) =
                await _redemptionRepo.GetForAdminAsync(
                    populationTypeId,
                    normalizedStatus,
                    normalizedFromDate,
                    normalizedToDate,
                    page,
                    pageSize,
                    cancellationToken);

            return new AdminRedemptionsPageDto
            {
                Items = items.Select(MapToDto).ToList(),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public Task<AdminRedemptionsPageDto> GetPendingRedemptionsAsync(
            int? populationTypeId,
            DateTime? fromDate,
            DateTime? toDate,
            int page = 1,
            int pageSize = 50,
            CancellationToken cancellationToken = default)
        {
            return GetRedemptionsAsync(
                populationTypeId,
                "PENDING",
                fromDate,
                toDate,
                page,
                pageSize,
                cancellationToken);
        }

        public async Task<AdminRedemptionDto> GetRedemptionByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                throw new BusinessValidationException(
                    "Redemption ID must be positive.");
            }

            var redemption =
                await _redemptionRepo.GetByIdForAdminAsync(
                    id,
                    cancellationToken);

            if (redemption is null)
            {
                throw new ResourceNotFoundException(
                    $"Redemption with ID {id} was not found.");
            }

            return MapToDto(redemption);
        }

        public async Task<AdminRedemptionDecisionDto>
            ApproveRedemptionAsync(
                int userId,
                int redemptionId,
                CancellationToken cancellationToken = default)
        {
            if (userId <= 0)
            {
                throw new ForbiddenException(
                    "The authenticated user identifier is invalid.");
            }

            if (redemptionId <= 0)
            {
                throw new BusinessValidationException(
                    "Redemption ID must be positive.");
            }

            // Resolve the authenticated admin before changing data.
            var admin = await _adminRepo.GetByUserIdAsync(
                userId,
                cancellationToken);

            if (admin?.User is null || !admin.User.IsActive)
            {
                throw new ForbiddenException(
                    "An active admin profile is required.");
            }

            // Read the related IDs before taking locks.
            var redemptionReference =
                await _redemptionRepo.GetByIdForAdminAsync(
                    redemptionId,
                    cancellationToken);

            if (redemptionReference is null)
            {
                throw new ResourceNotFoundException(
                    $"Redemption with ID {redemptionId} was not found.");
            }

            await using var transaction =
                await _unitOfWork.BeginTransactionAsync(
                    IsolationLevel.ReadCommitted,
                    cancellationToken);

            // Always lock the donation before the recipient.
            var donation =
                await _donationRepo.GetByIdForUpdateAsync(
                    redemptionReference.DonationId,
                    cancellationToken);

            if (donation is null)
            {
                throw new ResourceNotFoundException(
                    "The donation associated with this redemption was not found.");
            }

            var recipient =
                await _recipientRepo.GetByIdForUpdateAsync(
                    redemptionReference.RecipientId,
                    cancellationToken);

            if (recipient is null)
            {
                throw new ResourceNotFoundException(
                    "The recipient associated with this redemption was not found.");
            }

            // Lock and re-read the redemption after the other locks.
            var redemption =
                await _redemptionRepo.GetByIdForUpdateAsync(
                    redemptionId,
                    cancellationToken);

            if (redemption is null)
            {
                throw new ResourceNotFoundException(
                    $"Redemption with ID {redemptionId} was not found.");
            }

            if (redemption.DonationId != donation.Id ||
                redemption.RecipientId != recipient.Id ||
                redemption.DonationId != redemptionReference.DonationId ||
                redemption.RecipientId != redemptionReference.RecipientId)
            {
                throw new ConflictException(
                    "The redemption relationships changed. Please retry.");
            }

            if (redemption.Status != "PENDING")
            {
                throw new ConflictException(
                    "Only pending redemptions can be approved.");
            }

            var utcNow = DateTime.UtcNow;

            if (donation.Status != "ACTIVE" ||
                (donation.ExpirationDate.HasValue &&
                 donation.ExpirationDate.Value <= utcNow))
            {
                throw new ConflictException(
                    "The donation is inactive or has expired.");
            }

            if (!await _donorRepo.IsDonorActiveAsync(donation.DonorId))
            {
                throw new ConflictException(
                    "The donor account is inactive.");
            }

            if (!await _recipientRepo.IsRecipientActiveAsync(recipient.Id))
            {
                throw new ConflictException(
                    "The recipient account is inactive.");
            }

            if (donation.PopulationTypeId != recipient.PopulationTypeId)
            {
                throw new ConflictException(
                    "The donation and recipient population types do not match.");
            }

            // Recalculate the cumulative recipient cap.
            var completedTotal =
                await _redemptionRepo.GetCompletedTotalAsync(
                    donation.Id,
                    recipient.Id,
                    cancellationToken);

            var remainingRecipientCap =
                donation.MaxAmountPerRecipient - completedTotal;

            var approvedAmount = Math.Max(
                0m,
                Math.Min(
                    donation.RemainingAmount,
                    Math.Min(
                        recipient.RemainingEligibility,
                        remainingRecipientCap)));

            if (approvedAmount <= 0)
            {
                throw new ConflictException(
                    "No positive amount can be approved. " +
                    "The redemption remains pending and balances are unchanged.");
            }

            // Update the redemption and both balances atomically.
            donation.RemainingAmount -= approvedAmount;
            recipient.RemainingEligibility -= approvedAmount;

            redemption.Amount = approvedAmount;
            redemption.Status = "COMPLETED";
            redemption.RedeemedAt = utcNow;
            redemption.HandledByAdminId = admin.Id;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return new AdminRedemptionDecisionDto
            {
                RedemptionId = redemption.Id,
                Status = redemption.Status,
                ApprovedAmount = approvedAmount,
                HandledByAdminId = admin.Id,
                RedeemedAt = utcNow,
                DonationRemainingAmount = donation.RemainingAmount,
                RecipientRemainingEligibility =
                    recipient.RemainingEligibility
            };
        }


        public async Task<AdminRedemptionRejectionDto> RejectRedemptionAsync(
            int userId,
            int redemptionId,
            CancellationToken cancellationToken = default)
        {
            if (userId <= 0)
            {
                throw new ForbiddenException(
                    "The authenticated user identifier is invalid.");
            }

            if (redemptionId <= 0)
            {
                throw new BusinessValidationException(
                    "Redemption ID must be positive.");
            }

            // Resolve the authenticated admin.
            var admin = await _adminRepo.GetByUserIdAsync(
                userId,
                cancellationToken);

            if (admin?.User is null || !admin.User.IsActive)
            {
                throw new ForbiddenException(
                    "An active admin profile is required.");
            }

            // Read the related IDs before acquiring locks.
            var redemptionReference =
                await _redemptionRepo.GetByIdForAdminAsync(
                    redemptionId,
                    cancellationToken);

            if (redemptionReference is null)
            {
                throw new ResourceNotFoundException(
                    $"Redemption with ID {redemptionId} was not found.");
            }

            await using var transaction =
                await _unitOfWork.BeginTransactionAsync(
                    IsolationLevel.ReadCommitted,
                    cancellationToken);

            // Keep the same lock order used by the approval workflow.
            var donation = await _donationRepo.GetByIdForUpdateAsync(
                redemptionReference.DonationId,
                cancellationToken);

            if (donation is null)
            {
                throw new ResourceNotFoundException(
                    "The associated donation was not found.");
            }

            var recipient = await _recipientRepo.GetByIdForUpdateAsync(
                redemptionReference.RecipientId,
                cancellationToken);

            if (recipient is null)
            {
                throw new ResourceNotFoundException(
                    "The associated recipient was not found.");
            }

            // Lock and re-read the redemption before changing its status.
            var redemption = await _redemptionRepo.GetByIdForUpdateAsync(
                redemptionId,
                cancellationToken);

            if (redemption is null)
            {
                throw new ResourceNotFoundException(
                    $"Redemption with ID {redemptionId} was not found.");
            }

            if (redemption.DonationId != donation.Id ||
                redemption.RecipientId != recipient.Id ||
                redemption.DonationId != redemptionReference.DonationId ||
                redemption.RecipientId != redemptionReference.RecipientId)
            {
                throw new ConflictException(
                    "The redemption relationships changed. Please retry.");
            }

            if (redemption.Status != "PENDING")
            {
                throw new ConflictException(
                    "Only pending redemptions can be rejected.");
            }

            // Reject the request without changing either balance.
            redemption.Status = "REJECTED";
            redemption.HandledByAdminId = admin.Id;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return new AdminRedemptionRejectionDto
            {
                RedemptionId = redemption.Id,
                Status = redemption.Status,
                HandledByAdminId = admin.Id
            };
        }


        private static AdminRedemptionDto MapToDto(
            Redemption redemption)
        {
            return new AdminRedemptionDto
            {
                Id = redemption.Id,
                DonationId = redemption.DonationId,
                RecipientId = redemption.RecipientId,
                Status = redemption.Status,
                Amount = redemption.Amount,
                CreatedAt = redemption.CreatedAt,
                RedeemedAt = redemption.RedeemedAt,
                HandledByAdminId = redemption.HandledByAdminId,

                Recipient = new AdminRedemptionRecipientDto
                {
                    Id = redemption.Recipient.Id,
                    UserId = redemption.Recipient.UserId,
                    Name = redemption.Recipient.User.Name,
                    Phone = redemption.Recipient.User.Phone,
                    PopulationTypeId =
                        redemption.Recipient.PopulationTypeId,
                    PopulationTypeName =
                        redemption.Recipient.PopulationType.Name,
                    IncomeAmount = redemption.Recipient.IncomeAmount,
                    AmountOfChildren =
                        redemption.Recipient.AmountOfChildren,
                    EligibilityAmount =
                        redemption.Recipient.EligibilityAmount,
                    RemainingEligibility =
                        redemption.Recipient.RemainingEligibility
                },

                Donation = new AdminRedemptionDonationDto
                {
                    Id = redemption.Donation.Id,
                    DonorId = redemption.Donation.DonorId,
                    DonorName = redemption.Donation.Donor.User.Name,
                    DonorPhone = redemption.Donation.Donor.User.Phone,
                    PopulationTypeId =
                        redemption.Donation.PopulationTypeId,
                    PopulationTypeName =
                        redemption.Donation.PopulationType.Name,
                    TotalAmount = redemption.Donation.TotalAmount,
                    RemainingAmount =
                        redemption.Donation.RemainingAmount,
                    MaxAmountPerRecipient =
                        redemption.Donation.MaxAmountPerRecipient,
                    Description = redemption.Donation.Description,
                    CreatedAt = redemption.Donation.CreatedAt,
                    ExpirationDate =
                        redemption.Donation.ExpirationDate,
                    Status = redemption.Donation.Status
                }
            };
        }
    }
}