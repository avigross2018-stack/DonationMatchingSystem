using System.Security.Claims;
using BackendService.DTOs;
using BackendService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendService.Controllers
{
    [ApiController]
    [Route("api/donations")]
    [Authorize(Roles = "DONOR")]
    public class DonationsController : ControllerBase
    {
        private readonly DonationService _donationService;

        public DonationsController(DonationService donationService)
        {
            _donationService = donationService;
        }


        [HttpPost]
        public async Task<IActionResult> CreateDonation(
            [FromBody] CreateDonationDto dto)
        {
            var userIdValue = User.FindFirstValue("sub");

            // Return 401 if the authenticated user's ID claim is missing or invalid.
            if (!int.TryParse(userIdValue, out int userId))
            {
                return Unauthorized();
            }

            // Business exceptions are handled by the global exception handler.
            int donationId = await _donationService
                .CreateDonationAsync(userId, dto);

            return StatusCode(StatusCodes.Status201Created, new
            {
                Message = "Donation created successfully",
                DonationId = donationId
            });
        }
        [HttpGet("show-donations")]
        public async Task<IActionResult> GetMyDonations()
        {
            var userIdValue = User.FindFirstValue("sub");

            if (!int.TryParse(userIdValue, out int userId))
            {
                return Unauthorized();
            }

            var donations = await _donationService.GetMyDonationsAsync(userId);

            return Ok(donations);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateDonation(
            int id,
            [FromBody] UpdateDonationDto dto)
        {
            var userIdValue = User.FindFirstValue("sub");

            if (!int.TryParse(userIdValue, out int userId))
            {
                return Unauthorized();
            }

            try
            {
                await _donationService.UpdateDonationAsync(userId, id, dto);

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Message = ex.Message });
            }
        }

        [HttpPost("{id:int}/cancel")]
        public async Task<IActionResult> CancelDonation(int id)
        {
            var userIdValue = User.FindFirstValue("sub");

            if (!int.TryParse(userIdValue, out int userId))
            {
                return Unauthorized();
            }

            try
            {
                await _donationService.CancelDonationAsync(userId, id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Message = ex.Message });
            }
        }
    }
}