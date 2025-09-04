using DidMark.DataLayer.Entities.Account;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.Security
{
    public class JwtSettings
    {
        [Required]
        public string SecretKey { get; set; }

        [Required]
        public string ValidIssuer { get; set; }

        public string ValidAudience { get; set; }

        [Range(1, 365)]
        public int ExpireDays { get; set; } = 7;

        [Range(1, 1440)]
        public int ExpireMinutes { get; set; } = 60;

    }

}
