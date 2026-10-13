using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            CancellationToken cancellationToken = default);
    }
}