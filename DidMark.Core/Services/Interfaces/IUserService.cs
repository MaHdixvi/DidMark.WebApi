using DidMark.Core.DTO.Account;
using DidMark.Core.DTO.User;
using DidMark.Core.DTO.Users;
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
        Task<List<UserDto>> GetAllUsersAsync();
        Task<RegisterUserResult> RegisterUserAsync(RegisterUserDTO register);
        Task<bool> ExistsByEmailAsync(string email);
        Task<LoginUserResult> LoginUserAsync(LoginUserDTO login, bool checkAdminRole = false);
        Task<LoginUserResult> LoginAdminAsync(LoginAdminDTO login);

        Task<ChangePasswordResult> ChangePasswordAsync(ChangePasswordDTO changePassword, long userId);
        Task<User?> GetUserByPhoneNumberAsync(string phoneNumber);
        Task<User?> GetUserByEmailAsync(string email);
        Task<UserDto?> GetUserByIdAsync(long userId);
        Task<bool> ActivateUserAsync(User user);
        Task<User?> GetUserByActivationCodeAsync(string activationCode);
        Task<User?> GetUserByEmailActivationCodeAsync(string activationCode);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<EditUserResult> UpdateUserAsync(EditUserDTO user, long userId);
        Task<bool> IsAdminAsync(long userId);
        Task<bool> ToggleStatusAsync(long userId);
        Task<EditUserByAdminResult> UpdateUserByAdminAsync(EditUserByAdminDTO dto);
        Task<bool> ActivateUserEmailAsync(User user);
        Task<bool> SendEmailActivationSmsAsync(long userId);
        Task<bool> SendPhoneNumberActivationSmsAsync(long userId);
        Task<ForgotPasswordResult> ForgotPasswordAsync(ForgotPasswordDTO forgotPassword);
        Task<CheckResetCodeResult> CheckResetCodeAsync(string code);
        Task<ResetPasswordResult> ResetPasswordAsync(ResetPasswordDTO resetPassword);

        Task<bool> HasAcceptedTermsAsync(long userId);
        Task<bool> AcceptTermsAsync(long userId);

    }
}