using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class SalaryStructureRepository : ISalaryStructureRepository
    {
        private readonly AppDbContext _appDbContext;

        public SalaryStructureRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<List<SalaryStructure>> GetAllActiveAsync()
        {
            return await _appDbContext.SalaryStructures
                .Where(ss => ss.IsActive)
                .OrderBy(ss => ss.ComponentType)
                .ThenBy(ss => ss.DisplayOrder)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<SalaryStructure?> GetByIdAsync(int id)
        {
            return await _appDbContext.SalaryStructures
                .AsNoTracking()
                .FirstOrDefaultAsync(ss => ss.Id == id);
        }
    }
}
