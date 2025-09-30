using System;

namespace DidMark.Core.DTO.Products.ProductWishList
{
    public class ProductWishlistDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long ProductId { get; set; }
        public DateTime CreateDate { get; set; }

        // اطلاعات محصول
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public int Price { get; set; }
        public int FinalPrice { get; set; }
        public bool IsExists { get; set; }
        public bool IsSpecial { get; set; }
        public int? DiscountPercent { get; set; }
    }
}