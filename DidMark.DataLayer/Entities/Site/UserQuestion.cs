using DidMark.DataLayer.Entities.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace DidMark.DataLayer.Entities.Site
{
    public class UserQuestion : BaseEntity
    {
        [Display(Name = "متن سوال")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Question { get; set; }

        [Display(Name = "نام کاربر (اختیاری)")]
        [MaxLength(100)]
        public string? FullName { get; set; }

        [Display(Name = "شماره تلفن کاربر)")]
        [MaxLength(11)]
        public string PhoneNumber { get; set; } // از Claim/پروفایل میاد

        [Display(Name = "تاریخ ارسال")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Display(Name = "پاسخ داده شده / نشده")]
        public bool IsAnswered { get; set; } = false;

        [Display(Name = "پاسخ")]
        public string? Answer { get; set; }
    }
}
