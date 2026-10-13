using BackendService.DTOs;
using BackendService.Repos;

namespace BackendService.Services
{
    public class PopulationTypeService
    {
        private readonly IPopulationTypeRepo _populationTypeRepo;

        public PopulationTypeService(
            IPopulationTypeRepo populationTypeRepo)
        {
            _populationTypeRepo = populationTypeRepo;
        }

        public async Task<List<PopulationTypeResponseDto>>
            GetActiveAsync(
                CancellationToken cancellationToken = default)
        {
            var populationTypes =
                await _populationTypeRepo.GetActiveAsync(
                    cancellationToken);

            return populationTypes
                .Select(p => new PopulationTypeResponseDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description
                })
                .ToList();
        }
    }
}