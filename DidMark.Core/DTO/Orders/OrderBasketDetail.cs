using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.DTO.Orders
{
    public class OrderBasketDetail
    {
        public long Id { get; set; }

        [Display(Name = "نام محصول")]
        public string ProductName { get; set; }

        [Display(Name = "رنگ")]
        public string? Color { get; set; }

        [Display(Name = "کد محصول")]
        public string? Code { get; set; }

        [Display(Name = "سایز")]
        public string? Size { get; set; }

        [Display(Name = "تعداد محصول خرید")]
        public int Count { get; set; }

        [Display(Name = "قیمت")]
        public decimal Price { get; set; }

        [Display(Name = "تصویر")]
        public string? ImageName { get; set; }

        [Display(Name = "کد تخفیف")]
        public string? OffCode { get; set; }
    }

}
