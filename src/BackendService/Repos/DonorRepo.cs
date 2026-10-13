
using BackendService.Data;
using BackendService.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendService.Repos
{
    public class DonorRepo : IDonorRepo
    {
        private readonly AppDbContext _appDb;

        public DonorRepo(AppDbContext appDb)
        {
            _appDb = appDb;
        }

        public async Task AddAsync(Donor donor)
        {
            await _appDb.Donors.AddAsync(donor);
        }

        public async Task<Donor?> GetByUserIdAsync(int userId)
        {
            return await _appDb.Donors
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.UserId == userId);
        }

        public async Task<bool> IsDonorActiveAsync(int donorId)
        {
            return await _appDb.Donors
                .AnyAsync(d =>
                    d.Id == donorId &&
                    d.User.IsActive);
        }

        public async Task<List<Donor>> GetAllForAdminAsync(
            CancellationToken cancellationToken = default)
        {
            // Load donor accounts without tracking changes.
            return await _appDb.Donors
                .AsNoTracking()
                .Include(d => d.User)
                .OrderBy(d => d.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<Donor?> GetByIdForAdminAsync(
            int donorId,
            CancellationToken cancellationToken = default)
        {
            // Load donor details and donation history.
            return await _appDb.Donors
                .AsNoTracking()
                .Include(d => d.User)
                .Include(d => d.Donations)
                    .ThenInclude(donation => donation.PopulationType)
                .FirstOrDefaultAsync(
                    d => d.Id == donorId,
                    cancellationToken);
        }

        public async Task<Donor?> GetTrackedByIdForAdminAsync(
            int donorId,
            CancellationToken cancellationToken = default)
        {
            // Keep the donor and user tracked for status updates.
            return await _appDb.Donors
                .Include(d => d.User)
                .FirstOrDefaultAsync(
                    d => d.Id == donorId,
                    cancellationToken);
        }
    }
}
    