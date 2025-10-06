using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DidMark.Core.DTO.Orders;
using DidMark.Core.Services.Interfaces;
using DidMark.Core.Utilities.Common;
using System.ComponentModel.DataAnnotations;
using DidMark.Core.DTO.OffCodes;

namespace DidMark.WebApi.Controllers
{
    public class AdminOrderController : SiteBaseController
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<AdminOrderController> _logger;

        public AdminOrderController(IOrderService orderService, ILogger<AdminOrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        /// <summary>
        /// دریافت لیست سفارشات با صفحه‌بندی و فیلتر
        /// GET: api/v1/AdminOrder/get-all
        /// </summary>
        [HttpGet("get-all")]
        public async Task<IActionResult> GetOrders(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool? isPay = null,
            [FromQuery] string? search = null)
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync(page, pageSize, isPay, search);
                var totalCount = orders.Count; // در حالت واقعی باید از سرویس تعداد کل را دریافت کرد

                var response = new OrderListResponse
                {
                    Orders = orders,
                    TotalCount = totalCount,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                };

                return JsonResponseStatus.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در دریافت لیست سفارشات");
                return JsonResponseStatus.Error("خطا در دریافت لیست سفارشات");
            }
        }

        /// <summary>
        /// دریافت جزئیات سفارش
        /// GET: api/v1/AdminOrder/get/{id}
        /// </summary>
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetOrderById(long id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                return JsonResponseStatus.Success(order);
            }
            catch (KeyNotFoundException)
            {
                return JsonResponseStatus.NotFound("سفارش یافت نشد");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در دریافت جزئیات سفارش");
                return JsonResponseStatus.Error("خطا در دریافت جزئیات سفارش");
            }
        }

        /// <summary>
        /// دریافت سبد خرید سفارش
        /// GET: api/v1/AdminOrder/{orderId}/basket
        /// </summary>
        [HttpGet("{orderId}/basket")]
        public async Task<IActionResult> GetOrderBasket(long orderId)
        {
            try
            {
                var basket = await _orderService.GetOrderBasketAsync(orderId);
                return JsonResponseStatus.Success(basket);
            }
            catch (KeyNotFoundException)
            {
                return JsonResponseStatus.NotFound("سبد خرید سفارش یافت نشد");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در دریافت سبد خرید سفارش");
                return JsonResponseStatus.Error("خطا در دریافت سبد خرید سفارش");
            }
        }

        /// <summary>
        /// آمار سفارشات
        /// GET: api/v1/AdminOrder/stats
        /// </summary>
        [HttpGet("stats")]
        public async Task<IActionResult> GetOrderStats()
        {
            try
            {
                var stats = await _orderService.GetOrderStatsAsync();
                return JsonResponseStatus.Success(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در دریافت آمار سفارشات");
                return JsonResponseStatus.Error("خطا در دریافت آمار سفارشات");
            }
        }

        /// <summary>
        /// به‌روزرسانی وضعیت پرداخت سفارش
        /// PUT: api/v1/AdminOrder/update-payment-status/{id}
        /// </summary>
        [HttpPut("update-payment-status/{id}")]
        public async Task<IActionResult> UpdatePaymentStatus(long id, [FromBody] UpdatePaymentStatusRequest request)
        {
            try
            {
                var result = await _orderService.UpdateOrderPaymentStatusAsync(id, request.IsPay);

                if (result)
                {
                    var updatedOrder = await _orderService.GetOrderByIdAsync(id);
                    return JsonResponseStatus.Success(updatedOrder);
                }
                else
                {
                    return JsonResponseStatus.NotFound("سفارش یافت نشد");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در به‌روزرسانی وضعیت پرداخت سفارش");
                return JsonResponseStatus.Error("خطا در به‌روزرسانی وضعیت پرداخت سفارش");
            }
        }

        /// <summary>
        /// حذف سفارش
        /// DELETE: api/v1/AdminOrder/delete/{id}
        /// </summary>
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteOrder(long id)
        {
            try
            {
                var result = await _orderService.DeleteOrderAsync(id);

                if (result)
                {
                    return JsonResponseStatus.Success("سفارش با موفقیت حذف شد");
                }
                else
                {
                    return JsonResponseStatus.NotFound("سفارش یافت نشد");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در حذف سفارش");
                return JsonResponseStatus.Error("خطا در حذف سفارش");
            }
        }

        /// <summary>
        /// جستجوی سفارشات
        /// GET: api/v1/AdminOrder/search
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> SearchOrders([FromQuery] string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return JsonResponseStatus.BadRequest("پارامتر جستجو الزامی است");
                }

                var orders = await _orderService.SearchOrdersAsync(query);
                return JsonResponseStatus.Success(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در جستجوی سفارشات");
                return JsonResponseStatus.Error("خطا در جستجوی سفارشات");
            }
        }

        /// <summary>
        /// دریافت سفارشات امروز
        /// GET: api/v1/AdminOrder/today
        /// </summary>
        [HttpGet("today")]
        public async Task<IActionResult> GetTodayOrders()
        {
            try
            {
                var orders = await _orderService.GetTodayOrdersAsync();
                return JsonResponseStatus.Success(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در دریافت سفارشات امروز");
                return JsonResponseStatus.Error("خطا در دریافت سفارشات امروز");
            }
        }

        /// <summary>
        /// افزودن محصول به سفارش
        /// POST: api/v1/AdminOrder/{orderId}/add-product
        /// </summary>
        [HttpPost("{orderId}/add-product")]
        public async Task<IActionResult> AddProductToOrder(long orderId, [FromBody] AddProductToOrderRequest request)
        {
            try
            {
                // ابتدا سفارش را دریافت می‌کنیم تا userId را داشته باشیم
                var order = await _orderService.GetOrderByIdAsync(orderId);
                await _orderService.AddProductToOrderAsync(order.UserId, request.ProductId, request.Count);

                return JsonResponseStatus.Success("محصول با موفقیت به سفارش اضافه شد");
            }
            catch (InvalidOperationException ex)
            {
                return JsonResponseStatus.BadRequest(ex.Message);
            }
            catch (KeyNotFoundException)
            {
                return JsonResponseStatus.NotFound("سفارش یافت نشد");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در افزودن محصول به سفارش");
                return JsonResponseStatus.Error("خطا در افزودن محصول به سفارش");
            }
        }

        /// <summary>
        /// حذف محصول از سفارش
        /// DELETE: api/v1/AdminOrder/order-detail/{detailId}
        /// </summary>
        [HttpDelete("order-detail/{detailId}")]
        public async Task<IActionResult> DeleteOrderDetail(long detailId)
        {
            try
            {
                await _orderService.DeleteOrderDetailsAsync(detailId);
                return JsonResponseStatus.Success("محصول با موفقیت از سفارش حذف شد");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در حذف محصول از سفارش");
                return JsonResponseStatus.Error("خطا در حذف محصول از سفارش");
            }
        }

        /// <summary>
        /// به‌روزرسانی تعداد محصول در سفارش
        /// PUT: api/v1/AdminOrder/order-detail/{detailId}/update-count
        /// </summary>
        [HttpPut("order-detail/{detailId}/update-count")]
        public async Task<IActionResult> UpdateOrderDetailCount(long detailId, [FromBody] UpdateOrderDetailCountRequest request)
        {
            try
            {
                // برای استفاده از این متد نیاز به userId داریم
                // در حالت واقعی باید از context کاربر جاری استفاده شود
                // اینجا به عنوان مثال از یک مقدار ثابت استفاده می‌کنیم
                long currentUserId = 1; // باید از سیستم احراز هویت دریافت شود

                await _orderService.UpdateOrderDetailCountAsync(detailId, request.NewCount, currentUserId);
                return JsonResponseStatus.Success("تعداد محصول با موفقیت به‌روزرسانی شد");
            }
            catch (InvalidOperationException ex)
            {
                return JsonResponseStatus.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در به‌روزرسانی تعداد محصول");
                return JsonResponseStatus.Error("خطا در به‌روزرسانی تعداد محصول");
            }
        }

        /// <summary>
        /// دریافت جزئیات کامل سفارش با اطلاعات کاربر
        /// GET: api/v1/AdminOrder/{id}/full-details
        /// </summary>
        [HttpGet("{id}/full-details")]
        public async Task<IActionResult> GetOrderFullDetails(long id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                var basket = await _orderService.GetOrderBasketAsync(id);

                var fullDetails = new OrderDetailResponse
                {
                    Order = order,
                    UserInfo = new UserOrderInfoDto
                    {
                        // در حالت واقعی این اطلاعات از سرویس کاربران دریافت می‌شود
                        UserId = order.Id, // اینجا نمونه‌سازی شده
                        UserName = "کاربر نمونه",
                        Email = "user@example.com",
                        PhoneNumber = "09123456789"
                    }
                };

                return JsonResponseStatus.Success(fullDetails);
            }
            catch (KeyNotFoundException)
            {
                return JsonResponseStatus.NotFound("سفارش یافت نشد");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در دریافت جزئیات کامل سفارش");
                return JsonResponseStatus.Error("خطا در دریافت جزئیات کامل سفارش");
            }
        }

        /// <summary>
        /// اعمال کد تخفیف روی سفارش
        /// POST: api/v1/AdminOrder/{orderId}/apply-offcode
        /// </summary>
        [HttpPost("{orderId}/apply-offcode")]
        public async Task<IActionResult> ApplyOffCode(long orderId, [FromBody] ApplyOffCodeDto request)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(orderId);
                var result = await _orderService.ApplyOffCodeAsync(order.UserId, request.Code);

                if (result)
                {
                    var updatedOrder = await _orderService.GetOrderByIdAsync(orderId);
                    return JsonResponseStatus.Success(updatedOrder);
                }
                else
                {
                    return JsonResponseStatus.BadRequest("کد تخفیف معتبر نیست یا قابل اعمال روی این سفارش نمی‌باشد");
                }
            }
            catch (KeyNotFoundException)
            {
                return JsonResponseStatus.NotFound("سفارش یافت نشد");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در اعمال کد تخفیف");
                return JsonResponseStatus.Error("خطا در اعمال کد تخفیف");
            }
        }
    }
}