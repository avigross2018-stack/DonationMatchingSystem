using BackendService.Models;

namespace BackendService.Repos
{
    public interface IRedemptionRepo
    {
        Task<bool> HasBlockedMatchAsync(
            int donationId,
            int recipientId,
            CancellationToken cancellationToken = default);

        Task<decimal> GetCompletedTotalAsync(
            int donationId,
            int recipientId,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            Redemption redemption,
            CancellationToken cancellationToken = default);
            
        Task<List<Redemption>> GetPendingByDonationIdAsync(int donationId);

        Task<decimal> GetTotalCompletedByRecipientAsync(
            int recipientId,
            CancellationToken cancellationToken = default);

        Task<List<Redemption>> GetByRecipientIdAsync(
            int recipientId,
            CancellationToken cancellationToken = default);

        Task<List<Redemption>> GetPendingByRecipientIdAsync(
            int recipientId,

        Task<List<Redemption>> GetPendingByDonationIdAsync(
            int donationId);

        Task<(List<Redemption> Items, int TotalCount)> GetForAdminAsync(
            int? populationTypeId,
            string? status,
            DateTime? fromDate,
            DateTime? toDate,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<Redemption?> GetByIdForAdminAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<Redemption?> GetByIdForUpdateAsync(
            int redemptionId,
            CancellationToken cancellationToken = default);
    }
}