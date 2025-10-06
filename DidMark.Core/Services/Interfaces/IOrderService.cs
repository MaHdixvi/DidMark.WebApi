using DidMark.Core.DTO.Orders;
using DidMark.DataLayer.Entities.Orders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DidMark.Core.Services.Interfaces
{
    public interface IOrderService : IDisposable
    {
        #region Order Management
        /// <summary>
        /// ایجاد سفارش جدید برای کاربر
        /// </summary>
        Task<Order> CreateOrderUserAsync(long userId);

        /// <summary>
        /// دریافت سفارش باز کاربر
        /// </summary>
        Task<Order> GetUserOpenOrderAsync(long userId);

        /// <summary>
        /// دریافت سفارش باز کاربر به صورت DTO
        /// </summary>
        Task<OrderDto> GetUserOpenOrderDtoAsync(long userId);

        /// <summary>
        /// دریافت تمام سفارشات با قابلیت صفحه‌بندی و فیلتر
        /// </summary>
        Task<List<OrderDto>> GetAllOrdersAsync(int page = 1, int pageSize = 10, bool? isPay = null, string? search = null);

        /// <summary>
        /// دریافت سفارش بر اساس شناسه
        /// </summary>
        Task<OrderDto> GetOrderByIdAsync(long orderId);

        /// <summary>
        /// جستجوی سفارشات
        /// </summary>
        Task<List<OrderDto>> SearchOrdersAsync(string query);

        /// <summary>
        /// دریافت سفارشات امروز
        /// </summary>
        Task<List<OrderDto>> GetTodayOrdersAsync();

        /// <summary>
        /// حذف سفارش (Soft Delete)
        /// </summary>
        Task<bool> DeleteOrderAsync(long orderId);
        #endregion

        #region Order Status & Payment
        /// <summary>
        /// به‌روزرسانی وضعیت پرداخت سفارش
        /// </summary>
        Task<bool> UpdateOrderPaymentStatusAsync(long orderId, bool isPay);

        /// <summary>
        /// علامت‌گذاری سفارش به عنوان پرداخت شده
        /// </summary>
        Task<bool> MarkOrderAsPaidAsync(long orderId, long refId);

        /// <summary>
        /// محاسبه قیمت کل سفارش
        /// </summary>
        Task CalculateOrderTotalPriceAsync(long orderId, long userId);
        #endregion

        #region Order Details & Basket
        /// <summary>
        /// افزودن محصول به سفارش
        /// </summary>
        Task AddProductToOrderAsync(long userId, long productId, int count);

        /// <summary>
        /// دریافت جزئیات سفارش
        /// </summary>
        Task<List<OrderDetailsDto>> GetOrderDetailsAsync(long orderId);

        /// <summary>
        /// دریافت سبد خرید کاربر
        /// </summary>
        Task<List<OrderBasketDetail>> GetUserBasketDetailsAsync(long userId);

        /// <summary>
        /// دریافت سبد خرید سفارش
        /// </summary>
        Task<List<OrderBasketDetail>> GetOrderBasketDetailsAsync(long orderId);

        /// <summary>
        /// دریافت سبد خرید کامل سفارش
        /// </summary>
        Task<BasketResponse> GetOrderBasketAsync(long orderId);

        /// <summary>
        /// حذف آیتم از سفارش
        /// </summary>
        Task DeleteOrderDetailsAsync(long detailId);

        /// <summary>
        /// به‌روزرسانی تعداد آیتم در سفارش
        /// </summary>
        Task UpdateOrderDetailCountAsync(long detailId, int newCount, long userId);

        /// <summary>
        /// دریافت جزئیات سفارش به همراه اطلاعات سفارش
        /// </summary>
        Task<OrderDetail> GetOrderDetailWithOrderAsync(long detailId);
        #endregion

        #region OffCode Management
        /// <summary>
        /// اعمال کد تخفیف روی سفارش
        /// </summary>
        Task<bool> ApplyOffCodeAsync(long userId, string code);

        /// <summary>
        /// افزایش تعداد استفاده از کد تخفیف
        /// </summary>
        Task IncreaseOffCodeUsageAsync(long orderId, long userId);
        #endregion

        #region Statistics & Reports
        /// <summary>
        /// دریافت آمار سفارشات
        /// </summary>
        Task<OrderStatsDto> GetOrderStatsAsync();
        #endregion
    }
}