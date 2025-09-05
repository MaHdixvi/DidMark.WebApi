using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace DidMark.Core.DTO.Slider
{
    public class AddSliderDTO
    {
        [Display(Name = "نام محصول")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "تعداد کاراکتر های {0} نمی‌تواند بیشتر از {1} باشد")]
        public string ProductName { get; set; }

        [Display(Name = "توضیحات")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(500, ErrorMessage = "تعداد کاراکتر های {0} نمی‌تواند بیشتر از {1} باشد")]
        public string Description { get; set; }

        [Display(Name = "تصویر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public IFormFile Image { get; set; }

        public bool? IsActive { get; set; } = true;
    }
}
