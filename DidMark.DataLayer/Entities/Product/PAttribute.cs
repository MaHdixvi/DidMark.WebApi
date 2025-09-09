using DidMark.DataLayer.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.DataLayer.Entities.Product
{
    public class PAttribute : BaseEntity
    {
        #region properties
        public string Name { get; set; }

        public long CategoryId { get; set; }
        #endregion
        #region relations
        public virtual ProductCategories Category { get; set; }

        #endregion
    }
}
