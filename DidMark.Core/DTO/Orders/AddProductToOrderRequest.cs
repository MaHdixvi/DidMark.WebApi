using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.DTO.Orders
{
    public class AddProductToOrderRequest
    {
        [Required]
        public long ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Count { get; set; } = 1;
    }
}
