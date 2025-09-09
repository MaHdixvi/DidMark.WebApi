using DidMark.Core.Services.Interfaces;
using DidMark.Core.Utilities.Common;
using DidMark.Core.Utilities.Extensions.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DidMark.WebApi.Controllers
{
    public class CheckoutController : SiteBaseController
    {
        private readonly IOrderService _orderService;

        public CheckoutController(IOrderService orderService)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        private long GetCurrentUserId()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                throw new UnauthorizedAccessException("کاربر وارد نشده است");

            return User.GetUserId();
        }

        [HttpPost("apply-offcode")]
        public async Task<IActionResult> ApplyOffCode([FromQuery] string code)
        {
            try
            {
                var userId = GetCurrentUserId();

                if (string.IsNullOrWhiteSpace(code))
                    return JsonResponseStatus.BadRequest(new { message = "کد تخفیف نامعتبر است" });

                var result = await _orderService.ApplyOffCodeAsync(userId, code);

                if (!result)
                    return JsonResponseStatus.NotFound(new { message = "کد تخفیف یافت نشد یا منقضی شده است" });

                var basketDetails = await _orderService.GetUserBasketDetailsAsync(userId);
                var order = await _orderService.GetUserOpenOrderAsync(userId);
                var totalPrice = await _orderService.CalculateOrderTotalPriceAsync(order.Id);

                return JsonResponseStatus.Success(new
                {
                    message = "کد تخفیف با موفقیت اعمال شد",
                    data = new
                    {
                        items = basketDetails,
                        totalPrice
                    }
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return JsonResponseStatus.Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return JsonResponseStatus.Error(new { message = "خطایی رخ داد", detail = ex.Message });
            }
        }
    }
}
