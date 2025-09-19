using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace DidMark.Core.DTO.Slider
{
    public class EditSliderDTO
    {
        [Required(ErrorMessage = "شناسه {0} الزامی است")]
        public long Id { get; set; }

        [Display(Name = "عنوان اسلایدر")]
        [MaxLength(200, ErrorMessage = "تعداد کاراکترهای {0} نمی‌تواند بیشتر از {1} باشد")]
        public string? Title { get; set; }

        [Display(Name = "توضیحات")]
        [MaxLength(500, ErrorMessage = "تعداد کاراکترهای {0} نمی‌تواند بیشتر از {1} باشد")]
        public string? Description { get; set; }

        [Display(Name = "تصویر")]
        public IFormFile? ImageUrl { get; set; }

        [Display(Name = "لینک")]
        [MaxLength(200, ErrorMessage = "تعداد کاراکترهای {0} نمی‌تواند بیشتر از {1} باشد")]
        public string? Link { get; set; }

        [Display(Name = "ترتیب نمایش")]
        [Range(0, int.MaxValue, ErrorMessage = "مقدار {0} باید عددی بزرگتر یا مساوی {1} باشد")]
        public int? DisplayOrder { get; set; }

        [Display(Name = "فعال بودن")]
        public bool? IsActive { get; set; }

        [Display(Name = "نام محصول")]
        public string ProductName { get; set; }
    }
}
