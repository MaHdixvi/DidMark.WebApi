using DidMark.Core.DTO.Account;
using DidMark.Core.Security;
using DidMark.Core.Services.Interfaces;
using DidMark.Core.Utilities.Convertors;
using DidMark.Core.Utilities.Enums;
using DidMark.DataLayer.Entities.Access;
using DidMark.DataLayer.Entities.Account;
using DidMark.DataLayer.Repository;
using Microsoft.EntityFrameworkCore;
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
        private bool _disposed;

        public UserService(
            IGenericRepository<User> userRepository,
            IGenericRepository<UserRole> userRoleRepository,
            IPasswordHelper passwordHelper,
            IMailSender mailSender,
            IViewRenderService viewRenderService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _userRoleRepository = userRoleRepository ?? throw new ArgumentNullException(nameof(userRoleRepository));
            _passwordHelper = passwordHelper ?? throw new ArgumentNullException(nameof(passwordHelper));
            _mailSender = mailSender ?? throw new ArgumentNullException(nameof(mailSender));
            _viewRenderService = viewRenderService ?? throw new ArgumentNullException(nameof(viewRenderService));
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetEntitiesQuery().ToListAsync();
        }

        public async Task<RegisterUserResult> RegisterUserAsync(RegisterUserDTO register)
        {
            if (await ExistsByEmailAsync(register.Email))
                return RegisterUserResult.EmailExists;

            if (await ExistsByPhoneNumberAsync(register.PhoneNumber))
                return RegisterUserResult.PhoneNumberExists;

            var user = new User
            {
                Email = register.Email.SanitizeText(),
                FirstName = register.FirstName.SanitizeText(),
                LastName = register.LastName.SanitizeText(),
                EmailActiveCode = Guid.NewGuid().ToString(),
                Password = _passwordHelper.EncodePasswordMd5(register.Password),
                NationalCode = register.NationalCode.SanitizeText(),
                PhoneNumber = register.PhoneNumber.SanitizeText(),
                PhoneActiveCode = Guid.NewGuid().ToString(),
                Username = register.Username.SanitizeText(),
                IsActivated = true
            };

            await _userRepository.AddEntity(user);
            await _userRepository.SaveChanges();

            var body = await _viewRenderService.RenderToStringAsync("Email/ActivateAccount", user);
            _mailSender.Send(user.Email, "Activate Your Account", body);

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

        public async Task<LoginUserResult> LoginUserAsync(LoginUserDTO login, bool checkAdminRole = false)
        {
            var password = _passwordHelper.EncodePasswordMd5(login.Password);
            var user = await _userRepository.GetEntitiesQuery()
                .SingleOrDefaultAsync(s => s.Email == login.Email.ToLower().Trim() && s.Password == password);

            if (user == null)
                return LoginUserResult.IncorrectData;

            if (!user.IsActivated)
                return LoginUserResult.NotActivated;

            if (checkAdminRole && !await IsAdminAsync(user.Id))
                return LoginUserResult.NotAdmin;

            return LoginUserResult.Success;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetEntitiesQuery()
                .SingleOrDefaultAsync(s => s.Email == email.ToLower().Trim());
        }

        public async Task<User?> GetUserByIdAsync(long userId)
        {
            return await _userRepository.GetEntityById(userId);
        }

        public async Task<bool> ActivateUserAsync(User user)
        {
            if (user == null)
                return false;

            user.IsActivated = true;
            user.EmailActiveCode = Guid.NewGuid().ToString();
            _userRepository.UpdateEntity(user);
            await _userRepository.SaveChanges();
            return true;
        }

        public async Task<User?> GetUserByActivationCodeAsync(string activationCode)
        {
            return await _userRepository.GetEntitiesQuery()
                .SingleOrDefaultAsync(s => s.EmailActiveCode == activationCode);
        }

        public async Task<bool> UpdateUserAsync(EditUserDTO user, long userId)
        {
            var existingUser = await _userRepository.GetEntityById(userId);
            if (existingUser == null)
                return false;

            existingUser.FirstName = user.FirstName.SanitizeText();
            existingUser.LastName = user.LastName.SanitizeText();
            existingUser.Address = user.Address.SanitizeText();

            _userRepository.UpdateEntity(existingUser);
            await _userRepository.SaveChanges();
            return true;
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

            user.Password = newPasswordHash;
            _userRepository.UpdateEntity(user);
            await _userRepository.SaveChanges();
            return ChangePasswordResult.Success;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _userRepository?.Dispose();
            _userRoleRepository?.Dispose();
            _disposed = true;
        }
    }
}