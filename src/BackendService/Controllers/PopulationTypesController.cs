using BackendService.DTOs;
using BackendService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendService.Controllers
{
    [ApiController]
    [Route("api/population-types")]
    public class PopulationTypesController : ControllerBase
    {
        private readonly PopulationTypeService _populationTypeService;

        public PopulationTypesController(
            PopulationTypeService populationTypeService)
        {
            _populationTypeService = populationTypeService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<List<PopulationTypeResponseDto>>>
            GetActivePopulationTypes(
                CancellationToken cancellationToken)
        {
            var populationTypes =
                await _populationTypeService.GetActiveAsync(
                    cancellationToken);

            return Ok(populationTypes);
        }
    }
}