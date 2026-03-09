
using AutoMapper;
using Application.Interfaces;
using Application.DTOs.Auth;
using Domain.Interfaces;
using Domain.Exceptions;
using Domain.Entities;

namespace Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        public RoleService(IRoleRepository roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task CreateRoleAsync(RoleDto roleDto)
        {
            if (string.IsNullOrWhiteSpace(roleDto.RoleName))
                throw new BadRequestException("Please enter a role name!");

            if (await _roleRepository.RoleExistsAsync(roleDto.RoleName))
                throw new BadRequestException("Role name is already exists");

            var role = _mapper.Map<Role>(roleDto);
            //var role = new Role
            //{
            //    RoleName = roleDto.RoleName,
            //    Description = roleDto.Description,
            //};

            await _roleRepository.CreateRoleAsync(role);
        }

        public async Task<List<RoleResponseDto>> GetAllRolesAsync()
        {
            var roles = await _roleRepository.GetAllRolesAsync();

            var roleDtos = roles.Select(r => new RoleResponseDto
            {
                Id = r.Id,
                RoleName = r.RoleName,
                Description = r.Description,
            }).ToList();

            return roleDtos;
        }

        public async Task<RoleResponseDto> GetRoleByIdAsync(int id)
        {
            if (id <= 0)
                throw new BadRequestException("Enter a valid id number!");

            var role = await _roleRepository.GetRoleByIdAsync(id);
            if (role == null)
                throw new NotFoundException("Role not found!");

            var roleDto = new RoleResponseDto
            {
                Id = role.Id,
                RoleName = role.RoleName,
                Description = role.Description,
            };

            return roleDto;
        }

        public async Task UpdateRoleAsync(int id, RoleDto roleDto)
        {
            if (id <= 0)
                throw new BadRequestException("Enter a valid id number!");

            if (string.IsNullOrWhiteSpace(roleDto.RoleName))
                throw new BadRequestException("Please enter a role name!");

            var role = await _roleRepository.GetRoleByIdAsync(id);
            if (role == null)
                throw new NotFoundException("Role not found!");

            _mapper.Map(roleDto, role);

            //role.RoleName = roleDto.RoleName;
            //role.Description = roleDto.Description;

            await _roleRepository.UpdateRoleAsync(role);

        }

        public async Task DeleteRoleAsync(int id)
        {
            if (id <= 0)
                throw new BadRequestException("Enter a valid id number!");

            var role = await _roleRepository.GetRoleByIdAsync(id);
            if (role == null)
                throw new NotFoundException("Role not found!");

            await _roleRepository.DeleteRoleAsync(role);
        }

    }
}
