using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.DTO.OffCodes
{
    public class OffCodeAdminDto
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime ExpireDate { get; set; }
        public int? MaxUsageCount { get; set; }
        public int UsedCount { get; set; }
        public bool IsActive => ExpireDate >= DateTime.Now && (MaxUsageCount == null || UsedCount < MaxUsageCount);
        public bool IsDelete { get; set; }
        public long? UserId { get; set; }


        // 🔹 کاربران مرتبط
        public List<OffCodeUserDTO> Users { get; set; } = new();

        // 🔹 محصولات مرتبط
        public List<OffCodeProductDTO> Products { get; set; } = new();

        // 🔹 دسته‌بندی‌های مرتبط
        public List<OffCodeCategoryDTO> Categories { get; set; } = new();
    }

}
