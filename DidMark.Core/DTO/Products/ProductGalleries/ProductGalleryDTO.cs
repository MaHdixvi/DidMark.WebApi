using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.DTO.Products.ProductGalleries
{
    public class ProductGalleryDTO
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public string ImageName { get; set; }
    }
}
