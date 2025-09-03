using DidMark.Core.DTO.Account;
using DidMark.Core.Security;
using DidMark.Core.Services.Interfaces;
using DidMark.Core.Utilities.Convertors;
using DidMark.DataLayer.Entities.Access;
using DidMark.DataLayer.Entities.Account;
using DidMark.DataLayer.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.Services.Implementations
{
    public class UserService : IUserService
    {
        #region constructor

        private IGenericRepository<User> userRepository;
        private IPasswordHelper passwordHelper;
        private IMailSender mailSender;
        private IViewRenderService renderView;
        private IGenericRepository<UserRole> userRoleRepository;

        public UserService(IGenericRepository<User> userRepository, IPasswordHelper passwordHelper, IMailSender mailSender, IViewRenderService renderView, IGenericRepository<UserRole> userRoleRepository)
        {
            this.userRepository = userRepository;
            this.passwordHelper = passwordHelper;
            this.mailSender = mailSender;
            this.renderView = renderView;
            this.userRoleRepository = userRoleRepository;

        }

        #endregion

        #region User Section

        public async Task<List<User>> GetAllUsers()
        {
            return await userRepository.GetEntitiesQuery().ToListAsync();
        }

        public async Task<RegisterUserResult> RegisterUser(RegisterUserDTO register)
        {
            if (IsUserExistsByEmail(register.Email))
                return RegisterUserResult.EmailExists;
            if (IsUserExistsByPhoneNumber(register.PhoneNumber))
                return RegisterUserResult.PhoneNumberExists;
            var user = new User
            {
                Email = register.Email.SanitizeText(),
                FirstName = register.FirstName.SanitizeText(),
                LastName = register.LastName.SanitizeText(),
                EmailActiveCode = Guid.NewGuid().ToString(),
                Password = passwordHelper.EncodePasswordMd5(register.Password),
                NationalCode = register.NationalCode.SanitizeText(),
                PhoneNumber = register.PhoneNumber.SanitizeText(),
                PhoneActiveCode = Guid.NewGuid().ToString(),
            };

            await userRepository.AddEntity(user);

            await userRepository.SaveChanges();

            var body = await renderView.RenderToStringAsync("Email/ActivateAccount", user);

            mailSender.Send(user.Email, "test", body);

            return RegisterUserResult.Success;
        }

        private bool IsUserExistsByPhoneNumber(string phoneNumber)
        {
            return userRepository.GetEntitiesQuery().Any(s => s.PhoneNumber == phoneNumber.ToLower().Trim());
        }

        public bool IsUserExistsByEmail(string email)
        {
            return userRepository.GetEntitiesQuery().Any(s => s.Email == email.ToLower().Trim());
        }

        public async Task<LoginUserResult> LoginUser(LoginUserDTO login, bool checkAdminRole = false)
        {
            var password = passwordHelper.EncodePasswordMd5(login.Password);

            var user = await userRepository.GetEntitiesQuery()
                .SingleOrDefaultAsync(s => s.Email == login.Email.ToLower().Trim() && s.Password == password);

            if (user == null) return LoginUserResult.IncorrectData;

            if (!user.IsActivated) return LoginUserResult.NotActivated;

            if (checkAdminRole)
            {
                var IsUserAdmin = await userRoleRepository.GetEntitiesQuery()
                    .Include(s => s.Role)
                    .AsQueryable().AnyAsync(s => s.UserId == user.Id && s.Role.RoleName == "Admin");

                if (!IsUserAdmin)
                {
                    return LoginUserResult.NotAdmin;
                }
            }

            return LoginUserResult.Success;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await userRepository.GetEntitiesQuery().SingleOrDefaultAsync(s => s.Email == email.ToLower().Trim());
        }
        public async Task<User> GetUserByUserId(long userId)
        {
            return await userRepository.GetEntityById(userId);
        }

        public void ActivateUser(User user)
        {
            user.IsActivated = true;
            user.EmailActiveCode = Guid.NewGuid().ToString();
            userRepository.UpdateEntity(user);
            userRepository.SaveChanges();
        }

        public Task<User> GetUserByEmailActiveCode(string emailActiveCode)
        {
            return userRepository.GetEntitiesQuery().SingleOrDefaultAsync(s => s.EmailActiveCode == emailActiveCode);
        }

        #endregion


        public async Task EditUserInfo(EditUserDTO user, long userId)
        {
            var mainuser = await userRepository.GetEntityById(userId);
            if (mainuser != null)
            {
                mainuser.FirstName = user.FirstName;
                mainuser.LastName = user.LastName;
                mainuser.Address = user.Address;
                userRepository.UpdateEntity(mainuser);
                await userRepository.SaveChanges();
            }
        }

        public async Task<bool> IsUserAdmin(long userId)
        {
            return await userRoleRepository.GetEntitiesQuery()
                    .Include(s => s.Role)
                    .AsQueryable().AnyAsync(s => s.UserId == userId && s.Role.RoleName == "Admin");
        }
        #region dispose

        public void Dispose()
        {
            userRepository?.Dispose();
        }
        #endregion
    }
}