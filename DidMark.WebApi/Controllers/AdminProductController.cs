using DidMark.Core.DTO.Products;
using DidMark.Core.Services.Interfaces;
using DidMark.Core.Utilities.Common;
using DidMark.WebApi.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DidMark.WebApi.Controllers
{
    public class AdminProductController : SiteBaseController
    {

        #region constructor
        private readonly IProductService productService;
        public AdminProductController(IProductService productService)
        {
            this.productService = productService;


        }
        #endregion
        #region get product for edit
        [HttpGet("get-product-for-edit/{Id}")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> GetProductForEdit(long Id)
        {
            var product = productService.GetProductForEdit(Id);
            if (product == null)
            {
                return JsonResponseStatus.NotFound();
            }
            return JsonResponseStatus.Success(product);
        }
        #endregion
        #region edit product
        [HttpPost("edit-product")]
        public async Task<IActionResult> EditProduct([FromBody] EditProductDTO product)
        {
            if (ModelState.IsValid)
            {
                await productService.EditProduct(product);
                return JsonResponseStatus.Success();


            }
            return JsonResponseStatus.Error();
        }
        #endregion
    }


}
