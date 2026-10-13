using BackendService.Data;
using BackendService.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendService.Repos
{
    public class DonationRepo : IDonationRepo
    {
        private readonly AppDbContext _appDb;

        public DonationRepo(AppDbContext appDb)
        {
            _appDb = appDb;
        }

        public async Task AddAsync(Donation donation)
        {
            await _appDb.Donations.AddAsync(donation);
        }

        public async Task<List<Donation>> GetAvailableDonationsAsync(
            DateTime utcNow,
            CancellationToken cancellationToken = default)
        {
            return await _appDb.Donations
                .AsNoTracking()
                .Where(d =>
                    d.Status == "ACTIVE" &&
                    (d.ExpirationDate == null ||
                     d.ExpirationDate > utcNow) &&
                    d.RemainingAmount > 0 &&
                    d.Donor.User.IsActive)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Donation>> GetByDonorIdAsync(int donorId)
        {
            return await _appDb.Donations
                .Where(d => d.DonorId == donorId)
                .ToListAsync();
        }

        public async Task<List<Donation>> GetByDonorIdForUpdateAsync(
            int donorId,
            CancellationToken cancellationToken = default)
        {
            // Lock donations in a consistent order.
            var query = _appDb.Donations
                .FromSqlInterpolated(
                    $"SELECT * FROM `Donations` WHERE `DonorId` = {donorId} ORDER BY `Id` FOR UPDATE")
                .AsAsyncEnumerable();

            var donations = new List<Donation>();

            await foreach (var donation in query
                .WithCancellation(cancellationToken))
            {
                donations.Add(donation);
            }

            return donations;
        }

        public async Task<Donation?> GetByIdAsync(int id)
        {
            return await _appDb.Donations
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<List<Donation>> GetExpiredActiveDonationsAsync(
            DateTime utcNow,
            CancellationToken cancellationToken = default)
        {
            return await _appDb.Donations
                .Where(d =>
                    d.Status == "ACTIVE" &&
                    d.ExpirationDate != null &&
                    d.ExpirationDate <= utcNow)
                .ToListAsync(cancellationToken);
        }

        public async Task<Donation?> GetByIdForUpdateAsync(
            int donationId,
            CancellationToken cancellationToken = default)
        {
            var query = _appDb.Donations
                .FromSqlInterpolated(
                    $"SELECT * FROM `Donations` WHERE `Id` = {donationId} FOR UPDATE")
                .AsAsyncEnumerable();

            await foreach (var donation in query
                .WithCancellation(cancellationToken))
            {
                return donation;
            }

            return null;
        }

        public async Task<(List<Donation> Items, int TotalCount)>
            GetForAdminAsync(
                int? donorId,
                int? populationTypeId,
                string? status,
                DateTime? fromDate,
                DateTime? toDate,
                int page,
                int pageSize,
                CancellationToken cancellationToken = default)
        {
            var query = BuildAdminQuery();

            if (donorId.HasValue)
            {
                query = query.Where(d => d.DonorId == donorId.Value);
            }

            if (populationTypeId.HasValue)
            {
                query = query.Where(
                    d => d.PopulationTypeId == populationTypeId.Value);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(d => d.Status == status);
            }

            if (fromDate.HasValue)
            {
                var startDate = fromDate.Value.Date;

                query = query.Where(d => d.CreatedAt >= startDate);
            }

            if (toDate.HasValue)
            {
                // Include the entire final calendar day.
                var endDateExclusive = toDate.Value.Date.AddDays(1);

                query = query.Where(
                    d => d.CreatedAt < endDateExclusive);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(d => d.CreatedAt)
                .ThenByDescending(d => d.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<Donation?> GetByIdForAdminAsync(
            int donationId,
            CancellationToken cancellationToken = default)
        {
            return await BuildAdminQuery()
                .Include(d => d.Redemptions)
                    .ThenInclude(r => r.Recipient)
                        .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(
                    d => d.Id == donationId,
                    cancellationToken);
        }

        private IQueryable<Donation> BuildAdminQuery()
        {
            return _appDb.Donations
                .AsNoTracking()
                .Include(d => d.Donor)
                    .ThenInclude(donor => donor.User)
                .Include(d => d.PopulationType);
        }

        public async Task<List<int>> GetActiveDonationIdsByDonorIdAsync(
            int donorId,
            CancellationToken cancellationToken = default)
        {
            return await _appDb.Donations
                .AsNoTracking()
                .Where(d =>
                    d.DonorId == donorId &&
                    d.Status == "ACTIVE")
                .OrderBy(d => d.Id)
                .Select(d => d.Id)
                .ToListAsync(cancellationToken);
        }
    }
}