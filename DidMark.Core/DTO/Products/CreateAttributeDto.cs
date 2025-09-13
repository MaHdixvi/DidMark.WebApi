using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.DTO.Products
{
    public class CreateAttributeDto
    {
        public long CategoryId { get; set; }
        public string Name { get; set; }
    }
}
