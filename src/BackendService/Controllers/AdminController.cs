
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BackendService.DTOs;
using BackendService.Exceptions;
using BackendService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendService.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "ADMIN")]
    public sealed class AdminController : ControllerBase
    {
        private readonly AdminService _adminService;

        public AdminController(AdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("me")]
        [ProducesResponseType(typeof(AdminProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<AdminProfileDto>> GetCurrentAdmin(
            CancellationToken cancellationToken)
        {
            var admin = await _adminService.GetCurrentAdminAsync(
                GetAuthenticatedUserId(),
                cancellationToken);

            return Ok(admin);
        }

        [HttpGet("donors")]
        [ProducesResponseType(
            typeof(IReadOnlyList<AdminDonorDto>),
            StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<AdminDonorDto>>> GetDonors(
            CancellationToken cancellationToken)
        {
            var donors = await _adminService.GetDonorsAsync(
                cancellationToken);

            return Ok(donors);
        }

        [HttpGet("donors/{id:int}")]
        [ProducesResponseType(
            typeof(AdminDonorDetailsDto),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AdminDonorDetailsDto>> GetDonorById(
            int id,
            CancellationToken cancellationToken)
        {
            var donor = await _adminService.GetDonorByIdAsync(
                id,
                cancellationToken);

            return Ok(donor);
        }

        [HttpPatch("donors/{id:int}/status")]
        [ProducesResponseType(
            typeof(AdminDonorStatusDto),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AdminDonorStatusDto>> UpdateDonorStatus(
            int id,
            [FromBody] UpdateDonorStatusDto request,
            CancellationToken cancellationToken)
        {
            var result = await _adminService.UpdateDonorStatusAsync(
                GetAuthenticatedUserId(),
                id,
                request,
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("recipients")]
        [ProducesResponseType(
            typeof(IReadOnlyList<AdminRecipientDto>),
            StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<AdminRecipientDto>>>
            GetRecipients(CancellationToken cancellationToken)
        {
            var recipients = await _adminService.GetRecipientsAsync(
                cancellationToken);

            return Ok(recipients);
        }

        [HttpGet("recipients/{id:int}")]
        [ProducesResponseType(
            typeof(AdminRecipientDetailsDto),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AdminRecipientDetailsDto>>
            GetRecipientById(
                int id,
                CancellationToken cancellationToken)
        {
            var recipient = await _adminService.GetRecipientByIdAsync(
                id,
                cancellationToken);

            return Ok(recipient);
        }

        [HttpPatch("recipients/{id:int}/status")]
        [ProducesResponseType(
            typeof(AdminRecipientStatusDto),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AdminRecipientStatusDto>>
            UpdateRecipientStatus(
                int id,
                [FromBody] UpdateRecipientStatusDto request,
                CancellationToken cancellationToken)
        {
            var result = await _adminService.UpdateRecipientStatusAsync(
                GetAuthenticatedUserId(),
                id,
                request,
                cancellationToken);

            return Ok(result);
        }

        private int GetAuthenticatedUserId()
        {
            // The JWT subject identifies User.Id, not Admin.Id.
            var userIdClaim = User.FindFirstValue(
                JwtRegisteredClaimNames.Sub);

            if (!int.TryParse(userIdClaim, out var userId) || userId <= 0)
            {
                throw new ForbiddenException(
                    "The authenticated user identifier is missing or invalid.");
            }

            return userId;
        }
    }
}
