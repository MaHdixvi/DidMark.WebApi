using DidMark.Core.DTO.Products;
using DidMark.Core.DTO.Products.NewFolder;
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

    public class AdminProductController : SiteBaseController
    {
        #region Fields

        private readonly IProductService _productService;
        private readonly IProductGalleryService _productGalleryService;

        #endregion

        #region Constructor

        public AdminProductController(IProductService productService, IProductGalleryService productGalleryService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _productGalleryService = productGalleryService;
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
        public async Task<IActionResult> AddProduct([FromForm] AddProductDTO product)
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
        public async Task<IActionResult> UpdateProduct([FromForm] EditProductDTO product)
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

        [HttpPost("galleries")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> AddProductGallery([FromForm] AddProductGalleryDTO dto)
        {
            if (ModelState.IsValid)
            {
                var result = await _productGalleryService.AddProductGallery(dto);
                if (result) return JsonResponseStatus.Success();
            }
            return JsonResponseStatus.BadRequest();
        }

        [HttpDelete("galleries/{galleryId}/{productId}")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> DeleteProductGallery(long galleryId, long productId)
        {
            var result = await _productGalleryService.DeleteProductGallery(galleryId, productId);
            if (result) return JsonResponseStatus.Success();
            return JsonResponseStatus.NotFound(new { message = "گالری برای محصول مشخص یافت نشد" });
        }


        [HttpGet("{productId}/galleries")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> GetProductGalleries(long productId)
        {
            var galleries = await _productGalleryService.GetProductGalleriesByProductId(productId);
            return JsonResponseStatus.Success(galleries);
        }

    }
}