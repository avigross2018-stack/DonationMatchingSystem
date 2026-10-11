using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendService.Models;

namespace BackendService.Repos
{
    public interface IUserRepo
    {
        Task<User?> GetByPhoneAsync(string phone);
        Task<User?> GetByIdAsync(int id);
        Task AddAsync(User user);
    }
}