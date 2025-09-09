using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.DTO.Payment
{
    public class VerifyPaymentResponseDto
    {
        public bool Success { get; set; }
        public int RefId { get; set; } // شماره پیگیری
        public string? ErrorMessage { get; set; }
    }
}
