using DidMark.Core.DTO.Orders;
using DidMark.DataLayer.Entities.Orders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DidMark.Core.Services.Interfaces
{
    public interface IOrderService : IDisposable
    {
        #region Order
        Task<Order> CreateOrderUserAsync(long userId);
        Task<Order> GetUserOpenOrderAsync(long userId);
        Task CalculateOrderTotalPriceAsync(long orderId, long userId);
        #endregion

        #region Order Detail
        Task AddProductToOrderAsync(long userId, long productId, int count);
        Task<List<OrderDetailsDto>> GetOrderDetailsAsync(long orderId);
        Task<List<OrderBasketDetail>> GetUserBasketDetailsAsync(long userId);
        Task DeleteOrderDetailsAsync(long detailId);
        #endregion

        #region OffCode
        Task<bool> ApplyOffCodeAsync(long userId, string code);
        Task IncreaseOffCodeUsageAsync(long orderId, long userId);

        #endregion
        Task UpdateOrderDetailCountAsync(long detailId, int newCount, long userId);

        #region Payment Status

        Task<bool> MarkOrderAsPaidAsync(long orderId, long refId);
        #endregion

    }
}