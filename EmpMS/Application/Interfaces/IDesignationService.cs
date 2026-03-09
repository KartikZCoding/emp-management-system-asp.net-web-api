using Application.DTOs.Designation;

namespace Application.Interfaces
{
    public interface IDesignationService
    {
        Task<List<DesignationResponseDto>> GetAllDesignationsAsync();
        Task<DesignationResponseDto> GetDesignationByIdAsync(int id);
        Task CreateDesignationAsync(DesignationDto designationDto);
        Task UpdateDesignationAsync(int id, DesignationDto designationDto);
        Task DeleteDesignationAsync(int id);
    }
}
