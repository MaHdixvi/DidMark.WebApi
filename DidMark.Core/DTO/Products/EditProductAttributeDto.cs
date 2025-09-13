using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.DTO.Products
{
    public class EditProductAttributeDto
    {
        public long ProductId { get; set; }
        public long PAttributeId { get; set; }
        public string Value { get; set; }
    }
}
