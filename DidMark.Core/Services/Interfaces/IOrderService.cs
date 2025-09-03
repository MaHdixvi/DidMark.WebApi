using DidMark.Core.DTO.Orders;
using DidMark.DataLayer.Entities.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.Services.Interfaces
{
    public interface IOrderService : IDisposable
    {
        #region order
        Task<Order> CreateOrderUser(long userId);
        Task<Order> GetUserOpenOrder(long userId);
        #endregion
        #region order detail
        Task AddProductToOrder(long userId, long productId, int count);
        Task<List<OrderDetail>> GetOrderDetails(long orderId);
        Task<List<OrderBasketDetail>> GetUserBasketDetails(long userId);
        Task DeleteOrderDetails(OrderDetail detail);
        #endregion



    }
}
