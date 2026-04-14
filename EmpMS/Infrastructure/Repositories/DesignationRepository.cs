using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class DesignationRepository : IDesignationRepository
    {
        private readonly AppDbContext _appDbContext;

        public DesignationRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<List<Designation>> GetAllAsync()
        {
            return await _appDbContext.Designations.AsNoTracking().Where(d => d.IsActive).ToListAsync();
        }

        public async Task<Designation?> GetByIdAsync(int id)
        {
            return await _appDbContext.Designations.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id && d.IsActive);
        }

        public async Task<bool> ExistsAsync(string designationName)
        {
            return await _appDbContext.Designations.AnyAsync(d => d.DesignationName == designationName && d.IsActive);
        }

        public async Task CreateAsync(Designation designation)
        {
            await _appDbContext.Designations.AddAsync(designation);

        }

        public async Task UpdateAsync(Designation designation)
        {
            _appDbContext.Designations.Update(designation);

        }

        public async Task DeleteAsync(Designation designation)
        {
            _appDbContext.Designations.Update(designation);

        }
    }
}
