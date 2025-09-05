using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.DTO.Products.ProductCategory
{
    public class ProductCategoryDTO
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string UrlTitle { get; set; }
        public long? ParentId { get; set; }
    }
}
