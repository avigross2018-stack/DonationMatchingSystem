using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BackendService.DTOs;
using BackendService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendService.Controllers
{
    [ApiController]
    [Route("api/recipients")]
    [Authorize(Roles = "RECIPIENT")]
    public class RecipientsController : ControllerBase
    {
        private readonly RecipientService _recipientService;

        public RecipientsController(RecipientService recipientService)
        {
            _recipientService = recipientService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userIdValue = User.FindFirstValue("sub");

            if (!int.TryParse(userIdValue, out int userId))
            {
                return Unauthorized();
            }

            var profile = await _recipientService
                .GetMyProfileAsync(userId);

            return Ok(profile);
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile(
            [FromBody] UpdateRecipientDto dto,
            CancellationToken cancellationToken)
        {
            var userIdValue = User.FindFirstValue("sub");

            if (!int.TryParse(userIdValue, out int userId))
            {
                return Unauthorized();
            }

            await _recipientService.UpdateMyProfileAsync(
                userId,
                dto,
                cancellationToken);

            return NoContent();
        }


        [HttpGet("me/redemptions")]
        public async Task<IActionResult> GetMyRedemptions(
            CancellationToken cancellationToken)
        {
            var userIdValue = User.FindFirstValue("sub");

            if (!int.TryParse(userIdValue, out int userId))
            {
                return Unauthorized();
            }

            var redemptions = await _recipientService
                .GetMyRedemptionsAsync(userId, cancellationToken);

            return Ok(redemptions);
        }

    }
}