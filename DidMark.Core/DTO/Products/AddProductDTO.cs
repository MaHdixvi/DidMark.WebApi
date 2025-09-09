using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.DTO.Products
{
    public class AddProductDTO
    {
        [Display(Name = "تصویر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public IFormFile ImageName { get; set; }


        [Display(Name = "نام محصول")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(100, ErrorMessage = "تعداد کاراکتر های {0} نمیتواند بیشتر از {1} باشد")]
        public string ProductName { get; set; }

        [Display(Name = "قیمت")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]

        public int Price { get; set; }

        [Display(Name = "توضیحات")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Description { get; set; }

        [Display(Name = " توضیحات کوتاه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string ShortDescription { get; set; }


        //[Display(Name = "رنگ")]
        //[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        //[MaxLength(100, ErrorMessage = "تعداد کاراکتر های {0} نمیتواند بیشتر از {1} باشد")]
        //public required string Color { get; set; }

        //[Display(Name = "کد")]
        //[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        //public long Code { get; set; }

        //[Display(Name = "سایز")]
        //[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        //[MaxLength(100, ErrorMessage = "تعداد کاراکتر های {0} نمیتواند بیشتر از {1} باشد")]
        //public string Size { get; set; }

        [Display(Name = "تعداد محصول")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int NumberofProduct { get; set; }

        [Display(Name = "موجود/به اتمام رسیده")]
        public bool IsExists { get; set; }

        [Display(Name = "ویژه")]
        public bool IsSpecial { get; set; }
        public List<CreateProductAttributeDto> Attributes { get; set; } = new();
        // 🔹 تخفیف
        // 🔹 تخفیف (اختیاری)
        [Display(Name = "درصد تخفیف")]
        public int? DiscountPercent { get; set; }

        [Display(Name = "تاریخ شروع تخفیف")]
        public DateTime? DiscountStartDate { get; set; }

        [Display(Name = "تاریخ پایان تخفیف")]
        public DateTime? DiscountEndDate { get; set; }

    }
}
