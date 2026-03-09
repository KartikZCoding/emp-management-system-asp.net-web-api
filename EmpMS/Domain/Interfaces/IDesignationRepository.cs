using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IDesignationRepository
    {
        Task<List<Designation>> GetAllAsync();
        Task<Designation?> GetByIdAsync(int id);
        Task<bool> ExistsAsync(string designationName);
        Task CreateAsync(Designation designation);
        Task UpdateAsync(Designation designation);
        Task DeleteAsync(Designation designation);
    }
}
