
using BackendService.DTOs;
using BackendService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendService.Controllers
{
    [ApiController]
    [Route("api/admin/population-types")]
    [Authorize(Roles = "ADMIN")]
    public sealed class AdminPopulationTypesController : ControllerBase
    {
        private readonly AdminPopulationTypeService _adminPopulationTypeService;

        public AdminPopulationTypesController(
            AdminPopulationTypeService adminPopulationTypeService)
        {
            _adminPopulationTypeService = adminPopulationTypeService;
        }

        [HttpGet]
        [ProducesResponseType(
            typeof(IReadOnlyList<AdminPopulationTypeDto>),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IReadOnlyList<AdminPopulationTypeDto>>>
            GetAll(CancellationToken cancellationToken)
        {
            var populationTypes =
                await _adminPopulationTypeService.GetAllAsync(
                    cancellationToken);

            return Ok(populationTypes);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(
            typeof(AdminPopulationTypeDto),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AdminPopulationTypeDto>> GetById(
            int id,
            CancellationToken cancellationToken)
        {
            var populationType =
                await _adminPopulationTypeService.GetByIdAsync(
                    id,
                    cancellationToken);

            return Ok(populationType);
        }

        [HttpPost]
        [ProducesResponseType(
            typeof(AdminPopulationTypeDto),
            StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AdminPopulationTypeDto>> Create(
            [FromBody] CreatePopulationTypeDto request,
            CancellationToken cancellationToken)
        {
            var populationType =
                await _adminPopulationTypeService.CreateAsync(
                    request,
                    cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = populationType.Id },
                populationType);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(
            typeof(AdminPopulationTypeDto),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AdminPopulationTypeDto>> Update(
            int id,
            [FromBody] UpdatePopulationTypeDto request,
            CancellationToken cancellationToken)
        {
            var populationType =
                await _adminPopulationTypeService.UpdateAsync(
                    id,
                    request,
                    cancellationToken);

            return Ok(populationType);
        }

        [HttpPatch("{id:int}/status")]
        [ProducesResponseType(
            typeof(AdminPopulationTypeDto),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AdminPopulationTypeDto>> UpdateStatus(
            int id,
            [FromBody] UpdatePopulationTypeStatusDto request,
            CancellationToken cancellationToken)
        {
            var populationType =
                await _adminPopulationTypeService.UpdateStatusAsync(
                    id,
                    request,
                    cancellationToken);

            return Ok(populationType);
        }
    }
}
