
using BackendService.Data;
using BackendService.DTOs;
using BackendService.Exceptions;
using BackendService.Repos;

namespace BackendService.Services
{
    public class DonorService
    {
        private readonly IDonorRepo _donorRepo;
        private readonly IUserRepo _userRepo;
        private readonly IUnitOfWork _unitOfWork;

        public DonorService(
            IDonorRepo donorRepo,
            IUserRepo userRepo,
            IUnitOfWork unitOfWork)
        {
            _donorRepo = donorRepo;
            _userRepo = userRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<DonorProfileDto> GetMyProfileAsync(int userId)
        {
            var donor = await _donorRepo.GetByUserIdAsync(userId);

            if (donor == null)
            {
                throw new ResourceNotFoundException(
                    "Donor profile not found.");
            }

            if (!donor.User.IsActive)
            {
                throw new ForbiddenException(
                    "Donor account is inactive.");
            }

            return new DonorProfileDto
            {
                Id = donor.Id,
                Name = donor.User.Name,
                Phone = donor.User.Phone,
                CreatedAt = donor.User.CreatedAt
            };
        }


        public async Task UpdateMyProfileAsync(
            int userId,
            UpdateDonorDto dto)
        {
            var donor = await _donorRepo.GetByUserIdAsync(userId);

            if (donor == null)
            {
                throw new ResourceNotFoundException(
                    "Donor profile not found.");
            }

            if (!donor.User.IsActive)
            {
                throw new ForbiddenException(
                    "Donor account is inactive.");
            }

            var name = dto.Name.Trim();
            var phone = dto.Phone.Trim();

            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(phone))
            {
                throw new BusinessValidationException(
                    "Name and phone are required.");
            }

            if (donor.User.Phone != phone)
            {
                var existingUser = await _userRepo.GetByPhoneAsync(phone);

                if (existingUser != null)
                {
                    throw new ConflictException(
                        "Phone number is already in use.");
                }
            }

            donor.User.Name = name;
            donor.User.Phone = phone;

            await _unitOfWork.SaveChangesAsync();
        }

    }
}
