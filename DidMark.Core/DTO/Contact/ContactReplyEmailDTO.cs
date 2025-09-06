using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.DTO.Contact
{
    public class ContactReplyEmailDTO
    {
        /// <summary>
        /// ایمیل کاربر که پیام را ارسال کرده
        /// </summary>
        [Required]
        [EmailAddress(ErrorMessage = "ایمیل وارد شده معتبر نیست")]
        public string UserEmail { get; set; }

        /// <summary>
        /// نام کامل کاربر
        /// </summary>
        [Required]
        [MaxLength(150)]
        public string FullName { get; set; }

        /// <summary>
        /// عنوان پیام اصلی یا موضوع پاسخ
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        /// <summary>
        /// متن پاسخ
        /// </summary>
        [Required]
        public string ReplyMessage { get; set; }
    }
}
