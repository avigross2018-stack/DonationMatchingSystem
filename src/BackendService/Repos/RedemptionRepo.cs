using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendService.Data;
using BackendService.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendService.Repos
{
    public class RedemptionRepo : IRedemptionRepo
    {
        private readonly AppDbContext _appDb;
        public RedemptionRepo(AppDbContext appDb)
        {
            _appDb = appDb;
        }

        public async Task<bool> HasBlockedMatchAsync(
            int donationId,
            int recipientId,
            CancellationToken cancellationToken = default)
        {
            return await _appDb.Redemptions
                .AnyAsync(r =>
                    r.DonationId == donationId &&
                    r.RecipientId == recipientId &&
                    (r.Status == "PENDING" ||
                    r.Status == "REJECTED"),
                    cancellationToken);
        }

        public async Task<decimal> GetCompletedTotalAsync(
            int donationId,
            int recipientId,
            CancellationToken cancellationToken = default)
        {
            return await _appDb.Redemptions
                .Where(r =>
                    r.DonationId == donationId &&
                    r.RecipientId == recipientId &&
                    r.Status == "COMPLETED")
                .SumAsync(r => (decimal?)r.Amount, cancellationToken)
                ?? 0m;
        }

        public async Task AddAsync(
            Redemption redemption,
            CancellationToken cancellationToken = default)
        {
            await _appDb.Redemptions.AddAsync(
                redemption,
                cancellationToken);
        }

        public async Task<List<Redemption>> GetPendingByDonationIdAsync(
            int donationId)
        {
            return await _appDb.Redemptions
                .Where(r =>
                    r.DonationId == donationId &&
                    r.Status == "PENDING")
                .ToListAsync();
        }
    }
}