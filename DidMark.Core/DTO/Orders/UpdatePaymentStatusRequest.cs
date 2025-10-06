using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.DTO.Orders
{
    public class UpdatePaymentStatusRequest
    {
        [Required]
        public bool IsPay { get; set; }
    }
}
