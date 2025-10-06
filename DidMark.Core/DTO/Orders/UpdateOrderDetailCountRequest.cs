using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.DTO.Orders
{
    public class UpdateOrderDetailCountRequest
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int NewCount { get; set; }
    }
}
