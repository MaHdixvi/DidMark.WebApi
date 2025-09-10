using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.DTO.User
{
    public class UserDto
    {
        public long Id { get; set; }            // از BaseEntity ارث‌بری می‌کنه
        public string Username { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActivated { get; set; }
        public bool IsEmailActivated { get; set; }
        public bool IsPhoneActivated { get; set; }

        // اگر نیاز داشتی نام کامل داشته باشی
        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}
