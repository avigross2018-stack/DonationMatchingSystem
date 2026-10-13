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
        [ProducesResponseType(
            typeof(AdminProfileDto),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<AdminProfileDto>> GetCurrentAdmin(
            CancellationToken cancellationToken)
        {
            // The JWT contains User.Id, not Admin.Id.
            var userIdClaim = User.FindFirstValue(
                JwtRegisteredClaimNames.Sub);

            if (!int.TryParse(userIdClaim, out var userId) ||
                userId <= 0)
            {
                throw new ForbiddenException(
                    "The authenticated user identifier is missing or invalid.");
            }

            var admin = await _adminService.GetCurrentAdminAsync(
                userId,
                cancellationToken);

            return Ok(admin);
        }
    }
}