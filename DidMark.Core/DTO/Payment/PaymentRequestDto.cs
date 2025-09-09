using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.DTO.Payment
{
    public class PaymentRequestDto
    {
        public long OrderId { get; set; }
        public int Amount { get; set; } // مبلغ ریالی
        public string CallbackUrl { get; set; } // آدرس بازگشت
        public string Description { get; set; }
    }
}
