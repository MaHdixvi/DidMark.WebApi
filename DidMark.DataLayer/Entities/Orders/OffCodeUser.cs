using DidMark.DataLayer.Entities.Account;
using DidMark.DataLayer.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.DataLayer.Entities.Orders
{
    public class OffCodeUser : BaseEntity
    {
        public long UserId { get; set; }
        public long OffCodeId { get; set; }

        public int UsedCount { get; set; } = 0; // تعداد دفعات استفاده توسط این کاربر
        public DateTime? AssignedDate { get; set; } // زمان اختصاص کد به کاربر

        #region relations
        public virtual OffCode OffCode { get; set; }
        public virtual User User { get; set; } // فرض بر اینه که جدول Users دارید
        #endregion
    }
}
