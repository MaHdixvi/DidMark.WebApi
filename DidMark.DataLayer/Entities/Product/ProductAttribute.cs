using DidMark.DataLayer.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.DataLayer.Entities.Product
{
    public class ProductAttribute : BaseEntity
    {
        #region properties

        public long ProductId { get; set; }
        public long PAttributeId { get; set; }
        public string Value { get; set; }


        #endregion
        #region relations

        public PAttribute PAttribute { get; set; }
        public Product Product { get; set; }


        #endregion
    }
}
