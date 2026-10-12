using BackendService.Data;
using BackendService.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendService.Repos
{
    public class AdminRepo : IAdminRepo
    {
        private readonly AppDbContext _appDb;

        public AdminRepo(AppDbContext appDb)
        {
            _appDb = appDb;
        }

        public async Task AddAsync(Admin admin)
        {
            // Only tracks the entity; the INSERT happens on SaveChangesAsync
            await _appDb.Admins.AddAsync(admin);
        }

        public async Task<Admin?> GetByUserIdAsync(int userId)
        {
            // Returns null when the user has no Admin profile
            return await _appDb.Admins
                .FirstOrDefaultAsync(a => a.UserId == userId);
        }
    }
}