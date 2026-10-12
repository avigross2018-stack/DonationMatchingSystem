using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendService.Data;
using BackendService.DTOs;
using BackendService.Exceptions;
using BackendService.Models;
using BackendService.Repos;
using Microsoft.AspNetCore.Identity;

namespace BackendService.Services
{
    public class AuthService
    {
        private readonly IUserRepo _userRepo;
        private readonly IDonorRepo _donorRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly JwtTokenService _jwtTokenService;
        public AuthService(
            IUserRepo userRepo,
            IDonorRepo donorRepo,
            IUnitOfWork unitOfWork,
            IPasswordHasher<User> passwordHasher,
            JwtTokenService jwtTokenService
        )
        {
            _userRepo = userRepo;
            _donorRepo = donorRepo;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<int> RegisterDonorAsync(RegisterDonorDto dto)
        {
            var existingUser = await _userRepo.GetByPhoneAsync(dto.Phone);

            if (existingUser != null)
            {
                throw new ConflictException(
                    "A user with this phone number already exists."
                );
            }

            var user = new User
            {
                Name = dto.Name,
                Phone = dto.Phone,
                UserType = "DONOR",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash = _passwordHasher.HashPassword(
                user,
                dto.Password
            );

            var donor = new Donor
            {
                User = user
            };

            await _userRepo.AddAsync(user);
            await _donorRepo.AddAsync(donor);

            await _unitOfWork.SaveChangesAsync();

            return user.Id;
        }

        public async Task<string?> LoginAsync(LoginDto dto)
        {
            var user = await _userRepo.GetByPhoneAsync(dto.Phone);

            if (user == null || !user.IsActive)
            {
                return null;
            }

            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                dto.Password
            );

            if(result == PasswordVerificationResult.Failed)
            {
                return null;
            }

            return _jwtTokenService.GenerateToken(user);
        }
    }
}