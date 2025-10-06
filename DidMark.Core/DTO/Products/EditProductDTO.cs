using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.DTO.Products
{
    public class EditProductDTO
    {
        public long Id { get; set; }
        public string? CurrentImage { get; set; }

        public IFormFile? Image { get; set; }

        [Display(Name = "نام محصول")]
        [MaxLength(100, ErrorMessage = "تعداد کاراکتر های {0} نمیتواند بیشتر از {1} باشد")]
        public string? ProductName { get; set; }

        [Display(Name = "قیمت")]
        public int? Price { get; set; }

        [Display(Name = "توضیحات")]
        public string? Description { get; set; }

        [Display(Name = " توضیحات کوتاه")]
        public string? ShortDescription { get; set; }

        //[Display(Name = "رنگ")]
        //[MaxLength(100, ErrorMessage = "تعداد کاراکتر های {0} نمیتواند بیشتر از {1} باشد")]
        //public string? Color { get; set; }

        //[Display(Name = "کد")]
        //public long? Code { get; set; }

        //[Display(Name = "سایز")]
        //[MaxLength(100, ErrorMessage = "تعداد کاراکتر های {0} نمیتواند بیشتر از {1} باشد")]
        //public string? Size { get; set; }

        [Display(Name = "تعداد محصول")]
        public int? NumberofProduct { get; set; }

        [Display(Name = "موجود/به اتمام رسیده")]
        public bool? IsExists { get; set; }

        [Display(Name = "ویژه")]
        public bool? IsSpecial { get; set; }

        [Display(Name = "فعال")]
        public bool? IsActive { get; set; }
        public List<EditProductAttributeDto> Attributes { get; set; } = new();
        // 🔹 تخفیف
        [Display(Name = "درصد تخفیف")]
        public int? DiscountPercent { get; set; }

        [Display(Name = "تاریخ شروع تخفیف")]
        public DateTime? DiscountStartDate { get; set; }

        [Display(Name = "تاریخ پایان تخفیف")]
        public DateTime? DiscountEndDate { get; set; }

    }
}
