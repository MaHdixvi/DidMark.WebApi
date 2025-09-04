using DidMark.DataLayer.Entities.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.Services.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateJwtToken(User user);
    }
}
