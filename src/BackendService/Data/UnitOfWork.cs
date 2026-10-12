using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BackendService.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _appDb;
        public UnitOfWork(AppDbContext appDb)
        {
            _appDb = appDb;
        }

        public async Task<int> SaveChangesAsync(CancellationToken stopToken = default)
        {
            return await _appDb.SaveChangesAsync(stopToken);
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(
            IsolationLevel isolationLevel,
            CancellationToken cancellationToken = default)
        {
            return await _appDb.Database.BeginTransactionAsync(
                isolationLevel,
                cancellationToken);
        }
    }
}