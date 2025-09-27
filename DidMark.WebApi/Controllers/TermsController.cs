using DidMark.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DidMark.WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/terms")]
    public class TermsController : ControllerBase
    {
        private readonly IUserService _userService;
        public TermsController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [HttpGet("has-accepted")]
        public async Task<IActionResult> HasAcceptedTerms()
        {
            var userId = long.Parse(User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId == 0) return Unauthorized(new { isSuccess = false, message = "کاربر وارد نشده" });

            var hasAccepted = await _userService.HasAcceptedTermsAsync(userId);
            return Ok(new { isSuccess = true, data = hasAccepted });
        }

        [Authorize]
        [HttpPost("accept")]
        public async Task<IActionResult> AcceptTerms()
        {
            var userId = long.Parse(User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId == 0) return Unauthorized(new { isSuccess = false, message = "کاربر وارد نشده" });

            var result = await _userService.AcceptTermsAsync(userId);
            if (!result) return BadRequest(new { isSuccess = false, message = "خطا در ثبت پذیرش قوانین" });

            return Ok(new { isSuccess = true, message = "قوانین پذیرفته شد" });
        }
    }

}
