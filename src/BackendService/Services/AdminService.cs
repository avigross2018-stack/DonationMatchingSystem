using BackendService.DTOs;
using BackendService.Exceptions;
using BackendService.Repos;

namespace BackendService.Services
{
    public sealed class AdminService
    {
        private readonly IAdminRepo _adminRepo;

        public AdminService(IAdminRepo adminRepo)
        {
            _adminRepo = adminRepo;
        }

        public async Task<AdminProfileDto> GetCurrentAdminAsync(
            int userId,
            CancellationToken cancellationToken = default)
        {
            if (userId <= 0)
            {
                throw new ForbiddenException(
                    "The authenticated user identifier is invalid.");
            }

            var admin = await _adminRepo.GetByUserIdAsync(
                userId,
                cancellationToken);

            if (admin is null || admin.User is null)
            {
                throw new ForbiddenException(
                    "The authenticated user does not have an admin profile.");
            }

            if (!admin.User.IsActive)
            {
                throw new ForbiddenException(
                    "The administrator account is inactive.");
            }

            return new AdminProfileDto
            {
                AdminId = admin.Id,
                UserId = admin.User.Id,
                Name = admin.User.Name,
                Phone = admin.User.Phone,
                IsActive = admin.User.IsActive,
                CreatedAt = admin.User.CreatedAt
            };
        }
    }
}