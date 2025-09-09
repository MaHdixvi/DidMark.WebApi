using DidMark.DataLayer.Entities.Account;
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
        // اگر بخوای محدودیت تعداد استفاده یا ارتباط به کاربر خاص بزنی، اینجا اضافه میشه
        public int? MaxUsageCount { get; set; }
        public int UsedCount { get; set; }
        // 🔹 اگر این کد تخفیف برای یک یوزر خاص باشه
        public long? UserId { get; set; }
        public virtual User User { get; set; }

        #endregion

        #region relations
        // لیست سفارشاتی که از این کد استفاده کردند
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        // 🔹 کد تخفیف برای یک محصول خاص
        public virtual ICollection<OffCodeProduct> OffCodeProducts { get; set; } = new List<OffCodeProduct>();

        // 🔹 کد تخفیف برای یک کتگوری خاص
        public virtual ICollection<OffCodeCategory> OffCodeCategories { get; set; } = new List<OffCodeCategory>();
        public virtual ICollection<OffCodeUser> UserOffCodes { get; set; } = new List<OffCodeUser>();

        #endregion
    }
}
