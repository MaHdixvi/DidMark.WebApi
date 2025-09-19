using DidMark.Core.DTO.Account;
using DidMark.Core.Security;
using DidMark.Core.Services.Implementations;
using DidMark.Core.Services.Interfaces;
using DidMark.Core.Utilities.Common;
using DidMark.Core.Utilities.Extensions.Identity;
using DidMark.DataLayer.Entities.Account;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DidMark.WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/admin/[controller]")]
    public class AdminAccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IRoleService _roleService;


        public AdminAccountController(IUserService userService, IJwtTokenService jwtTokenService, IRoleService roleService)
        {
            _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));

            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO login)
        {
            if (!ModelState.IsValid)
            {
                return JsonResponseStatus.BadRequest(new { message = "داده‌های ورودی معتبر نیستند" });
            }

            var result = await _userService.LoginUserAsync(login, checkAdminRole: true);

            switch (result)
            {
                case LoginUserResult.IncorrectData:
                    return JsonResponseStatus.Unauthorized(new { message = "ایمیل یا رمز عبور اشتباه است" });

                case LoginUserResult.NotActivated:
                    return JsonResponseStatus.Forbidden(new { message = "حساب کاربری شما فعال نشده است" });

                case LoginUserResult.NotAdmin:
                    return JsonResponseStatus.Forbidden(new { message = "شما دسترسی ادمین ندارید" });

                case LoginUserResult.Success:

                    User user;
                    if (login.UsernameOrPhone.All(char.IsDigit))
                    {
                        // یعنی ورودی شماره تلفنه
                        user = await _userService.GetUserByPhoneNumberAsync(login.UsernameOrPhone);
                    }
                    else
                    {
                        // یعنی ورودی نام کاربری هست
                        user = await _userService.GetUserByUsernameAsync(login.UsernameOrPhone);
                    }
                    if (user == null)
                    {
                        return JsonResponseStatus.NotFound(new { message = "کاربر یافت نشد" });
                    }
                    var roles = await _roleService.GetUserRolesAsync(user.Id);

                    var token = _jwtTokenService.GenerateJwtToken(user, roles);
                    return JsonResponseStatus.Success(new
                    {
                        token,
                        expireTime = 30, // Matches JwtSettings.ExpireDays
                        user.Id,
                        user.FirstName,
                        user.LastName,
                        user.Email,
                        user.PhoneNumber,
                        Roles = roles
                    });

                default:
                    return JsonResponseStatus.Error(new { message = "خطای ناشناخته در ورود" });
            }
        }

        [HttpGet("check-auth")]
        public async Task<IActionResult> CheckUserAuth()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return JsonResponseStatus.Unauthorized(new { message = "کاربر احراز هویت نشده است" });
            }

            var userId = User.GetUserId();
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return JsonResponseStatus.NotFound(new { message = "کاربر یافت نشد" });
            }

            if (!await _userService.IsAdminAsync(user.Id))
            {
                return JsonResponseStatus.Forbidden(new { message = "شما دسترسی ادمین ندارید" });
            }

            return JsonResponseStatus.Success(new
            {
                userInfo = new
                {
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.Username,
                    user.PhoneNumber
                }
            });
        }
    }
}