﻿using DidMark.DataLayer.Entities.Common;
using DidMark.DataLayer.Entities.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.DataLayer.Entities.Orders
{
    public class OffCodeCategory : BaseEntity
    {
        public long OffCodeId { get; set; }
        public OffCode OffCode { get; set; }

        public long CategoryId { get; set; }
        public ProductCategories Category { get; set; }
    }

}
