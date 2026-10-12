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
    }
}