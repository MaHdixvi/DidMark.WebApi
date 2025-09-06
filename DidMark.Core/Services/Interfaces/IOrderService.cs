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
        Task<decimal> CalculateOrderTotalPriceAsync(long orderId);
        #endregion

        #region Order Detail
        Task AddProductToOrderAsync(long userId, long productId, int count);
        Task<List<OrderDetail>> GetOrderDetailsAsync(long orderId);
        Task<List<OrderBasketDetail>> GetUserBasketDetailsAsync(long userId);
        Task DeleteOrderDetailsAsync(long detailId);
        #endregion
    }
}