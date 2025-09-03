using DidMark.Core.DTO.Account;
using DidMark.Core.Services.Interfaces;
using DidMark.Core.Utilities.Common;
using DidMark.Core.Utilities.Extensions.Identity;
using DidMark.DataLayer.Entities.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DidMark.WebApi.Controllers
{
    public class AccountController : SiteBaseController
    {
        #region costructor

        private IUserService userService;

        public AccountController(IUserService userService)
        {
            this.userService = userService;
        }

        #endregion

        #region Register

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO register)
        {
            if (ModelState.IsValid)
            {
                var res = await userService.RegisterUser(register);

                switch (res)
                {
                    case RegisterUserResult.EmailExists:
                        return JsonResponseStatus.Error(new { info = "EmailExist" });
                    case RegisterUserResult.PhoneNumberExists:
                        return JsonResponseStatus.Error(new { info = "PhoneNumberExist" });
                }
            }

            return JsonResponseStatus.Success();
        }

        #endregion

        #region Login

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO login)
        {
            if (ModelState.IsValid)
            {
                var res = await userService.LoginUser(login);

                switch (res)
                {
                    case LoginUserResult.IncorrectData:
                        return JsonResponseStatus.NotFound(new { message = "کاربری با مشخصات وارد شده یافت نشد" });

                    case LoginUserResult.NotActivated:
                        return JsonResponseStatus.Error(new { message = "حساب کاربری شما فعال نشده است" });

                    case LoginUserResult.Success:
                        var user = await userService.GetUserByEmail(login.Email);
                        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("DidmarkJwtBearer"));
                        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                        var tokenOptions = new JwtSecurityToken(
                            issuer: "https://localhost:44381",
                            claims: new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, user.Email,user.PhoneNumber),
                                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                            },
                            expires: DateTime.Now.AddDays(30),
                            signingCredentials: signinCredentials
                        );

                        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

                        return JsonResponseStatus.Success(new
                        {
                            token = tokenString,
                            expireTime = 30,
                            firstName = user.FirstName,
                            lastName = user.LastName,
                            userId = user.Id
                        });
                }
            }

            return JsonResponseStatus.Error();
        }

        #endregion

        #region Check User Authentication

        [HttpPost("check-auth")]
        public async Task<IActionResult> CheckUserAuth()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await userService.GetUserByUserId(User.GetUserId());
                return JsonResponseStatus.Success(new
                {
                    userId = user.Id,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    email = user.Email,
                    username = user.Username,
                    password = user.Password,
                    phoneNumber = user.PhoneNumber,

                });
            }

            return JsonResponseStatus.Error();
        }

        #endregion


        #region Activate User Account

        [HttpGet("activate-account/{id}")]
        public async Task<IActionResult> ActivateAccount(string id)
        {
            var user = await userService.GetUserByEmailActiveCode(id);

            if (user != null)
            {
                userService.ActivateUser(user);
                return JsonResponseStatus.Success();
            }

            return JsonResponseStatus.NotFound();
        }

        #endregion

        #region Sign Out

        [HttpGet("sign-out")]
        public async Task<IActionResult> LogOut()
        {
            if (User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync();
                return JsonResponseStatus.Success();
            }

            return JsonResponseStatus.Error();
        }

        #endregion

        #region edit user account
        [HttpPost("edit-user")]

        public async Task<IActionResult> EditUser([FromBody] EditUserDTO editUser)
        {
            Task.Delay(3000);
            if (User.Identity.IsAuthenticated)
            {
                await userService.EditUserInfo(editUser, User.GetUserId());
                return JsonResponseStatus.Success(returnData: new { message = "اطلاعات کاربر با موفقیت ویرایش شد" });
            }
            return JsonResponseStatus.Success();
        }
        #endregion
    }
}