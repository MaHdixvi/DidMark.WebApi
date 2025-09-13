using DidMark.Core.DTO.Products;
using DidMark.Core.Services.Interfaces;
using DidMark.Core.Utilities.Common;
using DidMark.WebApi.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DidMark.WebApi.Controllers
{
    public class AdminAttributeController : SiteBaseController
    {
        private readonly IAttributeService _attributeService;

        public AdminAttributeController(IAttributeService attributeService)
        {
            _attributeService = attributeService ?? throw new ArgumentNullException(nameof(attributeService));
        }

        #region Attribute CRUD

        [HttpGet("get-all")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> GetAll()
        {
            var attributes = await _attributeService.GetAllAttributes();
            return JsonResponseStatus.Success(attributes);
        }

        [HttpGet("get/{id}")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> Get(long id)
        {
            var attribute = await _attributeService.GetAttributeById(id);
            if (attribute == null)
                return JsonResponseStatus.NotFound(new { message = "ویژگی یافت نشد" });

            return JsonResponseStatus.Success(attribute);
        }

        [HttpGet("get-by-category/{categoryId}")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> GetByCategory(long categoryId)
        {
            var attributes = await _attributeService.GetAttributesByCategoryId(categoryId);
            return JsonResponseStatus.Success(attributes);
        }

        [HttpPost("add")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> Add([FromBody] CreateAttributeDto dto)
        {
            await _attributeService.AddAttribute(dto);
            return JsonResponseStatus.Success(new { message = "ویژگی اضافه شد" });
        }

        [HttpPut("update/{id}")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> Update(long id, [FromBody] EditAttributeDto dto)
        {
            await _attributeService.UpdateAttribute(id, dto);
            return JsonResponseStatus.Success(new { message = "ویژگی بروزرسانی شد" });
        }

        [HttpDelete("delete/{id}")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _attributeService.DeleteAttribute(id);
            if (!result)
                return JsonResponseStatus.NotFound(new { message = "ویژگی یافت نشد یا قبلا حذف شده است" });

            return JsonResponseStatus.Success(new { message = "ویژگی حذف شد" });
        }

        #endregion
    }
}
