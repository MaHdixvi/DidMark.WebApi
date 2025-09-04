using DidMark.Core.Services.Interfaces;
using DidMark.Core.Utilities.Common;
using DidMark.Core.Utilities.Extensions.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DidMark.WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        [HttpPost("add-order")]
        public async Task<IActionResult> AddProductToOrder([FromQuery] long productId, [FromQuery] int count)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return JsonResponseStatus.Unauthorized(new { message = "برای افزودن محصول به سبد خرید، ابتدا ثبت‌نام یا ورود کنید" });
            }

            if (productId <= 0 || count <= 0)
            {
                return JsonResponseStatus.BadRequest(new { message = "شناسه محصول یا تعداد نامعتبر است" });
            }

            var userId = User.GetUserId();
            await _orderService.AddProductToOrderAsync(userId, productId, count);
            var basketDetails = await _orderService.GetUserBasketDetailsAsync(userId);

            return JsonResponseStatus.Success(new
            {
                message = "محصول با موفقیت به سبد خرید اضافه شد",
                data = basketDetails
            });
        }

        [HttpGet("order-details")]
        public async Task<IActionResult> GetUserBasketDetails()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return JsonResponseStatus.Unauthorized(new { message = "برای مشاهده جزئیات سبد خرید، ابتدا ثبت‌نام یا ورود کنید" });
            }

            var userId = User.GetUserId();
            var details = await _orderService.GetUserBasketDetailsAsync(userId);
            return JsonResponseStatus.Success(details);
        }

        [HttpDelete("order-details/{detailId}")]
        public async Task<IActionResult> RemoveOrderDetail(long detailId)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return JsonResponseStatus.Unauthorized(new { message = "برای حذف محصول از سبد خرید، ابتدا ثبت‌نام یا ورود کنید" });
            }

            var userId = User.GetUserId();
            var userOpenOrder = await _orderService.GetUserOpenOrderAsync(userId);
            if (userOpenOrder == null || !userOpenOrder.OrderDetails.Any(x => x.Id == detailId))
            {
                return JsonResponseStatus.NotFound(new { message = "جزئیات سفارش یافت نشد" });
            }

            await _orderService.DeleteOrderDetailsAsync(detailId);
            var basketDetails = await _orderService.GetUserBasketDetailsAsync(userId);
            return JsonResponseStatus.Success(new
            {
                message = "محصول با موفقیت از سبد خرید حذف شد",
                data = basketDetails
            });
        }
    }
}