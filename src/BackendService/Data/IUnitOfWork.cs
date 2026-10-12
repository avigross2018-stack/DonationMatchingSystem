using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace BackendService.Data
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken stopToken = default);
        Task<IDbContextTransaction> BeginTransactionAsync(
            IsolationLevel isolationLevel,
            CancellationToken cancellationToken = default);
    }
}