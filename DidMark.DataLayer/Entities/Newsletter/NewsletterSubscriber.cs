using DidMark.DataLayer.Entities.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace DidMark.DataLayer.Entities.Site
{
    public class NewsletterSubscriber : BaseEntity
    {
        #region Properties

        [Display(Name = "ایمیل")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [EmailAddress(ErrorMessage = "ایمیل معتبر نیست")]
        [MaxLength(150, ErrorMessage = "تعداد کاراکتر های {0} نمیتواند بیشتر از {1} باشد")]
        public string Email { get; set; }

        #endregion

        #region Relations

        // اگر بخواهیم ارتباطی با جدول User داشته باشیم:
        // public long? UserId { get; set; }
        // public virtual User User { get; set; }

        #endregion
    }
}
