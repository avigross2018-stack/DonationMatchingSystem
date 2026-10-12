using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BackendService.Data;
using BackendService.Models;
using BackendService.Repos;

namespace BackendService.Services
{
    public class DonationMatchingService
    {
        private readonly IRedemptionRepo _redemptionRepo;
        private readonly IDonationRepo _donationRepo;
        private readonly IRecipientRepo _recipientRepo;
        private readonly IDonorRepo _donorRepo;
        private readonly IUnitOfWork _unitOfWork;

        public DonationMatchingService(IDonationRepo donationRepo,
            IRecipientRepo recipientRepo,
            IRedemptionRepo redemptionRepo,
            IDonorRepo donorRepo,
            IUnitOfWork unitOfWork)
        {
            _donationRepo = donationRepo;
            _recipientRepo = recipientRepo;
            _donorRepo = donorRepo;
            _redemptionRepo = redemptionRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<decimal> CalculateMatchingAmountAsync(
            Donation donation,
            Recipient recipient,
            CancellationToken cancellationToken = default)
        {
            var completedTotal = await _redemptionRepo
                .GetCompletedTotalAsync(
                    donation.Id,
                    recipient.Id,
                    cancellationToken);

            var remainingCap =
                donation.MaxAmountPerRecipient - completedTotal;

            var matchingAmount = Math.Min(
                donation.RemainingAmount,
                Math.Min(
                    recipient.RemainingEligibility,
                    remainingCap));

            return Math.Max(0m, matchingAmount);
        }

        public async Task<bool> TryCreateMatchAsync(
            int donationId,
            int recipientId,
            CancellationToken cancellationToken = default)
        {
            await using var transaction =
                await _unitOfWork.BeginTransactionAsync(
                    IsolationLevel.ReadCommitted,
                    cancellationToken);

            // Lock the donation row
            var donation = await _donationRepo
                .GetByIdForUpdateAsync(donationId, cancellationToken);

            if (donation == null)
                return false;

            var recipient = await _recipientRepo
                .GetByIdAsync(recipientId);

            if (recipient == null)
                return false;

            // Check recipient activity
            if (!recipient.User.IsActive)
                return false;

            // Check donor activity
            var isDonorActive = await _donorRepo
                .IsDonorActiveAsync(donation.DonorId);

            if (!isDonorActive)
                return false;

            var utcNow = DateTime.UtcNow;

            // Validate donation
            if (donation.Status != "ACTIVE" ||
                donation.ExpirationDate <= utcNow ||
                donation.RemainingAmount <= 0)
                return false;

            // Validate recipient
            if (recipient.RemainingEligibility <= 0)
                return false;

            // Verify population type
            if (donation.PopulationTypeId != recipient.PopulationTypeId)
                return false;

            // Check existing PENDING / REJECTED
            var blocked = await _redemptionRepo
                .HasBlockedMatchAsync(
                    donationId,
                    recipientId,
                    cancellationToken);

            if (blocked)
                return false;

            // Calculate remaining allowed amount
            var amount = await CalculateMatchingAmountAsync(
                donation,
                recipient,
                cancellationToken);

            if (amount <= 0)
                return false;

            // Create pending redemption
            var redemption = new Redemption
            {
                DonationId = donationId,
                RecipientId = recipientId,
                Amount = amount,
                Status = "PENDING",
                CreatedAt = utcNow
            };

            await _redemptionRepo.AddAsync(
                redemption,
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return true;
        }      
    }
}