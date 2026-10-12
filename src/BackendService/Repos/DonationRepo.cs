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
            return await _appDb.Donations
                .FromSqlInterpolated(
                    $"SELECT * FROM Donations WHERE Id = {donationId} FOR UPDATE")
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}