using DidMark.DataLayer.Entities.Access;
using DidMark.DataLayer.Entities.Common;
using DidMark.DataLayer.Entities.Orders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.DataLayer.Entities.Account
{
    public class User : BaseEntity
    {
        #region properties

        [Display(Name = "ایمیل")]
        [MaxLength(100, ErrorMessage = "تعداد کاراکتر های {0} نمیتواند بیشتر از {1} باشد")]
        public string? Email { get; set; }

        [Display(Name = "نام کاربری")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(100, ErrorMessage = "تعداد کاراکتر های {0} نمیتواند بیشتر از {1} باشد")]
        public string Username { get; set; }


        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(100, ErrorMessage = "تعداد کاراکتر های {0} نمیتواند بیشتر از {1} باشد")]
        public string Password { get; set; }

        [Display(Name = "نام")]
        [MaxLength(100, ErrorMessage = "تعداد کاراکتر های {0} نمیتواند بیشتر از {1} باشد")]
        public string? FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        [MaxLength(100, ErrorMessage = "تعداد کاراکتر های {0} نمیتواند بیشتر از {1} باشد")]
        public string? LastName { get; set; }

        [Display(Name = "شهر")]
        [MaxLength(100, ErrorMessage = "تعداد کاراکتر های {0} نمیتواند بیشتر از {1} باشد")]
        public string? City { get; set; }

        [Display(Name = "استان")]
        [MaxLength(100, ErrorMessage = "تعداد کاراکتر های {0} نمیتواند بیشتر از {1} باشد")]
        public string? Province { get; set; }

        [Display(Name = "آدرس")]
        [MaxLength(100, ErrorMessage = "تعداد کاراکتر های {0} نمیتواند بیشتر از {1} باشد")]
        public string? Address { get; set; }

        [Display(Name = "شماره تلفن")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(100, ErrorMessage = "تعداد کاراکتر های {0} نمیتواند بیشتر از {1} باشد")]
        public string PhoneNumber { get; set; }

        [Display(Name = "تاییدیه ایمیل")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(100, ErrorMessage = "تعداد کاراکتر های {0} نمیتواند بیشتر از {1} باشد")]
        public string EmailActiveCode { get; set; }

        public bool IsEmailActivated { get; set; }

        [Display(Name = " تاییدیه کد شماره تلفن ")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(100, ErrorMessage = "تعداد کاراکتر های {0} نمیتواند بیشتر از {1} باشد")]
        public string PhoneActiveCode { get; set; }

        public bool IsPhoneActivated { get; set; }

        [Display(Name = "کد بازیابی رمز عبور")]
        [MaxLength(100, ErrorMessage = "تعداد کاراکتر های {0} نمیتواند بیشتر از {1} باشد")]
        public string? ResetPasswordCode { get; set; }

        [Display(Name = "تاریخ انقضای کد بازیابی")]
        public DateTime? ResetPasswordExpireDate { get; set; }


        #endregion

        #region Relations

        public virtual ICollection<UserRole> UserRoles { get; set; }
        public bool IsActivated { get; set; }

        public virtual ICollection<Order> Orders { get; set; }

        #endregion
    }
}