using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.DTO.Newsletter
{
    public class AddNewsletterDTO
    {
        [Required(ErrorMessage = "ایمیل الزامی است")]
        [EmailAddress(ErrorMessage = "فرمت ایمیل معتبر نیست")]
        [MaxLength(150, ErrorMessage = "ایمیل نمی‌تواند بیشتر از 150 کاراکتر باشد")]
        public string Email { get; set; }
    }
}
