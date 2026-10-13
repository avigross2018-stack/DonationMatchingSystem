
using BackendService.Data;
using BackendService.DTOs;
using BackendService.Exceptions;
using BackendService.Models;
using BackendService.Repos;

namespace BackendService.Services
{
    public sealed class AdminPopulationTypeService
    {
        private readonly IPopulationTypeRepo _populationTypeRepo;
        private readonly IUnitOfWork _unitOfWork;

        public AdminPopulationTypeService(
            IPopulationTypeRepo populationTypeRepo,
            IUnitOfWork unitOfWork)
        {
            _populationTypeRepo = populationTypeRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<AdminPopulationTypeDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default)
        {
            var populationTypes =
                await _populationTypeRepo.GetAllAsync(
                    cancellationToken);

            // Map entities to DTOs before returning them to the API.
            return populationTypes
                .Select(MapToDto)
                .ToList();
        }

        public async Task<AdminPopulationTypeDto> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                throw new BusinessValidationException(
                    "Population type ID must be positive.");
            }

            var populationType =
                await _populationTypeRepo.GetByIdAsync(
                    id,
                    cancellationToken);

            if (populationType is null)
            {
                throw new ResourceNotFoundException(
                    $"Population type with ID {id} was not found.");
            }

            return MapToDto(populationType);
        }

        public async Task<AdminPopulationTypeDto> CreateAsync(
            CreatePopulationTypeDto request,
            CancellationToken cancellationToken = default)
        {
            var name = request.Name?.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new BusinessValidationException(
                    "Population type name is required.");
            }

            if (name.Length > 50)
            {
                throw new BusinessValidationException(
                    "Population type name cannot exceed 50 characters.");
            }

            var description = string.IsNullOrWhiteSpace(request.Description)
                ? null
                : request.Description.Trim();

            if (description?.Length > 250)
            {
                throw new BusinessValidationException(
                    "Population type description cannot exceed 250 characters.");
            }

            ValidateBaseEligibilityAmount(
                request.BaseEligibilityAmount);

            // Reject duplicate names before attempting to save.
            if (await _populationTypeRepo.ExistsByNameAsync(
                    name,
                    cancellationToken))
            {
                throw new ConflictException(
                    "A population type with this name already exists.");
            }

            var populationType = new PopulationType
            {
                Name = name,
                Description = description,
                BaseEligibilityAmount = request.BaseEligibilityAmount,
                IsActive = true
            };

            await _populationTypeRepo.AddAsync(
                populationType,
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToDto(populationType);
        }

        public async Task<AdminPopulationTypeDto> UpdateAsync(
            int id,
            UpdatePopulationTypeDto request,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                throw new BusinessValidationException(
                    "Population type ID must be positive.");
            }

            var name = request.Name?.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new BusinessValidationException(
                    "Population type name is required.");
            }

            if (name.Length > 50)
            {
                throw new BusinessValidationException(
                    "Population type name cannot exceed 50 characters.");
            }

            var description = string.IsNullOrWhiteSpace(request.Description)
                ? null
                : request.Description.Trim();

            if (description?.Length > 250)
            {
                throw new BusinessValidationException(
                    "Population type description cannot exceed 250 characters.");
            }

            ValidateBaseEligibilityAmount(
                request.BaseEligibilityAmount);

            var populationType =
                await _populationTypeRepo.GetTrackedByIdAsync(
                    id,
                    cancellationToken);

            if (populationType is null)
            {
                throw new ResourceNotFoundException(
                    $"Population type with ID {id} was not found.");
            }

            // Reject names that belong to another population type.
            if (await _populationTypeRepo.ExistsByNameAsync(
                    name,
                    id,
                    cancellationToken))
            {
                throw new ConflictException(
                    "A population type with this name already exists.");
            }

            populationType.Name = name;
            populationType.Description = description;
            populationType.BaseEligibilityAmount =
                request.BaseEligibilityAmount;

            // Existing recipient eligibility amounts remain unchanged.
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToDto(populationType);
        }

        public async Task<AdminPopulationTypeDto> UpdateStatusAsync(
            int id,
            UpdatePopulationTypeStatusDto request,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                throw new BusinessValidationException(
                    "Population type ID must be positive.");
            }

            if (!request.IsActive.HasValue)
            {
                throw new BusinessValidationException(
                    "The IsActive field is required.");
            }

            var populationType =
                await _populationTypeRepo.GetTrackedByIdAsync(
                    id,
                    cancellationToken);

            if (populationType is null)
            {
                throw new ResourceNotFoundException(
                    $"Population type with ID {id} was not found.");
            }

            populationType.IsActive = request.IsActive.Value;

            // Persist only the status change.
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToDto(populationType);
        }

        private static void ValidateBaseEligibilityAmount(decimal amount)
        {
            if (amount < 0 || amount > 99999999.99m)
            {
                throw new BusinessValidationException(
                    "Base eligibility amount must be between 0 and 99999999.99.");
            }
        }

        private static AdminPopulationTypeDto MapToDto(
            PopulationType populationType)
        {
            return new AdminPopulationTypeDto
            {
                Id = populationType.Id,
                Name = populationType.Name,
                Description = populationType.Description,
                BaseEligibilityAmount =
                    populationType.BaseEligibilityAmount,
                IsActive = populationType.IsActive
            };
        }
    }
}
