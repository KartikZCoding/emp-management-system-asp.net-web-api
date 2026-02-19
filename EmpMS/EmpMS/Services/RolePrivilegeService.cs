using EmpMS.DTOs.Auth;
using EmpMS.Models;
using EmpMS.Repositories;

namespace EmpMS.Services
{
    public class RolePrivilegeService : IRolePrivilegeService
    {
        private readonly IRolePrivilegeRepository _repository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPrivilegeRepository _privilegeRepository;

        public RolePrivilegeService(
            IRolePrivilegeRepository repository,
            IRoleRepository roleRepository,
            IPrivilegeRepository privilegeRepository)
        {
            _repository = repository;
            _roleRepository = roleRepository;
            _privilegeRepository = privilegeRepository;
        }

        public async Task AssignPrivilegeToRoleAsync(RolePrivilegeDto dto)
        {
            // Validate Role exists
            var role = await _roleRepository.GetRoleByIdAsync(dto.RoleId);
            if (role == null) throw new Exception("Role not found");

            // Validate Privilege exists
            var privilege = await _privilegeRepository.GetPrivilegeByIdAsync(dto.PrivilegeId);
            if (privilege == null) throw new Exception("Privilege not found");

            // Check if already assigned
            if (await _repository.RolePrivilegeExistsAsync(dto.RoleId, dto.PrivilegeId))
                throw new Exception("Privilege already assigned to this role");

            var rolePrivilege = new RolePrivilege
            {
                RoleId = dto.RoleId,
                PrivilegeId = dto.PrivilegeId
            };

            await _repository.AddRolePrivilegeAsync(rolePrivilege);
        }

        public async Task<List<PrivilegeDto>> GetPrivilegesByRoleIdAsync(int roleId)
        {
            if (roleId <= 0) throw new Exception("Please enter a valid role id!");

            var privileges = await _repository.GetPrivilegesByRoleIdAsync(roleId);
            if (privileges.Count == 0) throw new Exception("Privileges not found!");

            return privileges.Select(p => new PrivilegeDto
            {
                PrivilegeName = p.PrivilegeName,
                Description = p.Description
            }).ToList();
        }

        public async Task RemovePrivilegeFromRoleAsync(int roleId, int privilegeId)
        {
            if (roleId <= 0) throw new Exception("Please enter a valid role id!");
            if (privilegeId <= 0) throw new Exception("Please enter a valid privilege id!");

            var rolePrivilege = await _repository.GetRolePrivilegeAsync(roleId, privilegeId);
            if (rolePrivilege == null) throw new Exception("Role-Privilege link not found");

            await _repository.DeleteRolePrivilegeAsync(rolePrivilege);
        }
    }
}
