using DidMark.Core.DTO.Roles;
using DidMark.DataLayer.Entities.Access;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DidMark.Core.Services.Interfaces
{
    public interface IRoleService
    {
        Task<List<RoleDTO>> GetAllRolesAsync();
        Task<RoleDTO?> GetRoleByIdAsync(long roleId);
        Task<bool> AddRoleAsync(AddRoleDTO dto);
        Task<bool> EditRoleAsync(EditRoleDTO dto);
        Task<bool> DeleteRoleAsync(long roleId);

        Task<UserRoleAssignResult> AssignRoleToUserAsync(UserRoleAssignDTO dto);
        Task<UserRoleRemoveResult> RemoveRoleFromUserAsync(UserRoleAssignDTO dto);
        Task<List<string>> GetUserRolesAsync(long userId);
    }
}
