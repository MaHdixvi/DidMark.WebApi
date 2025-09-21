using DidMark.DataLayer.Entities.Common;
using DidMark.DataLayer.Entities.Product;
using System;
using System.Collections.Generic;

namespace DidMark.DataLayer.Entities.Offers
{
    public class SpecialOffer : BaseEntity
    {
        public string Title { get; set; }              // عنوان مثل "تخفیف پایان فصل"
        public string Description { get; set; }       // توضیحات مثل "تا 50% تخفیف..."
        public DateTime StartDate { get; set; }       // تاریخ شروع
        public DateTime EndDate { get; set; }         // تاریخ پایان
        public int? DiscountPercent { get; set; }     // درصد تخفیف کلی (اختیاری، اگه بخوای به همه محصولات این پیشنهاد اعمال بشه)

        // ارتباط با محصولات
        public virtual ICollection<SpecialOfferProduct> SpecialOfferProducts { get; set; } = new List<SpecialOfferProduct>();
    }
}
