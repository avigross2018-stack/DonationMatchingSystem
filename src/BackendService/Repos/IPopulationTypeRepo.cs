using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendService.Models;

namespace BackendService.Repos
{
    public interface IPopulationTypeRepo
    {
        Task<PopulationType?> GetByIdAsync(int id);
    }
}