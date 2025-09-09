using DidMark.Core.DTO.OffCodes;
using DidMark.Core.Services.Interfaces;
using DidMark.Core.Utilities.Common;
using DidMark.WebApi.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DidMark.WebApi.Controllers
{
    public class AdminOffCodeController : SiteBaseController
    {
        private readonly IOffCodeService _offCodeService;

        public AdminOffCodeController(IOffCodeService offCodeService)
        {
            _offCodeService = offCodeService;
        }

        [HttpGet("get-all")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> GetAll()
        {
            var codes = await _offCodeService.GetAllOffCodesAsync();
            return JsonResponseStatus.Success(codes);
        }

        [HttpPost("add")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> Create([FromBody] CreateOffCodeDto dto)
        {
            if (!ModelState.IsValid)
                return JsonResponseStatus.BadRequest(ModelState);

            await _offCodeService.AddOffCodeAsync(dto);
            return JsonResponseStatus.Success(new { message = "کد تخفیف با موفقیت ایجاد شد" });
        }

        [HttpPost("edit")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> Edit([FromBody] UpdateOffCodeDto dto)
        {
            if (!ModelState.IsValid)
                return JsonResponseStatus.BadRequest(ModelState);

            await _offCodeService.EditOffCodeAsync(dto);
            return JsonResponseStatus.Success(new { message = "کد تخفیف با موفقیت ایجاد شد" });
        }

        [HttpDelete("delete/{id}")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> Delete(long id)
        {
            await _offCodeService.DeleteOffCodeAsync(id);
            return JsonResponseStatus.Success(new { message = "کد تخفیف حذف شد" });
        }

        [HttpPost("product-offcode-add")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> CreateOffCodeProduct([FromBody] AddOffCodeProductDto dto)
        {
            if (!ModelState.IsValid)
                return JsonResponseStatus.BadRequest(ModelState);

            await _offCodeService.AddOffCodeProductAsync(dto);
            return JsonResponseStatus.Success(new { message = "کد تخفیف با موفقیت ایجاد شد" });
        }

        [HttpPost("product-offcode-edit")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> EditOffCodeProduct([FromBody] EditOffCodeProductDto dto)
        {
            if (!ModelState.IsValid)
                return JsonResponseStatus.BadRequest(ModelState);

            await _offCodeService.EditOffCodeProductAsync(dto);
            return JsonResponseStatus.Success(new { message = "کد تخفیف با موفقیت ایجاد شد" });
        }

        [HttpDelete("product-offcode-delete/{id}")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> DeleteOffCodeProduct(long id)
        {
            await _offCodeService.DeleteOffCodeProductAsync(id);
            return JsonResponseStatus.Success(new { message = "کد تخفیف حذف شد" });
        }

        [HttpPost("category-offcode-add")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> CreateOffCodeCategory([FromBody] AddOffCodeCategoryDto dto)
        {
            if (!ModelState.IsValid)
                return JsonResponseStatus.BadRequest(ModelState);

            await _offCodeService.AddOffCodeCategoryAsync(dto);
            return JsonResponseStatus.Success(new { message = "کد تخفیف با موفقیت ایجاد شد" });
        }
        [HttpPost("category-offcode-edit")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> EditOffCodeCategory([FromBody] EditOffCodeCategoryDto dto)
        {
            if (!ModelState.IsValid)
                return JsonResponseStatus.BadRequest(ModelState);

            await _offCodeService.EditOffCodeCategoryAsync(dto);
            return JsonResponseStatus.Success(new { message = "کد تخفیف با موفقیت ایجاد شد" });
        }

        [HttpDelete("category-offcode-delete/{id}")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> DeleteOffCodeCategory(long id)
        {
            await _offCodeService.DeleteOffCodeCategoryAsync(id);
            return JsonResponseStatus.Success(new { message = "کد تخفیف حذف شد" });
        }

        [HttpPost("user-offcode-add")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> CreateOffCodeUser([FromBody] AddOffCodeUserDto dto)
        {
            if (!ModelState.IsValid)
                return JsonResponseStatus.BadRequest(ModelState);

            await _offCodeService.AddOffCodeUserAsync(dto);
            return JsonResponseStatus.Success(new { message = "کد تخفیف با موفقیت ایجاد شد" });
        }

        [HttpPost("user-offcode-edit")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> EditOffCodeUser([FromBody] EditOffCodeUserDto dto)
        {
            if (!ModelState.IsValid)
                return JsonResponseStatus.BadRequest(ModelState);

            await _offCodeService.EditOffCodeUserAsync(dto);
            return JsonResponseStatus.Success(new { message = "کد تخفیف با موفقیت ایجاد شد" });
        }

        [HttpDelete("user-offcode-delete/{id}")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> DeleteOffCodeUser(long id)
        {
            await _offCodeService.DeleteOffCodeUserAsync(id);
            return JsonResponseStatus.Success(new { message = "کد تخفیف حذف شد" });
        }
    }
}

