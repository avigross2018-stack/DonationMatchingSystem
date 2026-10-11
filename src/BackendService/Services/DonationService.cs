using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendService.Data;
using BackendService.DTOs;
using BackendService.Models;
using BackendService.Repos;

namespace BackendService.Services
{
    public class DonationService
    {
        private readonly IDonationRepo _donationRepo;
        private readonly IDonorRepo _donorRepo;
        private readonly IPopulationTypeRepo _populationTypeRepo;
        private readonly IUserRepo _userRepo;
        private readonly IUnitOfWork _unitOfWork;

        public DonationService(
            IDonationRepo donationRepo,
            IDonorRepo donorRepo,
            IPopulationTypeRepo populationTypeRepo,
            IUserRepo userRepo,
            IUnitOfWork unitOfWork)
        {
            _donationRepo = donationRepo;
            _donorRepo = donorRepo;
            _populationTypeRepo = populationTypeRepo;
            _userRepo = userRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> CreateDonationAsync(
            int userId,
            CreateDonationDto dto
        )
        {
            var user = await _userRepo.GetByIdAsync(userId);

            if (user == null || !user.IsActive || user.UserType != "DONOR")
            {
                throw new UnauthorizedAccessException(
                    "User is not an active donor.");
            }

            var donor = await _donorRepo.GetByUserIdAsync(userId);

            if (donor == null)
            {
                throw new InvalidOperationException(
                    "Donor profile not found.");
            }

            var populationType = await _populationTypeRepo
                .GetByIdAsync(dto.PopulationTypeId);

            if (populationType == null || !populationType.IsActive)
            {
                throw new ArgumentException(
                    "Population type is invalid or inactive.");
            }

            // Validate business rules
            if (dto.TotalAmount <= 0 ||
                dto.MaxAmountPerRecipient <= 0)
            {
                throw new ArgumentException(
                    "Donation amounts must be positive.");
            }

            var now = DateTime.UtcNow;

            if (dto.ExpirationDate.ToUniversalTime() <= now)
            {
                throw new ArgumentException(
                    "Expiration date must be in the future.");
            }

            // Create Donation
            var donation = new Donation
            {
                DonorId = donor.Id,
                PopulationTypeId = dto.PopulationTypeId,
                TotalAmount = dto.TotalAmount,
                RemainingAmount = dto.TotalAmount,
                MaxAmountPerRecipient = dto.MaxAmountPerRecipient,
                Description = dto.Description,
                CreatedAt = now,
                ExpirationDate = dto.ExpirationDate.ToUniversalTime(),
                Status = "ACTIVE"
            };

            await _donationRepo.AddAsync(donation);

            await _unitOfWork.SaveChangesAsync();

            return donation.Id;
        }
    }
}