using EmpMS.Exceptions;
using EmpMS.DTOs.Auth;
using EmpMS.Models;
using EmpMS.Repositories;

namespace EmpMS.Services
{
    public class PrivilegeService : IPrivilegeService
    {
        private readonly IPrivilegeRepository _privilegeRepository;

        public PrivilegeService(IPrivilegeRepository privilegeRepository)
        {
            _privilegeRepository = privilegeRepository;
        }

        public async Task<List<PrivilegeDto>> GetAllPrivilegesAsync()
        {
            var privileges = await _privilegeRepository.GetAllPrivilegesAsync();
            return privileges.Select(p => new PrivilegeDto
            {
                PrivilegeName = p.PrivilegeName,
                Description = p.Description
            }).ToList();
        }

        public async Task<PrivilegeDto> GetPrivilegeByIdAsync(int id)
        {
            var privilege = await _privilegeRepository.GetPrivilegeByIdAsync(id);
            if (privilege == null) throw new NotFoundException("Privilege not found");

            return new PrivilegeDto
            {
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
        }

        public async Task DeletePrivilegeAsync(int id)
        {
            var privilege = await _privilegeRepository.GetPrivilegeByIdAsync(id);
            if (privilege == null) throw new NotFoundException("Privilege not found");

            await _privilegeRepository.DeletePrivilegeAsync(privilege);
        }
    }
}
