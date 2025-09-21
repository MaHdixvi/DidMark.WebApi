using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.DTO.Products.SpecialOffers
{
    public class EditOfferProductDto
    {
        public long Id { get; set; } // ID رکورد SpecialOfferProduct
        public long NewOfferId { get; set; }
    }
}
