using DidMark.Core.DTO.Products.ProductCategory;
using DidMark.Core.DTO.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.DataLayer.Entities.Product
{
    public class ProductDto
    {
        public long Id { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public int Price { get; set; }
        public int NumberofProduct { get; set; }
        public bool IsExists { get; set; }
        public bool IsSpecial { get; set; }
        public string ImageName { get; set; }

        // ویژگی‌های محصول
        public List<ProductAttributeDto> Attributes { get; set; } = new();

        // دسته‌های محصول
        public List<ProductCategoryDTO> Categories { get; set; } = new();

        // گالری تصاویر محصول
        public List<string> Galleries { get; set; } = new();
        // 🔹 تخفیف
        public int? DiscountPercent { get; set; }
        public DateTime? DiscountStartDate { get; set; }
        public DateTime? DiscountEndDate { get; set; }
        public int FinalPrice { get; set; }

    }
}
