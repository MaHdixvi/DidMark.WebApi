using DidMark.Core.DTO.Products;
using DidMark.Core.DTO.Products.ProductCategory;
using DidMark.Core.Services.Interfaces;
using DidMark.Core.Utilities.Common;
using DidMark.WebApi.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DidMark.WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProductController : SiteBaseController
    {
        #region Fields
        private readonly IProductService _productService;
        private readonly IProductCategoryService _categoryService;

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductController"/> class.
        /// </summary>
        /// <param name="productService">The product service for handling product-related operations.</param>
        public ProductController(IProductService productService, IProductCategoryService categoryService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        }
        #endregion

        #region Filter Products
        /// <summary>
        /// Retrieves a list of products based on the provided filter criteria.
        /// </summary>
        /// <param name="filter">The filter criteria for products.</param>
        /// <returns>A JSON response containing the filtered products.</returns>
        [HttpGet("filter-products")]
        public async Task<IActionResult> GetProducts([FromQuery] FilterProductsDTO filter)
        {
            if (filter == null)
            {
                return JsonResponseStatus.BadRequest(new { message = "داده‌های فیلتر ارائه نشده است" });
            }

            filter.TakeEntity = 12; // Limit the number of products returned
            var products = await _productService.FilterProducts(filter);
            return JsonResponseStatus.Success(products);
        }
        #endregion

        

        #region Get Single Product
        /// <summary>
        /// Retrieves a single product and its active galleries by product ID.
        /// </summary>
        /// <param name="id">The ID of the product.</param>
        /// <returns>A JSON response containing the product and its galleries, or a not found status.</returns>
        [HttpGet("single-product/{id}")]
        public async Task<IActionResult> GetProduct(long id)
        {
            if (id <= 0)
            {
                return JsonResponseStatus.BadRequest(new { message = "شناسه محصول نامعتبر است" });
            }

            var product = await _productService.GetProductById(id);
            if (product == null)
            {
                return JsonResponseStatus.NotFound(new { message = "محصول یافت نشد" });
            }

            var productGalleries = await _productService.GetProductActiveGalleries(id);
            return JsonResponseStatus.Success(new { product, galleries = productGalleries });
        }
        #endregion

        #region Get Related Products
        /// <summary>
        /// Retrieves related products for a given product ID.
        /// </summary>
        /// <param name="id">The ID of the product.</param>
        /// <returns>A JSON response containing the related products.</returns>
        [HttpGet("related-products/{id}")]
        public async Task<IActionResult> GetRelatedProducts(long id)
        {
            if (id <= 0)
            {
                return JsonResponseStatus.BadRequest(new { message = "شناسه محصول نامعتبر است" });
            }

            var relatedProducts = await _productService.GetRelatedProducts(id);
            return JsonResponseStatus.Success(relatedProducts);
        }
        #endregion
        #region
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllActiveCategories();
            return JsonResponseStatus.Success(categories);
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var category = await _categoryService.GetCategoryById(id);
            if (category == null)
                return JsonResponseStatus.NotFound(new { message = "دسته‌بندی یافت نشد" });

            return JsonResponseStatus.Success(category);
        }

        [HttpGet("get-root-categories")]
        public async Task<IActionResult> GetRootCategories()
        {
            var categories = await _categoryService.GetRootCategories();
            return JsonResponseStatus.Success(categories);
        }

        [HttpGet("get-child-categories/{parentId}")]
        public async Task<IActionResult> GetChildCategories(long parentId)
        {
            var categories = await _categoryService.GetChildCategories(parentId);
            return JsonResponseStatus.Success(categories);
        }



        [HttpGet("get-product-categories/{productId}")]
        public async Task<IActionResult> GetProductCategories(long productId)
        {
            var categories = await _categoryService.GetCategoriesOfProduct(productId);
            return JsonResponseStatus.Success(categories);
        }


        #endregion
    }
}
