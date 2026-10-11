using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
    }
}