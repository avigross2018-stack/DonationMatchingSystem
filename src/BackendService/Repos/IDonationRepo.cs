using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendService.Models;

namespace BackendService.Repos
{
    public interface IDonationRepo
    {
        Task AddAsync(Donation donation);

        Task<Donation?> GetByIdAsync(int id);

        Task<List<Donation>> GetByDonorIdAsync(int donorId);
        Task<List<Donation>> GetExpiredActiveDonationsAsync(
            DateTime utcNow,
            CancellationToken cancellationToken = default); 
        Task<List<Donation>> GetAvailableDonationsAsync(
            DateTime utcNow,
            CancellationToken cancellationToken = default);

        Task<Donation?> GetByIdForUpdateAsync(
            int donationId,
            CancellationToken cancellationToken = default);
        Task<List<int>> GetActiveDonationIdsByDonorIdAsync(
            int donorId,
            CancellationToken cancellationToken = default);
    }
}