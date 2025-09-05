using DidMark.Core.DTO.Account;
using DidMark.Core.DTO.Users;
using DidMark.Core.Services.Interfaces;
using DidMark.Core.Utilities.Common;
using DidMark.WebApi.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DidMark.WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/admin/users")]
    public class AdminUsersController : SiteBaseController
    {
        private readonly IUserService _userService;

        #region Constructor
        public AdminUsersController(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }
        #endregion

        #region Get All Users
        [HttpGet("get-all")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return JsonResponseStatus.Success(users);
        }
        #endregion

        #region Get User By Id
        [HttpGet("{id:long}")]
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
        [HttpPut("update/{id:long}")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> UpdateUser(long id, [FromBody] EditUserByAdminDTO dto)
        {
            if (dto == null)
                return JsonResponseStatus.BadRequest(new { message = "اطلاعات ارسال نشده است" });

            dto.Id = id; // اطمینان از اینکه آی‌دی درست ست شده
            var result = await _userService.UpdateUserByAdminAsync(dto);
            if (!result)
                return JsonResponseStatus.NotFound(new { message = "کاربر یافت نشد" });

            return JsonResponseStatus.Success(new { message = "کاربر با موفقیت بروزرسانی شد" });
        }
        #endregion

        #region Toggle User Status
        [HttpPatch("toggle-status/{id:long}")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> ToggleStatus(long id)
        {
            var result = await _userService.ToggleStatusAsync(id);
            if (!result)
                return JsonResponseStatus.NotFound(new { message = "کاربر یافت نشد" });

            return JsonResponseStatus.Success(new { message = "وضعیت کاربر تغییر یافت" });
        }

        #endregion
    }
}
