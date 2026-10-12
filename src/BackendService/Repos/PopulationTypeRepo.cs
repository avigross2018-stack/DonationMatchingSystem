using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendService.Data;
using BackendService.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendService.Repos
{
    public class PopulationTypeRepo : IPopulationTypeRepo
    {
        private readonly AppDbContext _appDb;

        public PopulationTypeRepo(AppDbContext appDb)
        {
            _appDb = appDb;
        }

        public async Task<PopulationType?> GetByIdAsync(int id)
        {
            return await _appDb.PopulationTypes
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}