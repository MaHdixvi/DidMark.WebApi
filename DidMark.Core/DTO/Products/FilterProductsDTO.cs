using DidMark.Core.DTO.Paging;
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
        public string Title { get; set; }
        public int StartPrice { get; set; }
        public int EndPrice { get; set; }
        public List<Product> Products { get; set; }
        public List<long> Categories { get; set; }
        public ProductOrderBy OrderBy { get; set; }
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
        public FilterProductsDTO SetProducts(List<Product> products)
        {
            this.Products = products;
            return this;
        }
        public enum ProductOrderBy
        {
            PriceAsc,
            PriceDes,
            CreateDataAsc,
            CreateDataDes,
            IsSpecial,
        }
    }
}