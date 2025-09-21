using DidMark.DataLayer.Entities.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.DTO.Products.SpecialOffers
{
    public class SpecialOfferDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? DiscountPercent { get; set; }
        public bool IsActive { get; set; }
        //public List<ProductDto> Products { get; set; } = new();/
    }


}
