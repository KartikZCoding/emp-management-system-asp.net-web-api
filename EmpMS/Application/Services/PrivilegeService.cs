
using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Application.Services
{
    public class PrivilegeService : IPrivilegeService
    {
        private readonly IPrivilegeRepository _privilegeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PrivilegeService(IPrivilegeRepository privilegeRepository, IUnitOfWork unitOfWork)
        {
            _privilegeRepository = privilegeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<PrivilegeResponseDto>> GetAllPrivilegesAsync()
        {
            var privileges = await _privilegeRepository.GetAllPrivilegesAsync();
            return privileges.Select(p => new PrivilegeResponseDto
            {
                Id = p.Id,
                PrivilegeName = p.PrivilegeName,
                Description = p.Description
            }).ToList();
        }

        public async Task<PrivilegeResponseDto> GetPrivilegeByIdAsync(int id)
        {
            var privilege = await _privilegeRepository.GetPrivilegeByIdAsync(id);
            if (privilege == null) throw new NotFoundException("Privilege not found");

            return new PrivilegeResponseDto
            {
                Id = privilege.Id,
                PrivilegeName = privilege.PrivilegeName,
                Description = privilege.Description
            };
        }

        public async Task CreatePrivilegeAsync(PrivilegeDto dto)
        {
            if (await _privilegeRepository.PrivilegeExistsAsync(dto.PrivilegeName))
                throw new BadRequestException("Privilege already exists");

            var privilege = new Privilege
            {
                PrivilegeName = dto.PrivilegeName,
                Description = dto.Description
            };

            await _privilegeRepository.CreatePrivilegeAsync(privilege);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdatePrivilegeAsync(int id, PrivilegeDto dto)
        {
            var privilege = await _privilegeRepository.GetPrivilegeByIdAsync(id);
            if (privilege == null) throw new NotFoundException("Privilege not found");

            if (await _privilegeRepository.PrivilegeExistsAsync(dto.PrivilegeName))
                throw new BadRequestException("Privilege name already exists!");

            privilege.PrivilegeName = dto.PrivilegeName;
            privilege.Description = dto.Description;

            await _privilegeRepository.UpdatePrivilegeAsync(privilege);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeletePrivilegeAsync(int id)
        {
            var privilege = await _privilegeRepository.GetPrivilegeByIdAsync(id);
            if (privilege == null) throw new NotFoundException("Privilege not found");

            await _privilegeRepository.DeletePrivilegeAsync(privilege);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
