using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendService.Data
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken stopToken = default);
    }
}