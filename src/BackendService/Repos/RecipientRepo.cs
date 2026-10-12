using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendService.Data;
using BackendService.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendService.Repos
{
    public class RecipientRepo : IRecipientRepo
    {
        private readonly AppDbContext _appDb;

        public RecipientRepo(AppDbContext appDb)
        {
            _appDb = appDb;
        }

        public async Task AddAsync(Recipient recipient)
        {
            await _appDb.Recipients.AddAsync(recipient);
        }

        public async Task<Recipient?> GetByIdAsync(int id)
        {
            return await _appDb.Recipients
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Recipient?> GetByUserIdAsync(int userId)
        {
            return await _appDb.Recipients
                .FirstOrDefaultAsync(r => r.UserId == userId);
        }

        public async Task<List<Recipient>> GetEligibleRecipientsAsync(CancellationToken cancellationToken = default)
        {
            return await _appDb.Recipients
                .AsNoTracking()
                .Where(r =>
                    r.User.IsActive &&
                    r.RemainingEligibility > 0)
                .ToListAsync(cancellationToken);
        }
    }
}