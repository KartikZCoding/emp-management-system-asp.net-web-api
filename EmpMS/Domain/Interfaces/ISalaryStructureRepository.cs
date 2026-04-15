using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface ISalaryStructureRepository
    {
        Task<List<SalaryStructure>> GetAllActiveAsync();
        Task<SalaryStructure?> GetByIdAsync(int id);
    }
}
