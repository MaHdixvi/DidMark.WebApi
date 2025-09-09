using DidMark.DataLayer.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.DataLayer.Entities.Orders
{
    public class OffCodeProduct : BaseEntity
    {
        public long OffCodeId { get; set; }
        public OffCode OffCode { get; set; }

        public long ProductId { get; set; }
        public Product.Product Product { get; set; }
    }
}
