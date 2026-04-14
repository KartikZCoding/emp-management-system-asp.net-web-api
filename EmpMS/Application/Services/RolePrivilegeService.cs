
using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Application.Services
{
    public class RolePrivilegeService : IRolePrivilegeService
    {
        private readonly IRolePrivilegeRepository _repository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPrivilegeRepository _privilegeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RolePrivilegeService(
            IRolePrivilegeRepository repository,
            IRoleRepository roleRepository,
            IPrivilegeRepository privilegeRepository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _roleRepository = roleRepository;
            _privilegeRepository = privilegeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task AssignPrivilegeToRoleAsync(RolePrivilegeDto dto)
        {
            // Validate Role exists
            var role = await _roleRepository.GetRoleByIdAsync(dto.RoleId);
            if (role == null) throw new NotFoundException("Role not found");

            // Validate Privilege exists
            var privilege = await _privilegeRepository.GetPrivilegeByIdAsync(dto.PrivilegeId);
            if (privilege == null) throw new NotFoundException("Privilege not found");

            // Check if already assigned
            if (await _repository.RolePrivilegeExistsAsync(dto.RoleId, dto.PrivilegeId))
                throw new BadRequestException("Privilege already assigned to this role");

            var rolePrivilege = new RolePrivilege
            {
                RoleId = dto.RoleId,
                PrivilegeId = dto.PrivilegeId
            };

            await _repository.AddRolePrivilegeAsync(rolePrivilege);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<PrivilegeResponseDto>> GetPrivilegesByRoleIdAsync(int roleId)
        {
            if (roleId <= 0) throw new BadRequestException("Please enter a valid role id!");

            var privileges = await _repository.GetPrivilegesByRoleIdAsync(roleId);
            if (privileges.Count == 0) throw new NotFoundException("Privileges not found!");

            return privileges.Select(p => new PrivilegeResponseDto
            {
                Id = p.Id,
                PrivilegeName = p.PrivilegeName,
                Description = p.Description
            }).ToList();
        }

        public async Task RemovePrivilegeFromRoleAsync(int roleId, int privilegeId)
        {
            if (roleId <= 0) throw new BadRequestException("Please enter a valid role id!");
            if (privilegeId <= 0) throw new BadRequestException("Please enter a valid privilege id!");

            var rolePrivilege = await _repository.GetRolePrivilegeAsync(roleId, privilegeId);
            if (rolePrivilege == null) throw new NotFoundException("Role-Privilege link not found");

            await _repository.DeleteRolePrivilegeAsync(rolePrivilege);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
