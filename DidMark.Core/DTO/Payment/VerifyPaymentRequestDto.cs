using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.DTO.Payment
{
    public class VerifyPaymentRequestDto
    {
        public string Authority { get; set; }
        public int Amount { get; set; }
    }
}
