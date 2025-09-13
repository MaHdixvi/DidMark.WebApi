using DidMark.Core.DTO.Products;
using DidMark.Core.DTO.Products.ProductCategory;
using DidMark.Core.Services.Interfaces;
using DidMark.Core.Utilities.Common;
using DidMark.WebApi.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DidMark.WebApi.Controllers
{

    public class AdminProductCategoryController : SiteBaseController
    {
        private readonly IProductCategoryService _categoryService;

        public AdminProductCategoryController(IProductCategoryService categoryService)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        }

        #region Category CRUD

        [HttpGet("get-all")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllCategories();
            return JsonResponseStatus.Success(categories);
        }

        [HttpGet("get/{id}")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> Get(long id)
        {
            var category = await _categoryService.GetCategoryById(id);
            if (category == null)
                return JsonResponseStatus.NotFound(new { message = "دسته‌بندی یافت نشد" });

            return JsonResponseStatus.Success(category);
        }

        [HttpPost("add")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> Add([FromBody] AddProductCategoryDTO dto)
        {
            await _categoryService.AddCategory(dto);
            return JsonResponseStatus.Success(new { message = "دسته‌بندی اضافه شد" });
        }

        [HttpPut("update")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> Update([FromBody] EditProductCategoryDTO dto)
        {
            await _categoryService.UpdateCategory(dto);
            return JsonResponseStatus.Success(new { message = "دسته‌بندی بروزرسانی شد" });
        }

        [HttpDelete("delete/{id}")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _categoryService.DeleteCategory(id);
            if (!result)
                return JsonResponseStatus.NotFound(new { message = "دسته‌بندی یافت نشد یا قبلا حذف شده است" });

            return JsonResponseStatus.Success(new { message = "دسته‌بندی حذف شد" });
        }
        [HttpGet("get-root-categories")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> GetRootCategories()
        {
            var categories = await _categoryService.GetRootCategories();
            return JsonResponseStatus.Success(categories);
        }

        [HttpGet("get-child-categories/{parentId}")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> GetChildCategories(long parentId)
        {
            var categories = await _categoryService.GetChildCategories(parentId);
            return JsonResponseStatus.Success(categories);
        }

        #endregion

        #region Product-Category Operations

        [HttpGet("get-product-categories/{productId}")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> GetProductCategories(long productId)
        {
            var categories = await _categoryService.GetCategoriesOfProduct(productId);
            return JsonResponseStatus.Success(categories);
        }

        [HttpPost("add-category-to-product")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> AddCategoryToProduct([FromQuery] long productId, [FromQuery] long categoryId)
        {
            await _categoryService.AddCategoryToProduct(productId, categoryId);
            return JsonResponseStatus.Success(new { message = "دسته‌بندی به محصول اضافه شد" });
        }

        [HttpPut("update-product-categories/{productId}")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> UpdateProductCategories(long productId, [FromBody] List<long> categoryIds)
        {
            await _categoryService.UpdateProductCategories(productId, categoryIds);
            return JsonResponseStatus.Success(new { message = "دسته‌بندی‌های محصول بروزرسانی شد" });
        }

        [HttpDelete("remove-category-from-product")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> RemoveCategoryFromProduct([FromQuery] long productId, [FromQuery] long categoryId)
        {
            await _categoryService.RemoveCategoryFromProduct(productId, categoryId);
            return JsonResponseStatus.Success(new { message = "دسته‌بندی از محصول حذف شد" });
        }

        [HttpGet("is-product-in-category")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> IsProductInCategory([FromQuery] long productId, [FromQuery] long categoryId)
        {
            var exists = await _categoryService.IsProductInCategory(productId, categoryId);
            return JsonResponseStatus.Success(new { exists });
        }

        #endregion
    }
}
