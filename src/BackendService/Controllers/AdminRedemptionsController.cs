using BackendService.DTOs;
using BackendService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BackendService.Exceptions;


namespace BackendService.Controllers
{
    [ApiController]
    [Route("api/admin/redemptions")]
    [Authorize(Roles = "ADMIN")]
    public sealed class AdminRedemptionsController : ControllerBase
    {
        private readonly AdminRedemptionService _adminRedemptionService;

        public AdminRedemptionsController(
            AdminRedemptionService adminRedemptionService)
        {
            _adminRedemptionService = adminRedemptionService;
        }

        [HttpGet]
        [ProducesResponseType(
            typeof(AdminRedemptionsPageDto),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<AdminRedemptionsPageDto>>
            GetRedemptions(
                [FromQuery] int? populationTypeId,
                [FromQuery] string? status,
                [FromQuery] DateTime? fromDate,
                [FromQuery] DateTime? toDate,
                [FromQuery] int page = 1,
                [FromQuery] int pageSize = 50,
                CancellationToken cancellationToken = default)
        {
            var result = await _adminRedemptionService.GetRedemptionsAsync(
                populationTypeId,
                status,
                fromDate,
                toDate,
                page,
                pageSize,
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("pending")]
        [ProducesResponseType(
            typeof(AdminRedemptionsPageDto),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<AdminRedemptionsPageDto>>
            GetPendingRedemptions(
                [FromQuery] int? populationTypeId,
                [FromQuery] DateTime? fromDate,
                [FromQuery] DateTime? toDate,
                [FromQuery] int page = 1,
                [FromQuery] int pageSize = 50,
                CancellationToken cancellationToken = default)
        {
            var result =
                await _adminRedemptionService.GetPendingRedemptionsAsync(
                    populationTypeId,
                    fromDate,
                    toDate,
                    page,
                    pageSize,
                    cancellationToken);

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(
            typeof(AdminRedemptionDto),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AdminRedemptionDto>> GetRedemptionById(
            int id,
            CancellationToken cancellationToken)
        {
            var result = await _adminRedemptionService
                .GetRedemptionByIdAsync(id, cancellationToken);

            return Ok(result);
        }


        [HttpPost("{id:int}/approve")]
        [ProducesResponseType(
            typeof(AdminRedemptionDecisionDto),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AdminRedemptionDecisionDto>> ApproveRedemption(
            int id,
            CancellationToken cancellationToken)
        {
            // The JWT subject contains User.Id, not Admin.Id.
            var userIdClaim = User.FindFirstValue(
                JwtRegisteredClaimNames.Sub);

            if (!int.TryParse(userIdClaim, out var userId) || userId <= 0)
            {
                throw new ForbiddenException(
                    "The authenticated user identifier is missing or invalid.");
            }

            var result = await _adminRedemptionService.ApproveRedemptionAsync(
                userId,
                id,
                cancellationToken);

            return Ok(result);
        }


        [HttpPost("{id:int}/reject")]
        [ProducesResponseType(
            typeof(AdminRedemptionRejectionDto),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AdminRedemptionRejectionDto>> RejectRedemption(
            int id,
            CancellationToken cancellationToken)
        {
            // The JWT subject identifies the user, not the Admin record.
            var userIdClaim = User.FindFirstValue(
                JwtRegisteredClaimNames.Sub);

            if (!int.TryParse(userIdClaim, out var userId) || userId <= 0)
            {
                throw new ForbiddenException(
                    "The authenticated user identifier is missing or invalid.");
            }

            var result = await _adminRedemptionService.RejectRedemptionAsync(
                userId,
                id,
                cancellationToken);

            return Ok(result);
        }
    }
}