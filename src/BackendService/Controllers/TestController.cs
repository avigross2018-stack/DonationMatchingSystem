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
        
        // Accessible to users with either role (comma = OR)
        [Authorize(Roles = "DONOR,ADMIN")]
        [HttpGet("donor-or-admin")]
        public IActionResult DonorOrAdmin()
        {
            return Ok(new
            {
                Message = "Welcome Donor or Admin!",
                UserId = User.FindFirstValue("sub"),
                Role = User.FindFirstValue(ClaimTypes.Role)
            });
        }
    }
}