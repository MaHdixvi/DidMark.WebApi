using DidMark.Core.DTO.Paging;
using DidMark.Core.DTO.Products.ProductCategory;
using DidMark.DataLayer.Entities.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.DTO.Products
{
    public class FilterProductsDTO : BasePaging
    {
        public string? Title { get; set; }
        public int StartPrice { get; set; }
        public int EndPrice { get; set; }
        public List<ProductDto>? Products { get; set; }
        public List<long>? Categories { get; set; }
        public ProductOrderBy OrderBy { get; set; }
        public bool OnlyDiscounted { get; set; } = false;

        public FilterProductsDTO SetPaging(BasePaging paging)
        {
            this.PageId = paging.PageId;
            this.ActivePage = paging.ActivePage;
            this.EndPage = paging.EndPage;
            this.PageCount = paging.PageCount;
            this.StartPage = paging.StartPage;
            this.TakeEntity = paging.TakeEntity;
            this.SkipEntity = paging.SkipEntity;
            return this;
        }
        //public FilterProductsDTO SetProducts(List<Product> products)
        //{
        //    this.Products = products;
        //    return this;
        //}
        public FilterProductsDTO SetProducts(List<Product> products)
        {
            this.Products = products?.Select(p => new ProductDto
            {
                Id = p.Id,
                ProductName = p.ProductName,
                Description = p.Description,
                ShortDescription = p.ShortDescription,
                Price = p.Price,
                NumberofProduct = p.NumberofProduct,
                IsExists = p.IsExists,
                IsSpecial = p.IsSpecial,
                ImageName = p.ImageName,
                Attributes = p.ProductAttributes?.Select(pa => new ProductAttributeDto
                {
                    PAttributeId = pa.PAttributeId,
                    Value = pa.Value,
                    Name = pa.PAttribute.Name
                }).ToList(),
                Categories = p.ProductSelectedCategories?
    .Where(pc => pc.ProductCategories != null)  // فقط موارد معتبر
    .Select(pc => new ProductCategoryDTO
    {
        Id = pc.ProductCategoriesId,
        Title = pc.ProductCategories!.Title,      // علامت ! ایمنی بعد از Where
        ParentId = pc.ProductCategories.ParentId,
        UrlTitle = pc.ProductCategories.UrlTitle
    }).ToList(),

                Galleries = p.ProductGalleries?.Where(g => !g.IsDelete).Select(g => g.ImageName).ToList(),
                DiscountEndDate = p.DiscountEndDate,
                DiscountPercent = p.DiscountPercent,
                DiscountStartDate = p.DiscountStartDate,
                FinalPrice = p.FinalPrice
            }).ToList();

            return this;
        }

        public enum ProductOrderBy
        {
            PriceAsc,
            PriceDes,
            CreateDateAsc,
            CreateDateDes,
            IsSpecial,
            MaxDiscount // 🔹 اضافه شد
        }
    }
}