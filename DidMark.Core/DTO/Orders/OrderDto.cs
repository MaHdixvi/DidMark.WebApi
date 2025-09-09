using DidMark.Core.DTO.OffCodes;
using DidMark.DataLayer.Entities.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.DTO.Orders
{
    public class OrderDto
    {
        public long Id { get; set; }
        public bool IsPay { get; set; }
        public DateTime? PaymentDate { get; set; }
        public decimal TotalPrice { get; set; } // محاسبه‌شده
        public OffCodeResultDto? OffCode { get; set; } // فقط اطلاعات لازم
        public List<OrderDetailsDto> OrderDetails { get; set; } = new List<OrderDetailsDto>();

    }
}
