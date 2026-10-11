using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackendService.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        [Authorize(Roles = "DONOR")]
        [HttpGet("donor")]
        public IActionResult DonorOnly()
        {
            return Ok(new
            {
                Message = "Welcome Donor!",
                UserId = User.FindFirstValue("sub"),
                Role = User.FindFirstValue(ClaimTypes.Role)
            });
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet("admin")]
        public IActionResult AdminOnly()
        {
            return Ok("Welcome Admin!");
        }
    }
}