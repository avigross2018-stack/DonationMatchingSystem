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
                .SumAsync(
                    r => (decimal?)r.Amount,
                    cancellationToken)
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

        public async Task<decimal> GetTotalCompletedByRecipientAsync(
            int recipientId,
            CancellationToken cancellationToken = default)
        {
            return await _appDb.Redemptions
                .Where(r =>
                    r.RecipientId == recipientId &&
                    r.Status == "COMPLETED")
                .SumAsync(
                    r => (decimal?)r.Amount,
                    cancellationToken)
                ?? 0m;
        }

        public async Task<List<Redemption>> GetByRecipientIdAsync(
            int recipientId,
            CancellationToken cancellationToken = default)
        {
            return await _appDb.Redemptions
                .AsNoTracking()
                .Include(r => r.Donation)
                .Where(r => r.RecipientId == recipientId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<int>> GetPendingDonationIdsByRecipientIdAsync(
            int recipientId,
            CancellationToken cancellationToken = default)
        {
            // Return donation IDs in a consistent lock order.
            return await _appDb.Redemptions
                .AsNoTracking()
                .Where(r =>
                    r.RecipientId == recipientId &&
                    r.Status == "PENDING")
                .Select(r => r.DonationId)
                .Distinct()
                .OrderBy(donationId => donationId)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Redemption>> GetPendingByRecipientIdAsync(
            int recipientId,
            CancellationToken cancellationToken = default)
        {
            return await _appDb.Redemptions
                .Where(r =>
                    r.RecipientId == recipientId &&
                    r.Status == "PENDING")
                .OrderBy(r => r.DonationId)
                .ThenBy(r => r.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Redemption>>
            GetPendingByRecipientIdForUpdateAsync(
                int recipientId,
                CancellationToken cancellationToken = default)
        {
            // Lock pending redemptions after locking donations and recipient.
            var query = _appDb.Redemptions
                .FromSqlInterpolated(
                    $"SELECT * FROM `Redemptions` WHERE `RecipientId` = {recipientId} AND `Status` = 'PENDING' ORDER BY `DonationId`, `Id` FOR UPDATE")
                .AsAsyncEnumerable();

            var redemptions = new List<Redemption>();

            await foreach (var redemption in query
                .WithCancellation(cancellationToken))
            {
                redemptions.Add(redemption);
            }

            return redemptions;
        }

        public async Task<(List<Redemption> Items, int TotalCount)>
            GetForAdminAsync(
                int? populationTypeId,
                string? status,
                DateTime? fromDate,
                DateTime? toDate,
                int page,
                int pageSize,
                CancellationToken cancellationToken = default)
        {
            var query = BuildAdminQuery();

            if (populationTypeId.HasValue)
            {
                query = query.Where(r =>
                    r.Donation.PopulationTypeId == populationTypeId.Value);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(r => r.Status == status);
            }

            if (fromDate.HasValue)
            {
                var startDate = fromDate.Value.Date;

                query = query.Where(r => r.CreatedAt >= startDate);
            }

            if (toDate.HasValue)
            {
                var endDateExclusive = toDate.Value.Date.AddDays(1);

                query = query.Where(r => r.CreatedAt < endDateExclusive);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(r => r.CreatedAt)
                .ThenByDescending(r => r.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<Redemption?> GetByIdForAdminAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await BuildAdminQuery()
                .FirstOrDefaultAsync(
                    r => r.Id == id,
                    cancellationToken);
        }

        public async Task<Redemption?> GetByIdForUpdateAsync(
            int redemptionId,
            CancellationToken cancellationToken = default)
        {
            var query = _appDb.Redemptions
                .FromSqlInterpolated(
                    $"SELECT * FROM `Redemptions` WHERE `Id` = {redemptionId} FOR UPDATE")
                .AsAsyncEnumerable();

            await foreach (var redemption in query
                .WithCancellation(cancellationToken))
            {
                return redemption;
            }

            return null;
        }

        private IQueryable<Redemption> BuildAdminQuery()
        {
            return _appDb.Redemptions
                .AsNoTracking()
                .Include(r => r.Recipient)
                    .ThenInclude(recipient => recipient.User)
                .Include(r => r.Recipient)
                    .ThenInclude(recipient => recipient.PopulationType)
                .Include(r => r.Donation)
                    .ThenInclude(donation => donation.Donor)
                        .ThenInclude(donor => donor.User)
                .Include(r => r.Donation)
                    .ThenInclude(donation => donation.PopulationType);
        }

        public async Task<decimal> GetCompletedTotalByDonationIdAsync(
            int donationId,
            CancellationToken cancellationToken = default)
        {
            return await _appDb.Redemptions
                       .Where(r =>
                           r.DonationId == donationId &&
                           r.Status == "COMPLETED")
                       .SumAsync(
                           r => (decimal?)r.Amount,
                           cancellationToken)
                   ?? 0m;
        }
    }
}