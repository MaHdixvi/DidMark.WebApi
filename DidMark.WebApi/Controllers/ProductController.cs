using DidMark.Core.DTO.Products;
using DidMark.Core.Services.Interfaces;
using DidMark.Core.Utilities.Common;
using Microsoft.AspNetCore.Mvc;

namespace DidMark.WebApi.Controllers
{
    public class ProductController : SiteBaseController
    {

        #region constructor

        private IProductService productService;

        public ProductController(IProductService productService)
        {
            this.productService = productService;
        }
        #endregion

        #region products
        [HttpGet("filter-products")]
        public async Task<IActionResult> GetProducts([FromQuery] FilterProductsDTO filter)
        {
            filter.TakeEntity = 12;
            var products = await productService.FilterProducts(filter);


            return JsonResponseStatus.Success(products);
        }
        #endregion

        #region Get products catagories
        [HttpGet("product-active-categories")]
        public async Task<IActionResult> GetProductCategories()
        {
            return JsonResponseStatus.Success(await productService.GetAllActiveProductCategories());
        }

        #endregion
        #region Get Single Product

        [HttpGet(template: "single-product/{id}")]
        public async Task<IActionResult> GetProduct(long id)
        {
            var Product = await productService.GetProductById(id);
            var ProductGalleries = await productService.GetProductActiveGalleries(id);

            if (Product != null)
                return JsonResponseStatus.Success(returnData: new { product = Product, galleries = ProductGalleries });

            return JsonResponseStatus.NotFound();
        }
        #endregion
        #region relatedproduct
        [HttpGet("related-products/{Id}")]

        public async Task<IActionResult> GetRelatedProducts(long Id)
        {
            var relatedproducts = await productService.GetRelatedProducts(Id);
            return JsonResponseStatus.Success(relatedproducts);
        }
        #endregion

    }
}
