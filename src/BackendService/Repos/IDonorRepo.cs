using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendService.Models;

namespace BackendService.Repos
{
    public interface IDonorRepo
    {
        Task AddAsync(Donor donor);

        Task<Donor?> GetByUserIdAsync(int userId);     
        Task<bool> IsDonorActiveAsync(int donorId);  
    }
}