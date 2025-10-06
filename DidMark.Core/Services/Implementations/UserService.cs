using DidMark.Core.DTO.Account;
using DidMark.Core.DTO.User;
using DidMark.Core.DTO.Users;
using DidMark.Core.Security;
using DidMark.Core.Services.Interfaces;
using DidMark.Core.Utilities.Convertors;
using DidMark.Core.Utilities.Enums;
using DidMark.DataLayer.Entities.Access;
using DidMark.DataLayer.Entities.Account;
using DidMark.DataLayer.Entities.Orders;
using DidMark.DataLayer.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DidMark.Core.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User> _userRepository;  
        private readonly IGenericRepository<UserRole> _userRoleRepository;
        private readonly IPasswordHelper _passwordHelper;
        private readonly IMailSender _mailSender;
        private readonly IViewRenderService _viewRenderService;
        private readonly ISmsService _smsService;
        private bool _disposed;

        public UserService(
            IGenericRepository<User> userRepository,
            IGenericRepository<UserRole> userRoleRepository,
            IPasswordHelper passwordHelper,
            IMailSender mailSender,
                        ISmsService smsService,
            IViewRenderService viewRenderService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _userRoleRepository = userRoleRepository ?? throw new ArgumentNullException(nameof(userRoleRepository));
            _passwordHelper = passwordHelper ?? throw new ArgumentNullException(nameof(passwordHelper));
            _mailSender = mailSender ?? throw new ArgumentNullException(nameof(mailSender));
            _viewRenderService = viewRenderService ?? throw new ArgumentNullException(nameof(viewRenderService));
            _smsService = smsService ?? throw new ArgumentNullException(nameof(smsService));

        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            return await _userRepository.GetEntitiesQuery()
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    PhoneNumber = u.PhoneNumber,
                    IsActivated = u.IsActivated,
                    IsEmailActivated = u.IsEmailActivated,
                    IsPhoneActivated = u.IsPhoneActivated
                })
                .ToListAsync();
        }

        public async Task<RegisterUserResult> RegisterUserAsync(RegisterUserDTO register)
        {
            if (await ExistsByPhoneNumberAsync(register.PhoneNumber))
                return RegisterUserResult.PhoneNumberExists;
            if (await ExistsByUsernameAsync(register.Username))
                return RegisterUserResult.UsernameExists;

            var user = new User
            {
                PhoneNumber = register.PhoneNumber.SanitizeText(),
                EmailActiveCode = Guid.NewGuid().ToString(),
                Password = _passwordHelper.EncodePasswordMd5(register.Password),
                PhoneActiveCode = Guid.NewGuid().ToString(),
                Username = register.Username.SanitizeText(),
            };

            await _userRepository.AddEntity(user);
            await _userRepository.SaveChanges();


            var roles = await _userRoleRepository.GetEntitiesQuery()
                .Include(s => s.Role)
                .Where(s => s.Role.RoleTitle == "User")
                .Select(s => s.RoleId)
                .ToListAsync();

            if (roles.Any())
            {
                var userRoles = roles.Select(roleId => new UserRole
                {
                    UserId = user.Id,
                    RoleId = roleId
                }).ToList();

                await _userRoleRepository.AddRange(userRoles);
                await _userRoleRepository.SaveChanges();
            }
            //var body = await _viewRenderService.RenderToStringAsync("Email/ActivateAccount", user);
            //_mailSender.Send(user.Email, "Activate Your Account", body);
            _smsService.SendActivationCodeSmsAsync(user.PhoneNumber, user.PhoneActiveCode);
            return RegisterUserResult.Success;
            
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _userRepository.GetEntitiesQuery()
                .AnyAsync(s => s.Email == email.ToLower().Trim());
        }

        public async Task<bool> ExistsByPhoneNumberAsync(string phoneNumber)
        {
            return await _userRepository.GetEntitiesQuery()
                .AnyAsync(s => s.PhoneNumber == phoneNumber.ToLower().Trim());
        }

        public async Task<bool> ExistsByUsernameAsync(string username)
        {
            return await _userRepository.GetEntitiesQuery()
                .AnyAsync(s => s.Username == username.ToLower().Trim());
        }

        public async Task<LoginUserResult> LoginUserAsync(LoginUserDTO login, bool checkAdminRole = false)
        {
            if (login.UsernameOrPhone.IsNullOrEmpty())
            {
                return LoginUserResult.IncorrectData;
            }
            var password = _passwordHelper.EncodePasswordMd5(login.Password);

            User user;
            if (login.UsernameOrPhone.All(char.IsDigit))
            {
                user = await _userRepository.GetEntitiesQuery()
    .SingleOrDefaultAsync(s => s.PhoneNumber == login.UsernameOrPhone && s.Password == password);
            }
            else
            {
                // یعنی ورودی نام کاربری هست
                user = await _userRepository.GetEntitiesQuery()
    .SingleOrDefaultAsync(s => s.Username == login.UsernameOrPhone && s.Password == password);
            }


            if (user == null)
                return LoginUserResult.IncorrectData;


            if (checkAdminRole && !await IsAdminAsync(user.Id))
                return LoginUserResult.NotAdmin;

            return LoginUserResult.Success;
        }
        public async Task<LoginUserResult> LoginAdminAsync(LoginAdminDTO login)
        {
            if (login.Email.IsNullOrEmpty())
            {
                return LoginUserResult.IncorrectData;
            }
            var password = _passwordHelper.EncodePasswordMd5(login.Password);

            User user;
            
                user = await _userRepository.GetEntitiesQuery()
    .SingleOrDefaultAsync(s => s.Email == login.Email && s.Password == password);
            


            if (user == null)
                return LoginUserResult.IncorrectData;

            return LoginUserResult.Success;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetEntitiesQuery()
                .SingleOrDefaultAsync(s => s.Email.ToLower().Trim() == email.ToLower().Trim());
        }
        public async Task<User?> GetUserByPhoneNumberAsync(string phoneNumber)
        {
            return await _userRepository.GetEntitiesQuery()
                .SingleOrDefaultAsync(s => s.PhoneNumber.Trim() == phoneNumber.Trim());
        }
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _userRepository.GetEntitiesQuery()
                .SingleOrDefaultAsync(s => s.Username.ToLower().Trim() == username.ToLower().Trim());
        }

        public async Task<UserDto?> GetUserByIdAsync(long userId)
        {
            var user = await _userRepository.GetEntityById(userId);

            if (user == null)
                return null;

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                IsActivated = user.IsActivated,
                IsEmailActivated = user.IsEmailActivated,
                IsPhoneActivated = user.IsPhoneActivated
            };
        }

        public async Task<bool> ActivateUserAsync(User user)
        {
            if (user == null)
                return false;

            user.IsActivated = true;
            user.PhoneActiveCode = Guid.NewGuid().ToString();
            user.IsPhoneActivated = true;
            _userRepository.UpdateEntity(user);
            await _userRepository.SaveChanges();
            await _smsService.SendActivatedAccountSmsAsync(user.PhoneNumber);
            return true;
        }
        public async Task<bool> ActivateUserEmailAsync(User user)
        {
            if (user == null)
                return false;

            user.EmailActiveCode = Guid.NewGuid().ToString();
            user.IsEmailActivated = true;
            _userRepository.UpdateEntity(user);
            await _userRepository.SaveChanges();
            await _smsService.SendActivatedEmailSmsAsync(user.PhoneNumber);
            return true;
        }

        public async Task<User?> GetUserByActivationCodeAsync(string activationCode)
        {
            return await _userRepository.GetEntitiesQuery()
                .SingleOrDefaultAsync(s => s.PhoneActiveCode == activationCode);
        }

        public async Task<User?> GetUserByEmailActivationCodeAsync(string activationCode)
        {
            return await _userRepository.GetEntitiesQuery()
                .SingleOrDefaultAsync(s => s.EmailActiveCode == activationCode);
        }

        public async Task<EditUserResult> UpdateUserAsync(EditUserDTO user, long userId)
        {
            var existingUser = await _userRepository.GetEntityById(userId);
            if (existingUser == null)
                return EditUserResult.Error;

            if (!string.IsNullOrWhiteSpace(user.FirstName))
                existingUser.FirstName = user.FirstName.SanitizeText();

            if (!string.IsNullOrWhiteSpace(user.LastName))
                existingUser.LastName = user.LastName.SanitizeText();

            if (!string.IsNullOrWhiteSpace(user.Address))
                existingUser.Address = user.Address.SanitizeText();

            if (!string.IsNullOrWhiteSpace(user.City))
                existingUser.City = user.City.SanitizeText();

            if (!string.IsNullOrWhiteSpace(user.Province))
                existingUser.Province = user.Province.SanitizeText();
            if (!string.IsNullOrWhiteSpace(user.Email)&user.Email!=existingUser.Email) {

                if (await ExistsByEmailAsync(user.Email))
                    return EditUserResult.EmailExists;
                if (string.IsNullOrEmpty(existingUser.Email))
                {

                    existingUser.Email = user.Email.SanitizeText();
                    existingUser.IsEmailActivated = false;
                    var body = await _viewRenderService.RenderToStringAsync("Email/ActivateAccount", existingUser);
                    _mailSender.Send(user.Email, "Activate Your Account", body);
                }
                else 
                if (existingUser.Email.ToLower().Trim()!=user.Email.ToLower().Trim())
                {
                    existingUser.Email = user.Email.SanitizeText();
                    existingUser.IsEmailActivated = false;
                    var body = await _viewRenderService.RenderToStringAsync("Email/ActivateAccount", existingUser);
                    _mailSender.Send(user.Email, "Activate Your Account", body);
                }
            }
            _userRepository.UpdateEntity(existingUser);
            await _userRepository.SaveChanges();
            return EditUserResult.Success;
        }

        public async Task<bool> IsAdminAsync(long userId)
        {
            return await _userRoleRepository.GetEntitiesQuery()
                .Include(s => s.Role)
                .AnyAsync(s => s.UserId == userId && s.Role.RoleTitle == "Admin");
        }

        public async Task<ChangePasswordResult> ChangePasswordAsync(ChangePasswordDTO changePassword, long userId)
        {
            var user = await _userRepository.GetEntityById(userId);
            if (user == null)
                return ChangePasswordResult.UserNotFound;

            var currentPasswordHash = _passwordHelper.EncodePasswordMd5(changePassword.CurrentPassword);
            if (user.Password != currentPasswordHash)
                return ChangePasswordResult.IncorrectCurrentPassword;

            var newPasswordHash = _passwordHelper.EncodePasswordMd5(changePassword.NewPassword);
            if (user.Password == newPasswordHash)
                return ChangePasswordResult.SameAsOldPassword;

            if (changePassword.NewPassword != changePassword.ConfirmPassword)
                return ChangePasswordResult.NotSameNewPasswordAndConfirmPassword;

            user.Password = newPasswordHash;
            _userRepository.UpdateEntity(user);
            await _userRepository.SaveChanges();
            return ChangePasswordResult.Success;
        }
        public async Task<bool> ToggleStatusAsync(long userId)
        {
            var user = await _userRepository.GetEntityById(userId);
            if (user == null) return false;

            user.IsActivated = !user.IsActivated;
            _userRepository.UpdateEntity(user);
            await _userRepository.SaveChanges();
            return true;
        }
        public async Task<EditUserByAdminResult> UpdateUserByAdminAsync(EditUserByAdminDTO dto)
        {
            var existingUser = await _userRepository.GetEntityById(dto.Id);
            if (existingUser == null)
                return EditUserByAdminResult.Error;

            if (!string.IsNullOrWhiteSpace(dto.FirstName))
                existingUser.FirstName = dto.FirstName.SanitizeText();

            if (!string.IsNullOrWhiteSpace(dto.LastName))
                existingUser.LastName = dto.LastName.SanitizeText();

            //if (!string.IsNullOrWhiteSpace(dto.Username))
            //    existingUser.Username = dto.Username.SanitizeText();

            //if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
            //    existingUser.PhoneNumber = dto.PhoneNumber.SanitizeText();

            if (!string.IsNullOrWhiteSpace(dto.City))
                existingUser.City = dto.City.SanitizeText();

            if (!string.IsNullOrWhiteSpace(dto.Province))
                existingUser.Province = dto.Province.SanitizeText();

            if (!string.IsNullOrWhiteSpace(dto.Address))
                existingUser.Address = dto.Address.SanitizeText();
            if (!string.IsNullOrWhiteSpace(dto.Email))
            {

                if (await ExistsByUsernameAsync(dto.Email))
                    return EditUserByAdminResult.EmailExists;
                if (!string.IsNullOrEmpty(existingUser.Email))
                {

                    existingUser.Email = dto.Email.SanitizeText();
                    existingUser.IsEmailActivated = false;
                    var body = await _viewRenderService.RenderToStringAsync("Email/ActivateAccount", dto);
                    _mailSender.Send(dto.Email, "Activate Your Account", body);
                }
                else
                if (existingUser.Email.ToLower().Trim() != dto.Email.ToLower().Trim())
                {
                    existingUser.Email = dto.Email.SanitizeText();
                    existingUser.IsEmailActivated = false;
                    var body = await _viewRenderService.RenderToStringAsync("Email/ActivateAccount", dto);
                    _mailSender.Send(dto.Email, "Activate Your Account", body);
                }
            }

            _userRepository.UpdateEntity(existingUser);
            await _userRepository.SaveChanges();
            return EditUserByAdminResult.Success;
        }


        public async Task<bool> SendEmailActivationSmsAsync(long userId)
        {
            var user = await _userRepository.GetEntityById(userId);
            if (user == null)
                return false;
            var body = await _viewRenderService.RenderToStringAsync("Email/ActivateAccount", user);
            _mailSender.Send(user.Email, "Activate Your Account", body);
            return true;
        }

        public async Task<bool> SendPhoneNumberActivationSmsAsync(long userId)
        {
            var user = await _userRepository.GetEntityById(userId);
            if (user == null)
                return false;

            _smsService.SendActivationCodeSmsAsync(user.PhoneNumber, "https://localhost:4200/ActivateAccount/" + user.PhoneActiveCode);

            return true;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _userRepository?.Dispose();
            _userRoleRepository?.Dispose();
            _disposed = true;
        }

        public async Task<ForgotPasswordResult> ForgotPasswordAsync(ForgotPasswordDTO forgotPassword)
        {
            User? user = null;

            if (string.IsNullOrEmpty(forgotPassword.UsernameOrPhone))
                return ForgotPasswordResult.UserNotFound;

            if (forgotPassword.UsernameOrPhone.All(char.IsDigit))
            {
                // یعنی ورودی شماره تلفنه
                user = await GetUserByPhoneNumberAsync(forgotPassword.UsernameOrPhone);
            }
            else
            {
                // یعنی ورودی نام کاربری هست
                user = await GetUserByUsernameAsync(forgotPassword.UsernameOrPhone);
            }

            if (user == null)
                return ForgotPasswordResult.UserNotFound;

            // 🔹 ساخت کد بازیابی 6 رقمی
            var random = new Random();
            user.ResetPasswordCode = random.Next(100000, 1000000).ToString();
            user.ResetPasswordExpireDate = DateTime.UtcNow.AddMinutes(2); // ⏳ فقط دو دقیقه اعتبار داره

            _userRepository.UpdateEntity(user);
            await _userRepository.SaveChanges();

            // 🔹 ارسال به ایمیل
            if (!string.IsNullOrEmpty(user.Email))
            {
                var body = await _viewRenderService.RenderToStringAsync("Email/ForgotPassword", user);
                _mailSender.Send(user.Email, "Reset Password", body);
            }

            // 🔹 ارسال به شماره
            if (!string.IsNullOrEmpty(user.PhoneNumber))
            {
                await _smsService.SendForgotPasswordCodeSmsAsync(user.PhoneNumber, user.ResetPasswordCode);
            }

            return ForgotPasswordResult.Success;
        }


        public async Task<CheckResetCodeResult> CheckResetCodeAsync(string code)
        {
            var user = await _userRepository.GetEntitiesQuery()
                .FirstOrDefaultAsync(u => u.ResetPasswordCode == code);

            if (user == null) return CheckResetCodeResult.UserNotFound;
            if (user.ResetPasswordExpireDate < DateTime.Now) return CheckResetCodeResult.ResetPasswordExpireDatePassed;

            return CheckResetCodeResult.Success;
        }


        public async Task<ResetPasswordResult> ResetPasswordAsync(ResetPasswordDTO resetPassword)
        {
            var user = await _userRepository.GetEntitiesQuery()
                .FirstOrDefaultAsync(u => u.ResetPasswordCode == resetPassword.ResetCode);

            if (user == null)
                return ResetPasswordResult.InvalidToken;

            if (user.ResetPasswordExpireDate < DateTime.Now)
                return ResetPasswordResult.ExpiredToken;

            if (resetPassword.NewPassword != resetPassword.ConfirmPassword)
                return ResetPasswordResult.NotSameNewPasswordAndConfirmPassword;

            if (user.Password == _passwordHelper.EncodePasswordMd5(resetPassword.NewPassword))
                return ResetPasswordResult.SameAsOldPassword;
            user.Password = _passwordHelper.EncodePasswordMd5(resetPassword.NewPassword);
            user.ResetPasswordCode = null; // بعد از استفاده باطل بشه
            user.ResetPasswordExpireDate = null;

            _userRepository.UpdateEntity(user);
            await _userRepository.SaveChanges();

            return ResetPasswordResult.Success;
        }



        public async Task<bool> HasAcceptedTermsAsync(long userId)
        {
            var user = await _userRepository.GetEntityById(userId);
            return user?.HasAcceptedTerms ?? false;
        }

        public async Task<bool> AcceptTermsAsync(long userId)
        {
            var user = await _userRepository.GetEntityById(userId);
            if (user == null) return false;

            if (!user.HasAcceptedTerms)
            {
                user.HasAcceptedTerms = true;
                user.TermsAcceptedDate = DateTime.UtcNow;

                _userRepository.UpdateEntity(user);
                await _userRepository.SaveChanges();
            }

            return true;
        }
    }
}