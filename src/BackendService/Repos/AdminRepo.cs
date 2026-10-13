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
            // Saving is handled by UnitOfWork.
            await _appDb.Admins.AddAsync(admin);
        }

        public async Task<Admin?> GetByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default)
        {
            // Load the admin and its linked user in one database query.
            return await _appDb.Admins
                .AsNoTracking()
                .Include(admin => admin.User)
                .FirstOrDefaultAsync(
                    admin => admin.UserId == userId,
                    cancellationToken);
        }
    }
}