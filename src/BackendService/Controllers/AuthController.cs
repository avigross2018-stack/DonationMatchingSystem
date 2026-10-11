using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendService.DTOs;
using BackendService.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackendService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register/donor")]
        public async Task<IActionResult> RegisterDonor(
            [FromBody] RegisterDonorDto dto
        )
        {
            try
            {
                int userId = await _authService.RegisterDonorAsync(dto);

                return StatusCode(StatusCodes.Status201Created, new
                {
                    Message = "Donor registered successfully",
                    UserId = userId
                });
            }
            catch(InvalidOperationException ex)
            {
                return Conflict(new
                {
                    Message = ex.Message
                });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = await _authService.LoginAsync(dto);

            if (token == null)
            {
                return Unauthorized(new
                {
                    Message = "Invalid phone or password"
                });
            }

            return Ok(new
            {
                AccessToken = token,
                TokenType = "Bearer"
            });
        }
    }
}