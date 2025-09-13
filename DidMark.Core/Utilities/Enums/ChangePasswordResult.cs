using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.Utilities.Enums
{
    public enum ChangePasswordResult
    {
        Success,
        IncorrectCurrentPassword,
        SameAsOldPassword,
        NotSameNewPasswordAndConfirmPassword,
        UserNotFound,
        Error
    }
    public enum ForgotPasswordResult
    {
        Success,
        UserNotFound,
        EmailNotConfirmed,
        SmsNotSent,
        Error
    }
    public enum ResetPasswordResult
    {
        Success,
        UserNotFound,
        InvalidToken,
        ExpiredToken,
        NotSameNewPasswordAndConfirmPassword,
        Error
    }
}
