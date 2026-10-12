using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendService.Models;
using BackendService.Repos;

namespace BackendService.Services
{
    public class DonationMatchingService
    {
        private readonly IRedemptionRepo _redemptionRepo;

        public DonationMatchingService(IRedemptionRepo redemptionRepo)
        {
            _redemptionRepo = redemptionRepo;
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
    }
}