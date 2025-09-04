using DidMark.Core.DTO.Products;
using DidMark.Core.Services.Interfaces;
using DidMark.Core.Utilities.Common;
using DidMark.DataLayer.Entities.Product;
using DidMark.WebApi.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DidMark.WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AdminProductController : SiteBaseController
    {
        #region Fields

        private readonly IProductService _productService;

        #endregion

        #region Constructor

        public AdminProductController(IProductService productService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        }

        #endregion

        #region Get Product for Edit

        [HttpGet("get-product-for-edit/{id}")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> GetProductForEdit(long id)
        {
            if (id <= 0)
                return JsonResponseStatus.BadRequest(new { message = "شناسه محصول نامعتبر است" });

            var product = await _productService.GetProductForEdit(id);
            if (product == null)
                return JsonResponseStatus.NotFound(new { message = "محصول یافت نشد" });

            return JsonResponseStatus.Success(product);
        }

        #endregion



        #region Add Product

        [HttpPost("add-product")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> AddProduct([FromBody] AddProductDTO product)
        {
            if (product == null)
                return JsonResponseStatus.BadRequest(new { message = "اطلاعات محصول ارسال نشده است" });

            await _productService.AddProduct(product);
            return JsonResponseStatus.Success(new { message = "محصول با موفقیت اضافه شد" });
        }

        #endregion

        #region Update Product

        [HttpPut("update-product")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> UpdateProduct([FromBody] EditProductDTO product)
        {
            if (product == null)
                return JsonResponseStatus.BadRequest(new { message = "اطلاعات محصول ارسال نشده است" });

            await _productService.UpdateProduct(product);
            return JsonResponseStatus.Success(new { message = "محصول با موفقیت بروزرسانی شد" });
        }

        #endregion

        #region Filter Products

        [HttpPost("filter-products")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> FilterProducts([FromBody] FilterProductsDTO filter)
        {
            var result = await _productService.FilterProducts(filter);
            return JsonResponseStatus.Success(result);
        }

        #endregion

        #region Get Product By ID

        [HttpGet("get-product-by-id/{id}")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> GetProductById(long id)
        {
            var product = await _productService.GetProductById(id);
            if (product == null)
                return JsonResponseStatus.NotFound(new { message = "محصول یافت نشد" });

            return JsonResponseStatus.Success(product);
        }

        #endregion

        #region Get Related Products

        [HttpGet("get-related-products/{id}")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> GetRelatedProducts(long id)
        {
            var products = await _productService.GetRelatedProducts(id);
            return JsonResponseStatus.Success(products);
        }

        #endregion

        #region Check Product Exists

        [HttpGet("is-product-exists/{id}")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> IsExistsProductById(long id)
        {
            var exists = await _productService.IsExistsProductById(id);
            return JsonResponseStatus.Success(new { exists });
        }

        #endregion

        #region Get Product By User Order

        [HttpGet("get-product-by-user-order/{id}")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> GetProductByUserOrder(long id)
        {
            var product = await _productService.GetProductByUserOrder(id);
            if (product == null)
                return JsonResponseStatus.NotFound(new { message = "محصول یافت نشد" });

            return JsonResponseStatus.Success(product);
        }

        #endregion

        #region Get All Active Product Categories

        [HttpGet("get-active-product-categories")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> GetAllActiveProductCategories()
        {
            var categories = await _productService.GetAllActiveProductCategories();
            return JsonResponseStatus.Success(categories);
        }

        #endregion

        #region Get Product Active Galleries

        [HttpGet("get-product-active-galleries/{id}")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> GetProductActiveGalleries(long id)
        {
            var galleries = await _productService.GetProductActiveGalleries(id);
            return JsonResponseStatus.Success(galleries);
        }

        #endregion
    }
}