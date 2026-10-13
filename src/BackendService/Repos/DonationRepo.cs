using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<List<Donation>> GetAvailableDonationsAsync(DateTime utcNow, CancellationToken cancellationToken = default)
        {
            return await _appDb.Donations
                .AsNoTracking()
                .Where(d =>
                    d.Status == "ACTIVE" &&
                    d.ExpirationDate > utcNow &&
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

        public async Task<Donation?> GetByIdAsync(int id)
        {
            return await _appDb.Donations
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<List<Donation>> GetExpiredActiveDonationsAsync(DateTime utcNow, CancellationToken cancellationToken = default)
        {
            return await _appDb.Donations
                .Where(d => d.Status == "ACTIVE" && d.ExpirationDate <= utcNow)
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

            await foreach (var donation in query.WithCancellation(cancellationToken))
            {
                return donation;
            }

            return null;
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