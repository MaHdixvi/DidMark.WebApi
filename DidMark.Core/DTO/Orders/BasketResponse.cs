using DidMark.Core.DTO.OffCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.DTO.Orders
{
    public class BasketResponse
    {
        public List<OrderBasketDetail> Items { get; set; } = new List<OrderBasketDetail>();
        public decimal Subtotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalPrice { get; set; }
        public int TotalItems => Items.Sum(item => item.Count);
        public bool HasDiscount => DiscountAmount > 0;

        // اضافه شده: اطلاعات کد تخفیف اگر اعمال شده باشد
        public ApplyOffCodeDto? AppliedOffCode { get; set; }
    }
}
