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
    public class RecipientService
    {
        private readonly IRecipientRepo _recipientRepo;
        private readonly IRedemptionRepo _redemptionRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly EligibilityService _eligibilityService;
        private readonly MatchingEventQueue _matchingQueue;
        private readonly IPopulationTypeRepo _populationTypeRepo;
        private readonly IUserRepo _userRepo;

        public RecipientService(
            IRecipientRepo recipientRepo,
            IRedemptionRepo redemptionRepo,
            IUnitOfWork unitOfWork,
            EligibilityService eligibilityService,
            MatchingEventQueue matchingQueue,
            IPopulationTypeRepo populationTypeRepo,
            IUserRepo userRepo
            )
        {
            _recipientRepo = recipientRepo;
            _redemptionRepo = redemptionRepo;
            _unitOfWork = unitOfWork;
            _eligibilityService = eligibilityService;
            _matchingQueue = matchingQueue;
            _populationTypeRepo = populationTypeRepo;
            _userRepo = userRepo;
        }

        public async Task<RecipientProfileDto> GetMyProfileAsync(
            int userId)
        {
            var recipient = await _recipientRepo
                .GetByUserIdAsync(userId);

            if (recipient == null)
            {
                throw new ResourceNotFoundException(
                    "Recipient profile not found.");
            }

            return new RecipientProfileDto
            {
                Id = recipient.Id,
                Name = recipient.User.Name,
                Phone = recipient.User.Phone,

                PopulationTypeId = recipient.PopulationTypeId,
                PopulationTypeName = recipient.PopulationType.Name,

                IncomeAmount = recipient.IncomeAmount,
                AmountOfChildren = recipient.AmountOfChildren,
                RequestReason = recipient.RequestReason,

                EligibilityAmount = recipient.EligibilityAmount,
                RemainingEligibility = recipient.RemainingEligibility
            };
        }


        public async Task UpdateMyProfileAsync(
            int userId,
            UpdateRecipientDto dto,
            CancellationToken cancellationToken = default)
        {
            if (dto.IncomeAmount < 0 ||
                dto.IncomeAmount > 99999999.99m ||
                decimal.Round(dto.IncomeAmount, 2) != dto.IncomeAmount ||
                dto.AmountOfChildren < 0)
            {
                throw new BusinessValidationException(
                    "Invalid income or children count.");
            }

            bool shouldRematch = false;
            int recipientId;

            await using (var transaction =
                await _unitOfWork.BeginTransactionAsync(
                    IsolationLevel.ReadCommitted,
                    cancellationToken))
            {
                var recipient = await _recipientRepo
                    .GetByUserIdForUpdateAsync(userId, cancellationToken);

                if (recipient == null)
                {
                    throw new ResourceNotFoundException(
                        "Recipient profile not found.");
                }

                if (!await _recipientRepo.IsRecipientActiveAsync(recipient.Id))
                {
                    throw new ForbiddenException(
                        "Recipient account is inactive.");
                }

                var populationType = await _populationTypeRepo
                    .GetByIdAsync(recipient.PopulationTypeId);

                if (populationType == null)
                {
                    throw new ResourceNotFoundException(
                        "Population type not found.");
                }

                decimal newEligibility =
                    _eligibilityService.CalculateEligibility(
                        populationType.BaseEligibilityAmount,
                        dto.IncomeAmount,
                        dto.AmountOfChildren);

                decimal completedTotal = await _redemptionRepo
                    .GetTotalCompletedByRecipientAsync(
                        recipient.Id,
                        cancellationToken);

                decimal newRemainingEligibility =
                    Math.Max(0m, newEligibility - completedTotal);

                shouldRematch =
                    newRemainingEligibility > recipient.RemainingEligibility;

                recipient.IncomeAmount = dto.IncomeAmount;
                recipient.AmountOfChildren = dto.AmountOfChildren;
                recipient.RequestReason = dto.RequestReason;
                recipient.EligibilityAmount = newEligibility;
                recipient.RemainingEligibility = newRemainingEligibility;

                recipientId = recipient.Id;

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            }

            if (shouldRematch)
            {
                await _matchingQueue.EnqueueAsync(
                    new MatchingEvent(
                        MatchingEventType.RecipientRegistered,
                        recipientId),
                    cancellationToken);
            }
        }

        public async Task<List<RedemptionResponseDto>> GetMyRedemptionsAsync(
            int userId,
            CancellationToken cancellationToken = default)
        {
            var recipient = await _recipientRepo.GetByUserIdAsync(userId);

            if (recipient == null)
            {
                throw new ResourceNotFoundException(
                    "Recipient profile not found.");
            }

            var redemptions = await _redemptionRepo.GetByRecipientIdAsync(
                recipient.Id,
                cancellationToken);

            return redemptions.Select(r => new RedemptionResponseDto
            {
                Id = r.Id,
                DonationId = r.DonationId,
                Amount = r.Amount,
                Status = r.Status,
                DonationDescription = r.Donation.Description,
                CreatedAt = r.CreatedAt,
                RedeemedAt = r.RedeemedAt
            }).ToList();
        }

        public async Task DeactivateMyAccountAsync(
            int userId,
            CancellationToken cancellationToken = default)
        {
            await using var transaction =
                await _unitOfWork.BeginTransactionAsync(
                    IsolationLevel.ReadCommitted,
                    cancellationToken);

            var recipient = await _recipientRepo
                .GetByUserIdForUpdateAsync(
                    userId,
                    cancellationToken);

            if (recipient == null)
            {
                throw new ResourceNotFoundException(
                    "Recipient profile not found.");
            }

            var user = await _userRepo.GetByIdAsync(userId);

            if (user == null || !user.IsActive)
            {
                throw new ForbiddenException(
                    "Recipient account is inactive.");
            }

            var pendingRedemptions = await _redemptionRepo
                .GetPendingByRecipientIdAsync(
                    recipient.Id,
                    cancellationToken);

            user.IsActive = false;

            foreach (var redemption in pendingRedemptions)
            {
                redemption.Status = "CANCELLED";
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }

    }
}