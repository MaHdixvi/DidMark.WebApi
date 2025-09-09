using DidMark.DataLayer.Entities.Common;
using DidMark.DataLayer.Entities.Orders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.DataLayer.Entities.Product
{
    public class Product : BaseEntity
    {
        #region properties
        [Display(Name = "تصویر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(150, ErrorMessage = "تعداد کاراکتر های {0} نمیتواند بیشتر از {1} باشد")]
        public string ImageName { get; set; }


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
        //public string Color { get; set; }

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
        #region discount properties
        [Display(Name = "درصد تخفیف")]
        [Range(0, 100, ErrorMessage = "درصد تخفیف باید بین 0 تا 100 باشد")]
        public int? DiscountPercent { get; set; }

        [Display(Name = "تاریخ شروع تخفیف")]
        public DateTime? DiscountStartDate { get; set; }

        [Display(Name = "تاریخ پایان تخفیف")]
        public DateTime? DiscountEndDate { get; set; }

        [Display(Name = "قیمت نهایی بعد از تخفیف")]
        public int FinalPrice
        {
            get
            {
                if (DiscountPercent.HasValue &&
                    DiscountPercent.Value > 0 &&
                    (!DiscountStartDate.HasValue || DiscountStartDate <= DateTime.Now) &&
                    (!DiscountEndDate.HasValue || DiscountEndDate >= DateTime.Now))
                {
                    return Price - (Price * DiscountPercent.Value / 100);
                }
                return Price;
            }
        }
        #endregion

        #endregion

        #region relations
        public virtual ICollection<ProductGalleries> ProductGalleries { get; set; }
        public virtual ICollection<ProductVisit> ProductVisit { get; set; }
        public virtual ICollection<ProductSelectedCategories> ProductSelectedCategories { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual List<ProductAttribute> ProductAttributes { get; set; }

        #endregion
    }
}
