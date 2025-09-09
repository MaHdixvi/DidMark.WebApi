using DidMark.Core.DTO.Account;
using DidMark.Core.DTO.Roles;
using DidMark.Core.DTO.Users;
using DidMark.Core.Services.Implementations;
using DidMark.Core.Services.Interfaces;
using DidMark.Core.Utilities.Common;
using DidMark.WebApi.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DidMark.WebApi.Controllers
{
    public class AdminUsersController : SiteBaseController
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;


        #region Constructor
        public AdminUsersController(IUserService userService, IRoleService roleService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
        }
        #endregion

        #region Get All Users
        [HttpGet("users/get-all")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return JsonResponseStatus.Success(users);
        }
        #endregion

        #region Get User By Id
        [HttpGet("users/{id:long}")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> GetUser(long id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return JsonResponseStatus.NotFound(new { message = "کاربر یافت نشد" });

            return JsonResponseStatus.Success(user);
        }
        #endregion

        #region Update User
        [HttpPut("users/update/{id:long}")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> UpdateUser(long id, [FromBody] EditUserByAdminDTO dto)
        {
            if (dto == null)
                return JsonResponseStatus.BadRequest(new { message = "اطلاعات ارسال نشده است" });

            dto.Id = id; // اطمینان از اینکه آی‌دی درست ست شده
            var result = await _userService.UpdateUserByAdminAsync(dto);

            return result switch
            {
                EditUserByAdminResult.Success => JsonResponseStatus.Success(new { message = "اطلاعات کاربر با موفقیت ویرایش شد" }),
                EditUserByAdminResult.EmailExists => JsonResponseStatus.BadRequest(new { message = "ایمیل تکراری است " }),
                _ => JsonResponseStatus.Error(new { message = "خطا در ویرایش اطلاعات کاربر" })
            };
        }
        #endregion

        #region Toggle User Status
        [HttpPatch("users/toggle-status/{id:long}")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> ToggleStatus(long id)
        {
            var result = await _userService.ToggleStatusAsync(id);
            if (!result)
                return JsonResponseStatus.NotFound(new { message = "کاربر یافت نشد" });

            return JsonResponseStatus.Success(new { message = "وضعیت کاربر تغییر یافت" });
        }
        #endregion
        #region Roles

        [HttpGet("roles/get-all")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return JsonResponseStatus.Success(roles);
        }

        [HttpPost("user-roles/assign")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> AssignRole([FromBody] UserRoleAssignDTO dto)
        {
            if (dto == null) return JsonResponseStatus.BadRequest(new { message = "اطلاعات ارسال نشده است" });

            var result = await _roleService.AssignRoleToUserAsync(dto);
            if (!result) return JsonResponseStatus.BadRequest(new { message = "نقش قبلا به کاربر اختصاص داده شده است" });

            return JsonResponseStatus.Success(new { message = "نقش به کاربر اختصاص یافت" });
        }

        [HttpPost("user-roles/remove")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> RemoveRole([FromBody] UserRoleAssignDTO dto)
        {
            if (dto == null) return JsonResponseStatus.BadRequest(new { message = "اطلاعات ارسال نشده است" });

            var result = await _roleService.RemoveRoleFromUserAsync(dto);
            if (!result) return JsonResponseStatus.BadRequest(new { message = "نقش به کاربر اختصاص داده نشده بود" });

            return JsonResponseStatus.Success(new { message = "نقش از کاربر حذف شد" });
        }

        [HttpGet("user-roles/{userId:long}")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> GetUserRoles(long userId)
        {
            var roles = await _roleService.GetUserRolesAsync(userId);
            return JsonResponseStatus.Success(roles);
        }
        #endregion

    }
}
