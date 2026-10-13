using BackendService.Models;

namespace BackendService.Repos
{
    public interface IRecipientRepo
    {
        Task AddAsync(Recipient recipient);

        Task<Recipient?> GetByUserIdAsync(int userId);

        Task<Recipient?> GetByIdAsync(int id);

        Task<List<Recipient>> GetEligibleRecipientsAsync(
            CancellationToken cancellationToken = default);

        Task ReloadAsync(
            Recipient recipient,
            CancellationToken cancellationToken = default);

        Task<Recipient?> GetByIdForUpdateAsync(
            int recipientId,
            CancellationToken cancellationToken = default);

        Task<Recipient?> GetByIdForUpdateWithUserAsync(
            int recipientId,
            CancellationToken cancellationToken = default);

        Task<Recipient?> GetByIdNoTrackingAsync(
            int recipientId,
            CancellationToken cancellationToken = default);

        Task<bool> IsRecipientActiveAsync(int recipientId);

        Task<List<Recipient>> GetAllForAdminAsync(
            CancellationToken cancellationToken = default);

        Task<Recipient?> GetByIdForAdminAsync(
            int recipientId,
        Task<Recipient?> GetByUserIdForUpdateAsync(
            int userId,
            CancellationToken cancellationToken = default);
    }
}