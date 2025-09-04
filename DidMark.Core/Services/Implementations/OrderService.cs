using DidMark.Core.DTO.Orders;
using DidMark.Core.Services.Interfaces;
using DidMark.Core.Utilities.Common;
using DidMark.DataLayer.Entities.Orders;
using DidMark.DataLayer.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DidMark.Core.Services.Implementations
{
    public class OrderService : IOrderService
    {
        #region Fields
        private readonly IGenericRepository<Order> _orderRepository;
        private readonly IGenericRepository<OrderDetail> _orderDetailRepository;
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        #endregion

        #region Constructor
        public OrderService(
            IGenericRepository<Order> orderRepository,
            IGenericRepository<OrderDetail> orderDetailRepository,
            IUserService userService,
            IProductService productService)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _orderDetailRepository = orderDetailRepository ?? throw new ArgumentNullException(nameof(orderDetailRepository));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        }
        #endregion

        #region Order
        /// <summary>
        /// Creates a new order for the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The created order.</returns>
        public async Task<Order> CreateOrderUserAsync(long userId)
        {
            var order = new Order { UserId = userId };
            await _orderRepository.AddEntity(order);
            await _orderRepository.SaveChanges();
            return order;
        }

        /// <summary>
        /// Retrieves the open (unpaid and not deleted) order for the specified user, or creates a new one if none exists.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The open order, or a new order if none exists.</returns>
        public async Task<Order> GetUserOpenOrderAsync(long userId)
        {
            var order = await _orderRepository.GetEntitiesQuery()
                .Include(s => s.OrderDetails)
                .ThenInclude(s => s.Product)
                .SingleOrDefaultAsync(s => s.UserId == userId && !s.IsPay && !s.IsDelete);
            if (order == null)
            {
                order = await CreateOrderUserAsync(userId);
            }
            return order;
        }
        #endregion

        #region Order Detail
        /// <summary>
        /// Adds a product to the user's open order, updating the quantity if the product already exists.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="productId">The ID of the product.</param>
        /// <param name="count">The quantity to add (limited to 1).</param>
        public async Task AddProductToOrderAsync(long userId, long productId, int count)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            var product = await _productService.GetProductById(productId);
            if (user == null || product == null)
            {
                return;
            }

            var order = await GetUserOpenOrderAsync(userId);
            if (count > 1) count = 1; // Limit count to 1

            var details = await GetOrderDetailsAsync(order.Id);
            var existsDetail = details.SingleOrDefault(s => s.ProductId == productId && !s.IsDelete);
            if (existsDetail != null)
            {
                existsDetail.Count += count;
                _orderDetailRepository.UpdateEntity(existsDetail);
            }
            else
            {
                var detail = new OrderDetail
                {
                    OrderId = order.Id,
                    ProductId = product.Id,
                    Count = count,
                    Price = (int)product.Price
                };
                await _orderDetailRepository.AddEntity(detail);
            }
            await _orderDetailRepository.SaveChanges();
        }

        /// <summary>
        /// Retrieves the list of order details for a specific order.
        /// </summary>
        /// <param name="orderId">The ID of the order.</param>
        /// <returns>A list of order details.</returns>
        public async Task<List<OrderDetail>> GetOrderDetailsAsync(long orderId)
        {
            return await _orderDetailRepository.GetEntitiesQuery()
                .Where(s => s.OrderId == orderId)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves the basket details for the user's open order.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A list of basket details, or null if no open order exists.</returns>
        public async Task<List<OrderBasketDetail>> GetUserBasketDetailsAsync(long userId)
        {
            var openOrder = await GetUserOpenOrderAsync(userId);
            if (openOrder == null)
            {
                return null;
            }
            return openOrder.OrderDetails.Select(f => new OrderBasketDetail
            {
                Id = f.Id,
                Count = f.Count,
                Price = f.Price,
                Title = f.Product.ProductName,
                ImageName = PathTools.Domain + PathTools.ProductImagePath + f.Product.ImageName
            }).ToList();
        }

        /// <summary>
        /// Deletes an order detail by its ID, if it exists and is not already deleted.
        /// </summary>
        /// <param name="detailId">The ID of the order detail.</param>
        public async Task DeleteOrderDetailsAsync(long detailId)
        {
            var detail = await _orderDetailRepository.GetEntitiesQuery()
                .SingleOrDefaultAsync(s => s.Id == detailId && !s.IsDelete);
            if (detail != null)
            {
                _orderDetailRepository.RemoveEntity(detail);
                await _orderDetailRepository.SaveChanges();
            }
        }
        #endregion

        #region Dispose
        /// <summary>
        /// Disposes of the repository resources.
        /// </summary>
        public void Dispose()
        {
            _orderRepository?.Dispose();
            _orderDetailRepository?.Dispose();
        }
        #endregion
    }
}