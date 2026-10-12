using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BackendService.Data;
using BackendService.DTOs;
using BackendService.Exceptions;
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
        private readonly IRedemptionRepo _redemptionRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly MatchingEventQueue _matchingQueue;

        public DonationService(
            IDonationRepo donationRepo,
            IDonorRepo donorRepo,
            IPopulationTypeRepo populationTypeRepo,
            IUserRepo userRepo,
            IRedemptionRepo redemptionRepo,
            IUnitOfWork unitOfWork,
            MatchingEventQueue matchingQueue)
        {
            _donationRepo = donationRepo;
            _donorRepo = donorRepo;
            _populationTypeRepo = populationTypeRepo;
            _userRepo = userRepo;
            _redemptionRepo = redemptionRepo;
            _unitOfWork = unitOfWork;
            _matchingQueue = matchingQueue;
        }

        public async Task<int> CreateDonationAsync(
            int userId,
            CreateDonationDto dto
        )
        {
            var user = await _userRepo.GetByIdAsync(userId);

            if (user == null || !user.IsActive || user.UserType != "DONOR")
            {
                throw new ForbiddenException(
                    "User is not an active donor.");
            }

            var donor = await _donorRepo.GetByUserIdAsync(userId);

            if (donor == null)
            {
                throw new ResourceNotFoundException(
                    "Donor profile not found.");
            }

            var populationType = await _populationTypeRepo
                .GetByIdAsync(dto.PopulationTypeId);

            if (populationType == null || !populationType.IsActive)
            {
                throw new BusinessValidationException(
                    "Population type is invalid or inactive.");
            }

            // Validate business rules
            if (dto.TotalAmount <= 0 ||
                dto.MaxAmountPerRecipient <= 0)
            {
                throw new BusinessValidationException(
                    "Donation amounts must be positive.");
            }

            var now = DateTime.UtcNow;

            if (dto.ExpirationDate.ToUniversalTime() <= now)
            {
                throw new BusinessValidationException(
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
            
            // Notify matching worker about the new donation
            await _matchingQueue.EnqueueAsync(
                new MatchingEvent(
                    MatchingEventType.DonationCreated,
                    donation.Id));

            return donation.Id;
        }

        public async Task<List<DonationResponseDto>> GetMyDonationsAsync(int userId)
        {
            var donor = await _donorRepo.GetByUserIdAsync(userId);

            if (donor == null)
            {
                throw new ResourceNotFoundException(
                    "Donor profile not found.");
            }

            var donations = await _donationRepo.GetByDonorIdAsync(donor.Id);

            return donations.Select(d => new DonationResponseDto
            {
                Id = d.Id,
                PopulationTypeId = d.PopulationTypeId,
                TotalAmount = d.TotalAmount,
                RemainingAmount = d.RemainingAmount,
                MaxAmountPerRecipient = d.MaxAmountPerRecipient,
                Description = d.Description,
                CreatedAt = d.CreatedAt,
                ExpirationDate = d.ExpirationDate,
                Status = d.Status
            }).ToList();
        }

        public async Task UpdateDonationAsync(
            int userId,
            int donationId,
            UpdateDonationDto dto)
        {
            // Find the authenticated donor
            var donor = await _donorRepo.GetByUserIdAsync(userId);

            if (donor == null || !donor.User.IsActive)
            {
                throw new ResourceNotFoundException(
                    "Donor profile not found.");
            }

            var utcNow = DateTime.UtcNow;
            var expirationDate = dto.ExpirationDate.ToUniversalTime();

            if (expirationDate <= utcNow)
            {
                throw new ArgumentException(
                    "Expiration date must be in the future.");
            }

            // Begin transaction
            await using var transaction =
                await _unitOfWork.BeginTransactionAsync(
                    IsolationLevel.ReadCommitted);

            // Lock the donation row
            var donation = await _donationRepo
                .GetByIdForUpdateAsync(donationId);

            if (donation == null)
            {
                throw new ResourceNotFoundException(
                    "Donation not found.");
            }

            // Verify ownership
            if (donation.DonorId != donor.Id)
            {
                throw new ForbiddenException(
                    "You cannot edit this donation.");
            }

            // Validate current donation status
            if (donation.Status != "ACTIVE" ||
                donation.ExpirationDate <= DateTime.UtcNow)
            {
                throw new ConflictException(
                    "Donation is not active or has expired.");
            }

            // Validate input
            if (dto.TotalAmount <= 0 ||
                dto.MaxAmountPerRecipient <= 0)
            {
                throw new BusinessValidationException(
                    "Amounts must be positive.");
            }

            if (dto.ExpirationDate.ToUniversalTime() <= DateTime.UtcNow)
            {
                throw new BusinessValidationException(
                    "Expiration date must be in the future.");
            }

            // Calculate the amount already redeemed
            decimal redeemedAmount =
                donation.TotalAmount - donation.RemainingAmount;

            if (dto.TotalAmount < redeemedAmount)
            {
                throw new ConflictException(
                    "Total amount cannot be lower than already redeemed amount.");
            }

            // Update donation
            donation.TotalAmount = dto.TotalAmount;
            donation.RemainingAmount =
                dto.TotalAmount - redeemedAmount;

            donation.MaxAmountPerRecipient =
                dto.MaxAmountPerRecipient;

            donation.Description = dto.Description;
            donation.ExpirationDate = expirationDate;

            await _unitOfWork.SaveChangesAsync();

            await transaction.CommitAsync();
        }

        public async Task CancelDonationAsync(int userId, int donationId)
        {
            var donor = await _donorRepo.GetByUserIdAsync(userId);

            if (donor == null || !donor.User.IsActive)
            {
                throw new UnauthorizedAccessException(
                    "Donor profile not found or inactive.");
            }

            await using var transaction =
                await _unitOfWork.BeginTransactionAsync(
                    IsolationLevel.ReadCommitted);

            // Lock the donation before checking its state
            var donation = await _donationRepo
                .GetByIdForUpdateAsync(donationId);

            if (donation == null)
            {
                throw new KeyNotFoundException(
                    "Donation not found.");
            }

            if (donation.DonorId != donor.Id)
            {
                throw new UnauthorizedAccessException(
                    "You cannot cancel this donation.");
            }

            if (donation.Status != "ACTIVE" ||
                donation.ExpirationDate <= DateTime.UtcNow)
            {
                throw new InvalidOperationException(
                    "Donation is not active or has expired.");
            }

            // Read pending matches after acquiring the lock
            var pendingRedemptions = await _redemptionRepo
                .GetPendingByDonationIdAsync(donationId);

            donation.Status = "CANCELLED";

            foreach (var redemption in pendingRedemptions)
            {
                redemption.Status = "CANCELLED";
            }

            await _unitOfWork.SaveChangesAsync();

            await transaction.CommitAsync();
        }

    }
}