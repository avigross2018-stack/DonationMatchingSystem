using BackendService.Models;

namespace BackendService.Repos
{
    public interface IAdminRepo
    {
        // Tracks an admin for insertion; saving is handled by UnitOfWork.
        Task AddAsync(Admin admin);

        // Returns the admin profile and its linked user.
        Task<Admin?> GetByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default);
    }
}