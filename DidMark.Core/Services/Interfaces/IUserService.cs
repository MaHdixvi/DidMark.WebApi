using DidMark.Core.DTO.Account;
using DidMark.Core.Utilities.Enums;
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
        Task<List<User>> GetAllUsersAsync();
        Task<RegisterUserResult> RegisterUserAsync(RegisterUserDTO register);
        Task<bool> ExistsByEmailAsync(string email);
        Task<LoginUserResult> LoginUserAsync(LoginUserDTO login, bool checkAdminRole = false);
        Task<ChangePasswordResult> ChangePasswordAsync(ChangePasswordDTO changePassword, long userId);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(long userId);
        Task<bool> ActivateUserAsync(User user);
        Task<User?> GetUserByActivationCodeAsync(string activationCode);
        Task<bool> UpdateUserAsync(EditUserDTO user, long userId);
        Task<bool> IsAdminAsync(long userId);
    }
}