
using System.Security.Claims;
using BackendService.DTOs;
using BackendService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendService.Controllers
{
    [ApiController]
    [Route("api/donors")]
    [Authorize(Roles = "DONOR")]
    public class DonorController : ControllerBase
    {
        private readonly DonorService _donorService;

        public DonorController(DonorService donorService)
        {
            _donorService = donorService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userIdValue = User.FindFirstValue("sub");

            if (!int.TryParse(userIdValue, out int userId))
            {
                return Unauthorized();
            }

            var profile = await _donorService.GetMyProfileAsync(userId);

            return Ok(profile);
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile(
            [FromBody] UpdateDonorDto dto)
        {
            var userIdValue = User.FindFirstValue("sub");

            if (!int.TryParse(userIdValue, out int userId))
            {
                return Unauthorized();
            }

            await _donorService.UpdateMyProfileAsync(userId, dto);

            return NoContent();
        }

        [HttpDelete("me")]
        public async Task<IActionResult> DeactivateMyAccount(
            CancellationToken cancellationToken)
        {
            var userIdValue = User.FindFirstValue("sub");

            if (!int.TryParse(userIdValue, out int userId))
            {
                return Unauthorized();
            }

            await _donorService.DeactivateMyAccountAsync(
                userId,
                cancellationToken);

            return NoContent();
        }

    }
}
