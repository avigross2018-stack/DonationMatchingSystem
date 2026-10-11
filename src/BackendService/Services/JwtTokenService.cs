using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BackendService.Models;
using Microsoft.IdentityModel.Tokens;

namespace BackendService.Services
{
    public class JwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(User user)
        {
            var key = _configuration["JWT:KEY"]
                ?? throw new InvalidOperationException("JWT key is missing");

            var issuer = _configuration["JWT:ISSUER"]
                ?? throw new InvalidOperationException("JWT issuer is missing");

            var audience = _configuration["JWT:AUDIENCE"]
                ?? throw new InvalidOperationException("JWT audience is missing");

            var expirationMinutes = _configuration.GetValue<int>(
                "JWT:EXPIRATION_MINUTES");

            if (expirationMinutes <= 0)
            {
                throw new InvalidOperationException(
                    "JWT expiration must be positive");
            }

            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key)
            );

            var credentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256
            );

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.UserType),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}