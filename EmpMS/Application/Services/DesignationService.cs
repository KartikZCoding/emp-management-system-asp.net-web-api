using AutoMapper;
using Application.DTOs.Designation;
using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Application.Services
{
    public class DesignationService : IDesignationService
    {
        private readonly IDesignationRepository _designationRepository;
        private readonly IMapper _mapper;

        public DesignationService(IDesignationRepository designationRepository, IMapper mapper)
        {
            _designationRepository = designationRepository;
            _mapper = mapper;
        }

        public async Task<List<DesignationResponseDto>> GetAllDesignationsAsync()
        {
            var designations = await _designationRepository.GetAllAsync();
            return _mapper.Map<List<DesignationResponseDto>>(designations);
        }

        public async Task<DesignationResponseDto> GetDesignationByIdAsync(int id)
        {
            if (id <= 0)
                throw new BadRequestException("Enter a valid id number!");

            var designation = await _designationRepository.GetByIdAsync(id);
            if (designation == null)
                throw new NotFoundException("Designation not found!");

            return _mapper.Map<DesignationResponseDto>(designation);
        }

        public async Task CreateDesignationAsync(DesignationDto designationDto)
        {
            if (string.IsNullOrWhiteSpace(designationDto.DesignationName))
                throw new BadRequestException("Please enter a designation name!");

            if (await _designationRepository.ExistsAsync(designationDto.DesignationName))
                throw new BadRequestException("Designation name already exists!");

            var designation = _mapper.Map<Designation>(designationDto);
            designation.CreatedAt = DateTime.Now;

            await _designationRepository.CreateAsync(designation);
        }

        public async Task UpdateDesignationAsync(int id, DesignationDto designationDto)
        {
            if (id <= 0)
                throw new BadRequestException("Enter a valid id number!");

            if (string.IsNullOrWhiteSpace(designationDto.DesignationName))
                throw new BadRequestException("Please enter a designation name!");

            var designation = await _designationRepository.GetByIdAsync(id);
            if (designation == null)
                throw new NotFoundException("Designation not found!");

            _mapper.Map(designationDto, designation);
            designation.UpdatedAt = DateTime.Now;

            await _designationRepository.UpdateAsync(designation);
        }

        public async Task DeleteDesignationAsync(int id)
        {
            if (id <= 0)
                throw new BadRequestException("Enter a valid id number!");

            var designation = await _designationRepository.GetByIdAsync(id);
            if (designation == null)
                throw new NotFoundException("Designation not found!");

            //soft delete
            designation.IsActive = false;
            designation.UpdatedAt = DateTime.Now;

            await _designationRepository.DeleteAsync(designation);
        }
    }
}
