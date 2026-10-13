using BackendService.Models;

namespace BackendService.Repos
{
    public interface IPopulationTypeRepo
    {
        Task<PopulationType?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<PopulationType?> GetTrackedByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<List<PopulationType>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task<bool> ExistsByNameAsync(
            string name,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsByNameAsync(
            string name,
            int excludeId,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            PopulationType populationType,
            CancellationToken cancellationToken = default);
    }
}