using DidMark.DataLayer.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.DataLayer.Entities.Orders
{
    public class OffCode : BaseEntity
    {
        #region properties
        public string Code { get; set; } = string.Empty;          // خود کد تخفیف
        public decimal DiscountPercentage { get; set; }           // درصد تخفیف
        public DateTime ExpireDate { get; set; }                  // تاریخ انقضا
        #endregion

        #region relations
        // لیست سفارشاتی که از این کد استفاده کردند
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        #endregion
    }
}
