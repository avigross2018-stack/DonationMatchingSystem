
using BackendService.Data;
using BackendService.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendService.Repos
{
    public class PopulationTypeRepo : IPopulationTypeRepo
    {
        private readonly AppDbContext _appDb;

        public PopulationTypeRepo(AppDbContext appDb)
        {
            _appDb = appDb;
        }

        public async Task<PopulationType?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await _appDb.PopulationTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    p => p.Id == id,
                    cancellationToken);
        }

        public async Task<PopulationType?> GetTrackedByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            // Keep the entity tracked so EF Core can persist updates.
            return await _appDb.PopulationTypes
                .FirstOrDefaultAsync(
                    p => p.Id == id,
                    cancellationToken);
        }

        public async Task<List<PopulationType>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            // Include inactive population types for administration.
            return await _appDb.PopulationTypes
                .AsNoTracking()
                .OrderBy(p => p.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsByNameAsync(
            string name,
            CancellationToken cancellationToken = default)
        {
            return await _appDb.PopulationTypes
                .AnyAsync(
                    p => p.Name == name,
                    cancellationToken);
        }

        public async Task<bool> ExistsByNameAsync(
            string name,
            int excludeId,
            CancellationToken cancellationToken = default)
        {
            return await _appDb.PopulationTypes
                .AnyAsync(
                    p => p.Name == name && p.Id != excludeId,
                    cancellationToken);
        }

        public async Task AddAsync(
            PopulationType populationType,
            CancellationToken cancellationToken = default)
        {
            await _appDb.PopulationTypes.AddAsync(
                populationType,
                cancellationToken);
        }
        public async Task<List<PopulationType>> GetActiveAsync(
            CancellationToken cancellationToken = default)
        {
            return await _appDb.PopulationTypes
                .AsNoTracking()
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync(cancellationToken);
        }
    }
}
