using DidMark.DataLayer.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.DataLayer.Entities.Product
{
    public class ProductSelectedCategories : BaseEntity
    {
        #region properties
        public long ProductId { get; set; }
        public long ProductCategoriesId { get; set; }
        #endregion 

        #region Relations
        public virtual Product Product { get; set; }
        public virtual ProductCategories ProductCategories { get; set; }
        #endregion
    }
}
