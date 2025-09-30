using DidMark.Core.DTO.Account;
using DidMark.Core.Security;
using DidMark.Core.Services.Interfaces;
using DidMark.Core.Utilities.Common;
using DidMark.Core.Utilities.Enums;
using DidMark.Core.Utilities.Extensions.Identity;
using DidMark.DataLayer.Entities.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DidMark.WebApi.Controllers
{

    public class AccountController : SiteBaseController
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IJwtTokenService _jwtTokenService;

        public AccountController(IUserService userService, IJwtTokenService jwtTokenService, IRoleService roleService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
            _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO register)
        {
            if (!ModelState.IsValid)
            {
                return JsonResponseStatus.BadRequest(new { message = "داده‌های ورودی معتبر نیستند" });
            }

            var result = await _userService.RegisterUserAsync(register);

            return result switch
            {
                RegisterUserResult.UsernameExists => JsonResponseStatus.Conflict(new { message = "نام کاربری قبلاً ثبت شده است" }),
                RegisterUserResult.PhoneNumberExists => JsonResponseStatus.Conflict(new { message = "شماره تلفن قبلاً ثبت شده است" }),
                RegisterUserResult.Success => JsonResponseStatus.Created(new { message = "ثبت‌نام با موفقیت انجام شد. لطفاً ایمیل خود را بررسی کنید." }),
                _ => JsonResponseStatus.Error(new { message = "خطای ناشناخته در ثبت‌نام" })
            };
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO login)
        {
            if (!ModelState.IsValid)
                return JsonResponseStatus.BadRequest(new { message = "داده‌های ورودی معتبر نیستند" });

            var result = await _userService.LoginUserAsync(login);
            if (result == LoginUserResult.IncorrectData)
                return JsonResponseStatus.Unauthorized(new { message = "ایمیل یا رمز عبور اشتباه است" });

            if (result == LoginUserResult.NotActivated)
                return JsonResponseStatus.Forbidden(new { message = "حساب کاربری شما فعال نشده است" });

            if (result == LoginUserResult.Success)
            {
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
                    return JsonResponseStatus.NotFound(new { message = "کاربر یافت نشد" });
                var roles = await _roleService.GetUserRolesAsync(user.Id);

                var token = _jwtTokenService.GenerateJwtToken(user, roles);
                return JsonResponseStatus.Success(new
                {
                    token,
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.PhoneNumber,
                    Roles = roles // اینجا رول‌ها رو هم برمی‌گردونی
                });
            }

            return JsonResponseStatus.Error(new { message = "خطای ناشناخته در ورود" });
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

            return JsonResponseStatus.Success(new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email,
                user.Username,
                user.PhoneNumber
            });
        }

        [HttpGet("activate-account/{activationCode}")]
        public async Task<IActionResult> ActivateAccount(string activationCode)
        {
            if (string.IsNullOrEmpty(activationCode))
            {
                return JsonResponseStatus.BadRequest(new { message = "کد فعال‌سازی نامعتبر است" });
            }

            var user = await _userService.GetUserByActivationCodeAsync(activationCode);
            if (user == null)
            {
                return JsonResponseStatus.NotFound(new { message = "کد فعال‌سازی نامعتبر یا منقضی شده است" });
            }

            var activationResult = await _userService.ActivateUserAsync(user);
            return activationResult
                ? JsonResponseStatus.Success(new { message = "حساب کاربری با موفقیت فعال شد" })
                : JsonResponseStatus.Error(new { message = "خطا در فعال‌سازی حساب کاربری" });
        }

        [HttpPost("sign-out")]
        public async Task<IActionResult> SignOut()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return JsonResponseStatus.Unauthorized(new { message = "کاربر احراز هویت نشده است" });
            }

            //await HttpContext.SignOutAsync();
            return JsonResponseStatus.Success(new { message = "خروج با موفقیت انجام شد" });
            //سمت فرانت اند انجام می شود
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] EditUserDTO editUser)
        {
            if (!ModelState.IsValid)
            {
                return JsonResponseStatus.BadRequest(new { message = "داده‌های ورودی معتبر نیستند" });
            }

            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return JsonResponseStatus.Unauthorized(new { message = "کاربر احراز هویت نشده است" });
            }

            var userId = User.GetUserId();
            var updateResult = await _userService.UpdateUserAsync(editUser, userId);

            return updateResult switch
            {
                EditUserResult.Success => JsonResponseStatus.Success(new {message = "اطلاعات کاربر با موفقیت ویرایش شد" }),
                EditUserResult.EmailExists => JsonResponseStatus.BadRequest(new { message = "ایمیل تکراری است "}),
                _ => JsonResponseStatus.Error(new { message = "خطا در ویرایش اطلاعات کاربر" })
            };
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePassword)
        {
            if (!ModelState.IsValid)
            {
                return JsonResponseStatus.BadRequest(new { message = "داده‌های ورودی معتبر نیستند" });
            }

            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return JsonResponseStatus.Unauthorized(new { message = "کاربر احراز هویت نشده است" });
            }

            var userId = User.GetUserId();
            var changeResult = await _userService.ChangePasswordAsync(changePassword, userId);

            return changeResult switch
            {
                ChangePasswordResult.Success => JsonResponseStatus.Success(new { message = "رمز عبور با موفقیت تغییر یافت" }),
                ChangePasswordResult.IncorrectCurrentPassword => JsonResponseStatus.BadRequest(new { message = "رمز عبور فعلی اشتباه است" }),
                ChangePasswordResult.SameAsOldPassword => JsonResponseStatus.BadRequest(new { message = "رمز عبور جدید نباید با رمز قبلی یکسان باشد" }),
                ChangePasswordResult.NotSameNewPasswordAndConfirmPassword => JsonResponseStatus.BadRequest(new { message = "رمز عبور جدید و تکرارش باید یکسان باشند" }),
                ChangePasswordResult.UserNotFound => JsonResponseStatus.NotFound(new { message = "کاربر یافت نشد" }),
                _ => JsonResponseStatus.Error(new { message = "خطا در تغییر رمز عبور" })
            };
        }
        [HttpPost("activate-email/{activationCode}")]
        public async Task<IActionResult> ActivateEmail(string activationCode)
        {
            if (string.IsNullOrEmpty(activationCode))
            {
                return JsonResponseStatus.BadRequest(new { message = "کد فعال‌سازی نامعتبر است" });
            }

            var user = await _userService.GetUserByEmailActivationCodeAsync(activationCode);
            if (user == null)
            {
                return JsonResponseStatus.NotFound(new { message = "کد فعال‌سازی نامعتبر یا منقضی شده است" });
            }

            var activationResult = await _userService.ActivateUserEmailAsync(user);
            return activationResult
                ? JsonResponseStatus.Success(new { message = "ایمیل با موفقیت فعال شد" })
                : JsonResponseStatus.Error(new { message = "خطا در فعال‌سازی ایمیل" });
        }
        [Authorize]

        [HttpPost("send-email-activation")]
        public async Task<IActionResult> SendEmailActivation()
        {
            if (!ModelState.IsValid)
            {
                return JsonResponseStatus.BadRequest(new { message = "داده‌های ورودی معتبر نیستند" });
            }
            var userId = User.GetUserId();

            var result = await _userService.SendEmailActivationSmsAsync(userId);

            return result
                ? JsonResponseStatus.Success(new { message = "کد فعال‌سازی به ایمیل ارسال شد" })
                : JsonResponseStatus.Error(new { message = "خطا در ارسال کد فعال‌سازی ایمیل" });
        }
        [Authorize]
        [HttpPost("send-phone-activation")]
        public async Task<IActionResult> SendPhoneActivation()
        {
            if (!ModelState.IsValid)
            {
                return JsonResponseStatus.BadRequest(new { message = "داده‌های ورودی معتبر نیستند" });
            }
            var userId = User.GetUserId();

            var result = await _userService.SendPhoneNumberActivationSmsAsync(userId);

            return result
                ? JsonResponseStatus.Success(new { message = "کد فعال‌سازی به شماره تلفن ارسال شد" })
                : JsonResponseStatus.Error(new { message = "خطا در ارسال کد فعال‌سازی شماره تلفن" });
        }
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO forgotPassword)
        {
            if (!ModelState.IsValid)
            {
                return JsonResponseStatus.BadRequest(new { message = "داده‌های ورودی معتبر نیستند" });
            }

            var result = await _userService.ForgotPasswordAsync(forgotPassword);

            return result switch
            {
                ForgotPasswordResult.UserNotFound => JsonResponseStatus.NotFound(new { message = "کاربری با این اطلاعات یافت نشد" }),
                ForgotPasswordResult.Success => JsonResponseStatus.Success(new { message = "کد بازیابی رمز عبور ارسال شد" }),
                _ => JsonResponseStatus.Error(new { message = "خطا در ارسال کد بازیابی رمز عبور" })
            };
        }
        [HttpPost("verify-reset-code")]
        public async Task<IActionResult> VerifyResetCode([FromBody] VerifyResetCodeDTO verify)
        {
            if (string.IsNullOrEmpty(verify.ResetCode))
                return JsonResponseStatus.BadRequest(new { message = "کد بازیابی وارد نشده است" });

            var user = await _userService.CheckResetCodeAsync(verify.ResetCode);

            return user switch
            {
                CheckResetCodeResult.UserNotFound => JsonResponseStatus.NotFound(new { message = "کد بازیابی نامعتبر است" }),
                CheckResetCodeResult.ResetPasswordExpireDatePassed => JsonResponseStatus.Success(new { message = "کد بازیابی منقضی شده است" }),
                CheckResetCodeResult.Success => JsonResponseStatus.Success(new { message = "کد بازیابی معتبر است" }),
                _ => JsonResponseStatus.Error(new { message = "خطا در اعتبارنجی کد بازیابی رمز عبور" })
            };
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPassword)
        {
            if (!ModelState.IsValid)
            {
                return JsonResponseStatus.BadRequest(new { message = "داده‌های ورودی معتبر نیستند" });
            }

            var result = await _userService.ResetPasswordAsync(resetPassword);

            return result switch
            {
                ResetPasswordResult.InvalidToken => JsonResponseStatus.BadRequest(new { message = "کد بازیابی نامعتبر است" }),
                ResetPasswordResult.ExpiredToken => JsonResponseStatus.BadRequest(new { message = "کد بازیابی منقضی شده است" }),
                ResetPasswordResult.SameAsOldPassword => JsonResponseStatus.BadRequest(new { message = "رمز جدید نباید با رمز قبلی یکسان باشد" }),
                ResetPasswordResult.NotSameNewPasswordAndConfirmPassword => JsonResponseStatus.BadRequest(new { message = "رمز جدید و تکرار آن یکسان نیستند" }),
                ResetPasswordResult.Success => JsonResponseStatus.Success(new { message = "رمز عبور با موفقیت تغییر یافت" }),
                _ => JsonResponseStatus.Error(new { message = "خطا در تغییر رمز عبور" })
            };
        }



    }
}