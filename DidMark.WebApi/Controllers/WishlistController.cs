using DidMark.Core.DTO.Products;
using DidMark.Core.DTO.Products.ProductWishList;
using DidMark.Core.Services.Interfaces;
using DidMark.Core.Utilities.Common;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DidMark.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController : SiteBaseController
    {
        private readonly IProductWishlistService _wishlistService;

        public WishlistController(IProductWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        /// <summary>
        /// افزودن محصول به لیست علاقه‌مندی
        /// </summary>
        [HttpPost("add")]
        public async Task<IActionResult> AddToWishlist([FromBody] AddToWishlistDTO dto)
        {
            if (!ModelState.IsValid)
                return JsonResponseStatus.BadRequest(new { message = "داده‌های ورودی معتبر نیستند" });

            var userId = GetUserId();
            if (userId == 0)
                return JsonResponseStatus.Unauthorized(new { message = "لطفاً وارد حساب کاربری خود شوید" });

            var result = await _wishlistService.AddToWishlist(userId, dto.ProductId);
            if (!result)
                return JsonResponseStatus.Error(new { message = "خطا در افزودن به علاقه‌مندی" });

            return JsonResponseStatus.Success(new { message = "محصول به لیست علاقه‌مندی اضافه شد" });
        }

        /// <summary>
        /// حذف محصول از لیست علاقه‌مندی
        /// </summary>
        [HttpDelete("remove/{productId}")]
        public async Task<IActionResult> RemoveFromWishlist(long productId)
        {
            if (productId <= 0)
                return JsonResponseStatus.BadRequest(new { message = "شناسه محصول نامعتبر است" });

            var userId = GetUserId();
            if (userId == 0)
                return JsonResponseStatus.Unauthorized(new { message = "لطفاً وارد حساب کاربری خود شوید" });

            var result = await _wishlistService.RemoveFromWishlist(userId, productId);
            if (!result)
                return JsonResponseStatus.Error(new { message = "خطا در حذف از علاقه‌مندی" });

            return JsonResponseStatus.Success(new { message = "محصول از لیست علاقه‌مندی حذف شد" });
        }

        /// <summary>
        /// تغییر وضعیت علاقه‌مندی (اضافه/حذف)
        /// </summary>
        [HttpPost("toggle")]
        public async Task<IActionResult> ToggleWishlist([FromBody] AddToWishlistDTO dto)
        {
            if (!ModelState.IsValid)
                return JsonResponseStatus.BadRequest(new { message = "داده‌های ورودی معتبر نیستند" });

            var userId = GetUserId();
            if (userId == 0)
                return JsonResponseStatus.Unauthorized(new { message = "لطفاً وارد حساب کاربری خود شوید" });

            var result = await _wishlistService.ToggleWishlist(userId, dto.ProductId);
            if (!result)
                return JsonResponseStatus.Error(new { message = "خطا در تغییر وضعیت علاقه‌مندی" });

            // بررسی وضعیت جدید
            var isInWishlist = await _wishlistService.IsInWishlist(userId, dto.ProductId);
            var action = isInWishlist ? "اضافه شد" : "حذف شد";

            return JsonResponseStatus.Success(new
            {
                message = $"محصول با موفقیت {action}",
                isInWishlist = isInWishlist
            });
        }

        /// <summary>
        /// دریافت لیست علاقه‌مندی کاربر
        /// </summary>
        [HttpGet("my-wishlist")]
        public async Task<IActionResult> GetMyWishlist()
        {
            var userId = GetUserId();
            if (userId == 0)
                return JsonResponseStatus.Unauthorized(new { message = "لطفاً وارد حساب کاربری خود شوید" });

            var wishlist = await _wishlistService.GetUserWishlist(userId);
            return JsonResponseStatus.Success(wishlist);
        }

        /// <summary>
        /// بررسی آیا محصول در لیست علاقه‌مندی است
        /// </summary>
        [HttpGet("check/{productId}")]
        public async Task<IActionResult> CheckWishlist(long productId)
        {
            if (productId <= 0)
                return JsonResponseStatus.BadRequest(new { message = "شناسه محصول نامعتبر است" });

            var userId = GetUserId();
            if (userId == 0)
                return JsonResponseStatus.Success(new { isInWishlist = false });

            var isInWishlist = await _wishlistService.IsInWishlist(userId, productId);
            return JsonResponseStatus.Success(new { isInWishlist = isInWishlist });
        }

        /// <summary>
        /// دریافت آمار علاقه‌مندی
        /// </summary>
        [HttpGet("stats")]
        public async Task<IActionResult> GetWishlistStats([FromQuery] long? productId = null)
        {
            var userId = GetUserId();
            if (userId == 0)
                return JsonResponseStatus.Success(new { totalItems = 0, isInWishlist = false });

            var stats = await _wishlistService.GetWishlistStats(userId, productId);
            return JsonResponseStatus.Success(stats);
        }

        /// <summary>
        /// تعداد محصولات در لیست علاقه‌مندی
        /// </summary>
        [HttpGet("count")]
        public async Task<IActionResult> GetWishlistCount()
        {
            var userId = GetUserId();
            if (userId == 0)
                return JsonResponseStatus.Success(new { count = 0 });

            var count = await _wishlistService.GetWishlistItemCount(userId);
            return JsonResponseStatus.Success(new { count = count });
        }

        /// <summary>
        /// پاک کردن کل لیست علاقه‌مندی
        /// </summary>
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearWishlist()
        {
            var userId = GetUserId();
            if (userId == 0)
                return JsonResponseStatus.Unauthorized(new { message = "لطفاً وارد حساب کاربری خود شوید" });

            var result = await _wishlistService.ClearWishlist(userId);
            if (!result)
                return JsonResponseStatus.Error(new { message = "خطا در پاک کردن لیست علاقه‌مندی" });

            return JsonResponseStatus.Success(new { message = "لیست علاقه‌مندی با موفقیت پاک شد" });
        }

        private long GetUserId()
        {
            // این متد باید شناسه کاربر را از سیستم احراز هویت برگرداند
            // بستگی به سیستم احراز هویت شما دارد
            if (User.Identity.IsAuthenticated)
            {
                var userIdClaim = User.FindFirst("UserId");
                if (userIdClaim != null && long.TryParse(userIdClaim.Value, out long userId))
                {
                    return userId;
                }
            }
            return 0;
        }
    }
}