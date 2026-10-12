using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    }
}