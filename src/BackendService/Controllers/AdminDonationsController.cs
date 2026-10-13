using BackendService.DTOs;
using BackendService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendService.Controllers
{
    [ApiController]
    [Route("api/admin/donations")]
    [Authorize(Roles = "ADMIN")]
    public sealed class AdminDonationsController : ControllerBase
    {
        private readonly AdminDonationService _adminDonationService;

        public AdminDonationsController(
            AdminDonationService adminDonationService)
        {
            _adminDonationService = adminDonationService;
        }

        [HttpGet]
        [ProducesResponseType(
            typeof(AdminDonationsPageDto),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<AdminDonationsPageDto>> GetAll(
            [FromQuery] int? donorId,
            [FromQuery] int? populationTypeId,
            [FromQuery] string? status,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            CancellationToken cancellationToken = default)
        {
            var result = await _adminDonationService.GetAllAsync(
                donorId,
                populationTypeId,
                status,
                fromDate,
                toDate,
                page,
                pageSize,
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(
            typeof(AdminDonationDetailsDto),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AdminDonationDetailsDto>> GetById(
            int id,
            CancellationToken cancellationToken)
        {
            var result = await _adminDonationService.GetByIdAsync(
                id,
                cancellationToken);

            return Ok(result);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Update(
            [FromRoute] int id,
            [FromBody] UpdateAdminDonationDto request,
            CancellationToken cancellationToken)
        {
            await _adminDonationService.UpdateAsync(
                id,
                request,
                cancellationToken);

            return NoContent();
        }

        [HttpPost("{id:int}/cancel")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Cancel(
            [FromRoute] int id,
            CancellationToken cancellationToken)
        {
            await _adminDonationService.CancelAsync(
                id,
                cancellationToken);

            return NoContent();
        }
    }
}