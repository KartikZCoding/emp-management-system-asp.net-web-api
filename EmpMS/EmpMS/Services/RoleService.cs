using EmpMS.DTOs.Auth;
using EmpMS.Models;
using EmpMS.Repositories;

namespace EmpMS.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task CreateRoleAsync(RoleDto roleDto)
        {
            if (string.IsNullOrWhiteSpace(roleDto.RoleName))
                throw new Exception("Please enter a role name!");

            if (await _roleRepository.RoleExistsAsync(roleDto.RoleName))
                throw new Exception("Role name is already exists");

            var role = new Role
            {
                RoleName = roleDto.RoleName,
                Description = roleDto.Description,
            };

            await _roleRepository.CreateRoleAsync(role);
        }

        public async Task<List<RoleDto>> GetAllRolesAsync()
        {
            var roles =  await _roleRepository.GetAllRolesAsync();

            var roleDtos = roles.Select(r => new RoleDto
            {
                RoleName = r.RoleName,
                Description = r.Description,
            }).ToList();

            return roleDtos;
        }
        public async Task<RoleDto> GetRoleByIdAsync(int id)
        {
            if (id <= 0)
                throw new Exception("Enter a valid id number!");

            var role = await _roleRepository.GetRoleByIdAsync(id);
            if (role == null)
                throw new Exception("Role not found!");

            var roleDto = new RoleDto
            {
                RoleName = role.RoleName,
                Description = role.Description,
            };

            return roleDto;
        }

        public async Task UpdateRoleAsync(int id, RoleDto roleDto)
        {
            if (id <= 0)
                throw new Exception("Enter a valid id number!");

            if (string.IsNullOrWhiteSpace(roleDto.RoleName))
                throw new Exception("Please enter a role name!");

            var role = await _roleRepository.GetRoleByIdAsync(id);
            if (role == null)
                throw new Exception("Role not found!");

            role.RoleName = roleDto.RoleName;
            role.Description = roleDto.Description;

            await _roleRepository.UpdateRoleAsync(role);
            
        }

        public async Task DeleteRoleAsync(int id)
        {
            if (id <= 0)
                throw new Exception("Enter a valid id number!");

            var role = await _roleRepository.GetRoleByIdAsync(id);
            if (role == null)
                throw new Exception("Role not found!");

            await _roleRepository.DeleteRoleAsync(role);
        }

    }
}
