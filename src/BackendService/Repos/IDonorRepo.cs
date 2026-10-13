using BackendService.Models;

namespace BackendService.Repos
{
    public interface IDonorRepo
    {
        Task AddAsync(Donor donor);

        Task<Donor?> GetByUserIdAsync(int userId);

        Task<bool> IsDonorActiveAsync(int donorId);

        Task<List<Donor>> GetAllForAdminAsync(
            CancellationToken cancellationToken = default);

        Task<Donor?> GetByIdForAdminAsync(
            int donorId,
            CancellationToken cancellationToken = default);

        Task<Donor?> GetTrackedByIdForAdminAsync(
            int donorId,
            CancellationToken cancellationToken = default);
    }
}