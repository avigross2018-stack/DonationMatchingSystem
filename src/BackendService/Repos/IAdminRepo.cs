using BackendService.Models;

namespace BackendService.Repos
{
    public interface IAdminRepo
    {
        // Adds an Admin to the DbContext (not saved to the DB until SaveChangesAsync is called)
        Task AddAsync(Admin admin);

        // Returns the Admin profile linked to the given user id, or null if none exists
        Task<Admin?> GetByUserIdAsync(int userId);
    }
}