using DidMark.Core.DTO.Orders;
using DidMark.Core.Services.Interfaces;
using DidMark.Core.Utilities.Common;
using DidMark.Core.Utilities.Extensions.Identity;
using DidMark.DataLayer.Entities.Orders;
using Microsoft.AspNetCore.Authorization;
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

        [HttpPut("order-details/{detailId}/count")]
        public async Task<IActionResult> UpdateOrderDetailCount(long detailId, [FromQuery] int newCount)
        {
            try
            {
                var userId = GetCurrentUserId();

                if (newCount <= 0)
                    return JsonResponseStatus.BadRequest(new { message = "تعداد باید بیشتر از صفر باشد" });

                await _orderService.UpdateOrderDetailCountAsync(detailId, newCount, userId);

                var basketDetails = await _orderService.GetUserBasketDetailsAsync(userId);
                var order = await _orderService.GetUserOpenOrderAsync(userId);

                return JsonResponseStatus.Success(new
                {
                    message = "تعداد محصول با موفقیت به‌روزرسانی شد",
                    data = new
                    {
                        items = basketDetails,
                        order.Subtotal,
                        order.DiscountAmount,
                        order.TotalPrice
                    }
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return JsonResponseStatus.Unauthorized(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return JsonResponseStatus.Error(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return JsonResponseStatus.Error(new { message = "خطایی رخ داد", detail = ex.Message });
            }
        }

        #region Apply Discount
        [HttpPost("apply-offcode")]
        [Authorize]
        public async Task<IActionResult> ApplyOffCode([FromBody] ApplyOffCodeRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();

                if (request == null || string.IsNullOrWhiteSpace(request.Code))
                    return JsonResponseStatus.BadRequest(new { message = "کد تخفیف نامعتبر است" });

                var result = await _orderService.ApplyOffCodeAsync(userId, request.Code);

                if (!result)
                    return JsonResponseStatus.NotFound(new { message = "کد تخفیف یافت نشد یا منقضی شده است" });

                var basketDetails = await _orderService.GetUserBasketDetailsAsync(userId);
                var order = await _orderService.GetUserOpenOrderAsync(userId);

                return JsonResponseStatus.Success(new
                {
                    message = "کد تخفیف با موفقیت اعمال شد",
                    data = new
                    {
                        items = basketDetails,
                        order.Subtotal,
                        order.DiscountAmount,
                        order.TotalPrice
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

        public class ApplyOffCodeRequest
        {
            public string Code { get; set; }
        }
        #endregion

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
                var order = await _orderService.GetUserOpenOrderAsync(userId);

                return JsonResponseStatus.Success(new
                {
                    message = "محصول با موفقیت به سبد خرید اضافه شد",
                    data = new
                    {
                        items = basketDetails,
                        order.Subtotal,
                        order.DiscountAmount,
                        order.TotalPrice
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
                var order = await _orderService.GetUserOpenOrderAsync(userId);

                return JsonResponseStatus.Success(new
                {
                    items = basketDetails,
                    order.Subtotal,
                    order.DiscountAmount,
                    order.TotalPrice
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

                return JsonResponseStatus.Success(new
                {
                    message = "محصول با موفقیت از سبد خرید حذف شد",
                    data = new
                    {
                        items = basketDetails,
                        order.Subtotal,
                        order.DiscountAmount,
                        order.TotalPrice
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
