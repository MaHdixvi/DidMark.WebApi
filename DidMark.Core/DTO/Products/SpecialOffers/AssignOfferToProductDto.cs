using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.DTO.Products.SpecialOffers
{
    public class AssignOfferToProductDto
    {
        public long ProductId { get; set; }
        public long OfferId { get; set; }
    }
}
