using DidMark.DataLayer.Entities.Account;
using DidMark.DataLayer.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.DataLayer.Entities.Orders
{
    public class Order : BaseEntity
    {
        #region properties
        public long UserId { get; set; }
        public bool IsPay { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string? OffCode { get; set; }
        #endregion
        #region relations
        public virtual User User { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        #endregion
    }
}
