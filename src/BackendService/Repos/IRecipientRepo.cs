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
    }
}