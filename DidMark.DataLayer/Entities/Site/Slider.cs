using DidMark.DataLayer.Entities.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace DidMark.DataLayer.Entities.Site
{
    public class Slider : BaseEntity
    {
        #region Properties

        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "تعداد کاراکتر های {0} نمی‌تواند بیشتر از {1} باشد")]
        public string Title { get; set; }                     // قبلاً ProductName

        [Display(Name = "توضیحات")]
        [MaxLength(500, ErrorMessage = "تعداد کاراکتر های {0} نمی‌تواند بیشتر از {1} باشد")]
        public string? Description { get; set; }             // اختیاری

        [Display(Name = "تصویر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(150, ErrorMessage = "تعداد کاراکتر های {0} نمیتواند بیشتر از {1} باشد")]
        public string ImageUrl { get; set; }                  // قبلاً Image

        [Display(Name = "لینک")]
        [MaxLength(200, ErrorMessage = "تعداد کاراکتر های {0} نمی‌تواند بیشتر از {1} باشد")]
        public string? Link { get; set; }                     // اختیاری

        [Display(Name = "ترتیب نمایش")]
        public int DisplayOrder { get; set; } = 0;           // اختیاری

        [Display(Name = "فعال بودن")]
        public bool IsActive { get; set; } = true;           // اختیاری


        [Display(Name = "نام محصول")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(100, ErrorMessage = "تعداد کاراکتر های {0} نمیتواند بیشتر از {1} باشد")]
        public string ProductName { get; set; }

        #endregion
    }
}
