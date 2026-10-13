using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendService.Data;
using BackendService.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendService.Repos
{
    public class UserRepo : IUserRepo
    {
        private readonly AppDbContext _appDb;
        public UserRepo(AppDbContext appDb)
        {
            _appDb = appDb;
        }

        public async Task AddAsync(User user)
        {
            await _appDb.AddAsync(user);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _appDb.Users
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetByPhoneAsync(string phone)
        {
            return await _appDb.Users
                .FirstOrDefaultAsync(u => u.Phone == phone);
        }

        public async Task<User?> GetByIdForUpdateAsync(
            int userId,
            CancellationToken cancellationToken = default)
        {
            var query = _appDb.Users
                .FromSqlInterpolated(
                    $"SELECT * FROM `Users` WHERE `Id` = {userId} FOR UPDATE")
                .AsAsyncEnumerable();

            await foreach (var user in
                query.WithCancellation(cancellationToken))
            {
                return user;
            }

            return null;
        }
    }
}