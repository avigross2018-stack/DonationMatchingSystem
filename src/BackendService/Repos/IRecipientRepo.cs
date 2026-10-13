using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        Task<Recipient?> GetByIdNoTrackingAsync(
            int recipientId,
            CancellationToken cancellationToken = default);

        Task<bool> IsRecipientActiveAsync(int recipientId);

        Task<Recipient?> GetByUserIdForUpdateAsync(
            int userId,
            CancellationToken cancellationToken = default);
    }
}