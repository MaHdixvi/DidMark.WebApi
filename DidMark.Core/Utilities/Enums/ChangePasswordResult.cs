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
        UserNotFound,
        Error
    }
}
