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
    }
}