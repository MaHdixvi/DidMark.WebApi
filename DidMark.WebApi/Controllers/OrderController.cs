using DidMark.Core.Services.Interfaces;
using DidMark.Core.Utilities.Common;
using DidMark.Core.Utilities.Extensions.Identity;
using DidMark.DataLayer.Entities.Orders;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DidMark.WebApi.Controllers
{

    public class OrderController : SiteBaseController
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        private long GetCurrentUserId()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                throw new UnauthorizedAccessException("کاربر وارد نشده است");

            return User.GetUserId();
        }

        private async Task<(Order order, decimal totalPrice)> GetUserOrderWithTotal(long userId)
        {
            var order = await _orderService.GetUserOpenOrderAsync(userId);
            decimal totalPrice = order != null ? await _orderService.CalculateOrderTotalPriceAsync(order.Id) : 0m;
            return (order, totalPrice);
        }

        [HttpPost("add-order")]
        public async Task<IActionResult> AddProductToOrder([FromQuery] long productId, [FromQuery] int count)
        {
            try
            {
                var userId = GetCurrentUserId();

                if (productId <= 0 || count <= 0)
                    return JsonResponseStatus.BadRequest(new { message = "شناسه محصول یا تعداد نامعتبر است" });

                await _orderService.AddProductToOrderAsync(userId, productId, count);

                var basketDetails = await _orderService.GetUserBasketDetailsAsync(userId);
                var (_, totalPrice) = await GetUserOrderWithTotal(userId);

                return JsonResponseStatus.Success(new
                {
                    message = "محصول با موفقیت به سبد خرید اضافه شد",
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

        [HttpGet("order-details")]
        public async Task<IActionResult> GetUserBasketDetails()
        {
            try
            {
                var userId = GetCurrentUserId();

                var basketDetails = await _orderService.GetUserBasketDetailsAsync(userId);
                var (_, totalPrice) = await GetUserOrderWithTotal(userId);

                return JsonResponseStatus.Success(new
                {
                    items = basketDetails,
                    totalPrice
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

        [HttpDelete("order-details/{detailId}")]
        public async Task<IActionResult> RemoveOrderDetail(long detailId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var order = await _orderService.GetUserOpenOrderAsync(userId);

                if (order == null || !order.OrderDetails.Any(d => d.Id == detailId))
                    return JsonResponseStatus.NotFound(new { message = "جزئیات سفارش یافت نشد" });

                await _orderService.DeleteOrderDetailsAsync(detailId);

                // مجدداً سبد خرید و قیمت کل را بعد از حذف دریافت می‌کنیم
                var basketDetails = await _orderService.GetUserBasketDetailsAsync(userId);
                var (_, totalPrice) = await GetUserOrderWithTotal(userId);

                return JsonResponseStatus.Success(new
                {
                    message = "محصول با موفقیت از سبد خرید حذف شد",
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
