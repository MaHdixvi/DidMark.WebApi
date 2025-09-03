using DidMark.Core.DTO.Account;
using DidMark.DataLayer.Entities.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.Services.Interfaces
{
    public interface IUserService : IDisposable
    {
        Task<List<User>> GetAllUsers();

        Task<RegisterUserResult> RegisterUser(RegisterUserDTO register);
        bool IsUserExistsByEmail(string email);
        Task<LoginUserResult> LoginUser(LoginUserDTO login, bool checkAdminRole = false);
        Task<User> GetUserByEmail(string email);


        Task<User> GetUserByUserId(long userId);

        void ActivateUser(User user);
        Task<User> GetUserByEmailActiveCode(string emailActiveCode);
        Task EditUserInfo(EditUserDTO user, long userId);
        Task<bool> IsUserAdmin(long userId);
    }
}