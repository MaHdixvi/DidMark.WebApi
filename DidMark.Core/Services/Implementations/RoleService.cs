using DidMark.Core.DTO.Roles;
using DidMark.Core.Services.Interfaces;
using DidMark.DataLayer.Entities.Access;
using DidMark.DataLayer.Entities.Account;
using DidMark.DataLayer.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DidMark.Core.Services.Implementations
{
    public class RoleService : IRoleService
    {
        private readonly IGenericRepository<Role> _roleRepository;
        private readonly IGenericRepository<UserRole> _userRoleRepository;
        private readonly IGenericRepository<User> _userRepository;

        public RoleService(
            IGenericRepository<Role> roleRepository,
            IGenericRepository<UserRole> userRoleRepository,
            IGenericRepository<User> userRepository)
        {
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _userRoleRepository = userRoleRepository ?? throw new ArgumentNullException(nameof(userRoleRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        // گرفتن همه نقش‌ها (فقط حذف‌نشده‌ها)
        public async Task<List<RoleDTO>> GetAllRolesAsync()
        {
            return await _roleRepository.GetEntitiesQuery()
                .Where(r => !r.IsDelete)
                .Select(r => new RoleDTO
                {
                    Id = r.Id,
                    RoleName = r.RoleName,
                    RoleTitle = r.RoleTitle
                })
                .ToListAsync();
        }

        // گرفتن نقش بر اساس Id
        public async Task<RoleDTO?> GetRoleByIdAsync(long roleId)
        {
            var role = await _roleRepository.GetEntitiesQuery()
                .Where(r => !r.IsDelete && r.Id == roleId)
                .SingleOrDefaultAsync();

            if (role == null) return null;

            return new RoleDTO
            {
                Id = role.Id,
                RoleName = role.RoleName,
                RoleTitle = role.RoleTitle
            };
        }

        // افزودن نقش جدید
        public async Task<bool> AddRoleAsync(AddRoleDTO dto)
        {
            var role = new Role
            {
                RoleName = dto.RoleName,
                RoleTitle = dto.RoleTitle
            };

            await _roleRepository.AddEntity(role);
            await _roleRepository.SaveChanges();
            return true;
        }

        // ویرایش نقش
        public async Task<bool> EditRoleAsync(EditRoleDTO dto)
        {
            var existingRole = await _roleRepository.GetEntitiesQuery()
                .Where(r => !r.IsDelete && r.Id == dto.Id)
                .SingleOrDefaultAsync();

            if (existingRole == null) return false;

            existingRole.RoleName = dto.RoleName;
            existingRole.RoleTitle = dto.RoleTitle;

            _roleRepository.UpdateEntity(existingRole);
            await _roleRepository.SaveChanges();
            return true;
        }

        // حذف نقش (فیزیکی یا می‌توان IsDeleted=true کرد)
        public async Task<bool> DeleteRoleAsync(long roleId)
        {
            var role = await _roleRepository.GetEntitiesQuery()
                .Where(r => !r.IsDelete && r.Id == roleId)
                .SingleOrDefaultAsync();

            if (role == null) return false;

            _roleRepository.RemoveEntity(role);
            await _roleRepository.SaveChanges();
            return true;
        }

        // اختصاص نقش به کاربر
        public async Task<UserRoleAssignResult> AssignRoleToUserAsync(UserRoleAssignDTO dto)
        {
            var hasUser = _userRepository.GetEntitiesQuery().Any(x => x.Id == dto.UserId);
            var hasRole = _roleRepository.GetEntitiesQuery().Any(x => x.Id == dto.RoleId);
            if (!hasUser||!hasRole)
            {
                return UserRoleAssignResult.Notfound;
            }

            var exists = await _userRoleRepository.GetEntitiesQuery()
                .Where(x => !x.IsDelete)
                .SingleOrDefaultAsync(x => x.UserId == dto.UserId && x.RoleId == dto.RoleId);

            if (exists != null) return UserRoleAssignResult.AlreadyHasRole;

            var userRole = new UserRole
            {
                UserId = dto.UserId,
                RoleId = dto.RoleId
            };

            await _userRoleRepository.AddEntity(userRole);
            await _userRoleRepository.SaveChanges();
            return UserRoleAssignResult.Success;
        }

        // حذف نقش از کاربر
        public async Task<UserRoleRemoveResult> RemoveRoleFromUserAsync(UserRoleAssignDTO dto)
        {
            var hasUser = _userRepository.GetEntitiesQuery().Any(x => x.Id == dto.UserId);
            var hasRole = _roleRepository.GetEntitiesQuery().Any(x => x.Id == dto.RoleId);
            if (!hasUser || !hasRole)
            {
                return UserRoleRemoveResult.Notfound;
            }

            var userRole = await _userRoleRepository.GetEntitiesQuery()
                .Where(x => !x.IsDelete)
                .SingleOrDefaultAsync(x => x.UserId == dto.UserId && x.RoleId == dto.RoleId);

            if (userRole == null) return UserRoleRemoveResult.DoesNotHaveRole;

            _userRoleRepository.RemoveEntity(userRole);
            await _userRoleRepository.SaveChanges();
            return UserRoleRemoveResult.Success;
        }

        // گرفتن همه نقش‌های کاربر
        public async Task<List<string>> GetUserRolesAsync(long userId)
        {
            return await _userRoleRepository.GetEntitiesQuery()
                .Include(x => x.Role)
                .Where(x => x.UserId == userId && !x.IsDelete && !x.Role.IsDelete)
                .Select(x => x.Role.RoleTitle)
                .ToListAsync();
        }

    }
}
