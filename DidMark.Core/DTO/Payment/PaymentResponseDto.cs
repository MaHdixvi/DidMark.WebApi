using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.DTO.Payment
{
    public class PaymentResponseDto
    {
        public bool Success { get; set; }
        public string? Authority { get; set; } // کد رهگیری زرین‌پال
        public string? GatewayUrl { get; set; } // لینک هدایت کاربر
        public string? ErrorMessage { get; set; }
    }
}
