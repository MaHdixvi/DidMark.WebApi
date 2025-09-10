using DidMark.DataLayer.Entities.Account;
using DidMark.DataLayer.Entities.Common;
using System;
using System.Collections.Generic;

namespace DidMark.DataLayer.Entities.Orders
{
    public class Order : BaseEntity
    {
        #region properties
        public long UserId { get; set; }
        public bool IsPay { get; set; }
        public DateTime? PaymentDate { get; set; }

        // شماره تراکنش پرداخت
        public long? PaymentRefId { get; set; }

        // اگر کد تخفیف استفاده شده باشد، FK به جدول OffCode
        public long? OffCodeId { get; set; }

        // مبلغ کل بدون تخفیف (مجموع قیمت محصولات)
        public decimal Subtotal { get; set; }

        // مبلغ تخفیف کل (از OffCode)
        public decimal DiscountAmount { get; set; }

        // مبلغ نهایی پس از تخفیف
        public decimal TotalPrice { get; set; }
        #endregion

        #region relations
        public virtual User User { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public virtual OffCode? OffCode { get; set; }
        #endregion
    }
}
