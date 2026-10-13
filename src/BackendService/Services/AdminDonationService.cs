using System.Data;
using BackendService.Data;
using BackendService.DTOs;
using BackendService.Exceptions;
using BackendService.Models;
using BackendService.Repos;

namespace BackendService.Services
{
    public sealed class AdminDonationService
    {
        private static readonly HashSet<string> SupportedStatuses =
            new(StringComparer.Ordinal)
            {
                "ACTIVE",
                "COMPLETED",
                "CANCELLED",
                "EXPIRED"
            };

        private readonly IDonationRepo _donationRepo;
        private readonly IRedemptionRepo _redemptionRepo;
        private readonly IUnitOfWork _unitOfWork;

        public AdminDonationService(
            IDonationRepo donationRepo,
            IRedemptionRepo redemptionRepo,
            IUnitOfWork unitOfWork)
        {
            _donationRepo = donationRepo;
            _redemptionRepo = redemptionRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<AdminDonationsPageDto> GetAllAsync(
            int? donorId,
            int? populationTypeId,
            string? status,
            DateTime? fromDate,
            DateTime? toDate,
            int page = 1,
            int pageSize = 50,
            CancellationToken cancellationToken = default)
        {
            if (donorId.HasValue && donorId.Value <= 0)
            {
                throw new BusinessValidationException(
                    "Donor ID must be positive.");
            }

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
                    "Status must be ACTIVE, COMPLETED, CANCELLED, or EXPIRED.");
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
                await _donationRepo.GetForAdminAsync(
                    donorId,
                    populationTypeId,
                    normalizedStatus,
                    normalizedFromDate,
                    normalizedToDate,
                    page,
                    pageSize,
                    cancellationToken);

            return new AdminDonationsPageDto
            {
                Items = items.Select(MapToDto).ToList(),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<AdminDonationDetailsDto> GetByIdAsync(
            int donationId,
            CancellationToken cancellationToken = default)
        {
            if (donationId <= 0)
            {
                throw new BusinessValidationException(
                    "Donation ID must be positive.");
            }

            var donation = await _donationRepo.GetByIdForAdminAsync(
                donationId,
                cancellationToken);

            if (donation is null)
            {
                throw new ResourceNotFoundException(
                    $"Donation with ID {donationId} was not found.");
            }

            // Return redemption history from newest to oldest.
            return new AdminDonationDetailsDto
            {
                Id = donation.Id,
                DonorId = donation.DonorId,
                DonorName = donation.Donor.User.Name,
                DonorPhone = donation.Donor.User.Phone,
                PopulationTypeId = donation.PopulationTypeId,
                PopulationTypeName = donation.PopulationType.Name,
                TotalAmount = donation.TotalAmount,
                RemainingAmount = donation.RemainingAmount,
                MaxAmountPerRecipient = donation.MaxAmountPerRecipient,
                Description = donation.Description,
                CreatedAt = donation.CreatedAt,
                ExpirationDate = donation.ExpirationDate,
                Status = donation.Status,
                Redemptions = donation.Redemptions
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(r => new AdminDonationRedemptionDto
                    {
                        Id = r.Id,
                        RecipientId = r.RecipientId,
                        RecipientName = r.Recipient.User.Name,
                        Status = r.Status,
                        Amount = r.Amount,
                        CreatedAt = r.CreatedAt,
                        RedeemedAt = r.RedeemedAt,
                        HandledByAdminId = r.HandledByAdminId
                    })
                    .ToList()
            };
        }

        public async Task UpdateAsync(
            int donationId,
            UpdateAdminDonationDto request,
            CancellationToken cancellationToken = default)
        {
            if (donationId <= 0)
            {
                throw new BusinessValidationException(
                    "Donation ID must be positive.");
            }

            if (request.TotalAmount <= 0 ||
                request.MaxAmountPerRecipient <= 0)
            {
                throw new BusinessValidationException(
                    "Donation amounts must be positive.");
            }

            if (!request.ExpirationDate.HasValue)
            {
                throw new BusinessValidationException(
                    "Expiration date is required.");
            }

            var newExpirationDate =
                request.ExpirationDate.Value.ToUniversalTime();

            if (newExpirationDate <= DateTime.UtcNow)
            {
                throw new BusinessValidationException(
                    "Expiration date must be in the future.");
            }

            // Start the transaction before locking the donation.
            await using var transaction =
                await _unitOfWork.BeginTransactionAsync(
                    IsolationLevel.ReadCommitted,
                    cancellationToken);

            var donation = await _donationRepo.GetByIdForUpdateAsync(
                donationId,
                cancellationToken);

            if (donation is null)
            {
                throw new ResourceNotFoundException(
                    $"Donation with ID {donationId} was not found.");
            }

            var utcNow = DateTime.UtcNow;

            // Only active, non-expired donations can be edited.
            if (donation.Status != "ACTIVE" ||
                (donation.ExpirationDate.HasValue &&
                 donation.ExpirationDate.Value <= utcNow))
            {
                throw new ConflictException(
                    "Only active, non-expired donations can be edited.");
            }

            var completedTotal =
                await _redemptionRepo.GetCompletedTotalByDonationIdAsync(
                    donationId,
                    cancellationToken);

            if (request.TotalAmount < completedTotal)
            {
                throw new ConflictException(
                    "Total amount cannot be lower than the amount " +
                    "already approved.");
            }

            donation.TotalAmount = request.TotalAmount;

            // Preserve completed redemption amounts when recalculating balance.
            donation.RemainingAmount =
                request.TotalAmount - completedTotal;

            donation.MaxAmountPerRecipient =
                request.MaxAmountPerRecipient;

            donation.Description = request.Description;
            donation.ExpirationDate = newExpirationDate;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }

        public async Task CancelAsync(
            int donationId,
            CancellationToken cancellationToken = default)
        {
            if (donationId <= 0)
            {
                throw new BusinessValidationException(
                    "Donation ID must be positive.");
            }

            await using var transaction =
                await _unitOfWork.BeginTransactionAsync(
                    IsolationLevel.ReadCommitted,
                    cancellationToken);

            // Lock the donation first to serialize cancellation and redemption approval.
            var donation = await _donationRepo.GetByIdForUpdateAsync(
                donationId,
                cancellationToken);

            if (donation is null)
            {
                throw new ResourceNotFoundException(
                    $"Donation with ID {donationId} was not found.");
            }

            if (donation.Status != "ACTIVE")
            {
                throw new ConflictException(
                    "Only active donations can be cancelled.");
            }

            // Read pending redemptions after acquiring the donation lock.
            var pendingRedemptions =
                await _redemptionRepo.GetPendingByDonationIdAsync(
                    donationId);

            donation.Status = "CANCELLED";

            foreach (var redemption in pendingRedemptions)
            {
                // Completed and rejected redemptions are never modified.
                if (redemption.Status == "PENDING")
                {
                    redemption.Status = "CANCELLED";
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }

        private static AdminDonationDto MapToDto(Donation donation)
        {
            return new AdminDonationDto
            {
                Id = donation.Id,
                DonorId = donation.DonorId,
                DonorName = donation.Donor.User.Name,
                DonorPhone = donation.Donor.User.Phone,
                PopulationTypeId = donation.PopulationTypeId,
                PopulationTypeName = donation.PopulationType.Name,
                TotalAmount = donation.TotalAmount,
                RemainingAmount = donation.RemainingAmount,
                MaxAmountPerRecipient = donation.MaxAmountPerRecipient,
                Description = donation.Description,
                CreatedAt = donation.CreatedAt,
                ExpirationDate = donation.ExpirationDate,
                Status = donation.Status
            };
        }
    }
}