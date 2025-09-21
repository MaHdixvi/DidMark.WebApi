using DidMark.DataLayer.Entities.Common;
using DidMark.DataLayer.Entities.Product;

namespace DidMark.DataLayer.Entities.Offers
{
    public class SpecialOfferProduct : BaseEntity
    {
        public long SpecialOfferId { get; set; }
        public SpecialOffer SpecialOffer { get; set; }

        public long ProductId { get; set; }
        public Product.Product Product { get; set; }
    }
}
