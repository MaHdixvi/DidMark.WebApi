using DidMark.Core.DTO.Orders;
using DidMark.Core.Services.Interfaces;
using DidMark.Core.Utilities.Common;
using DidMark.DataLayer.Entities.Orders;
using DidMark.DataLayer.Repository;
using Microsoft.EntityFrameworkCore;
namespace DidMark.Core.Services.Implementations
{
    public class OrderService : IOrderService
    {
        #region Fields
        private readonly IGenericRepository<Order> _orderRepository;
        private readonly IGenericRepository<OrderDetail> _orderDetailRepository;
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly ISmsService _smsService;
        #endregion

        #region Constructor
        public OrderService(
            IGenericRepository<Order> orderRepository,
            IGenericRepository<OrderDetail> orderDetailRepository,
            IUserService userService,
            IProductService productService,
            ISmsService smsService)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _orderDetailRepository = orderDetailRepository ?? throw new ArgumentNullException(nameof(orderDetailRepository));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _smsService = smsService ?? throw new ArgumentNullException(nameof(smsService));
        }
        #endregion

        #region Order
        public async Task<Order> CreateOrderUserAsync(long userId)
        {
            var order = new Order
            {
                UserId = userId,
                IsPay = false,
                PaymentDate = null,
                OrderDetails = new List<OrderDetail>()
            };

            await _orderRepository.AddEntity(order);
            await _orderRepository.SaveChanges();

            // ارسال پیامک اطلاع‌رسانی ایجاد سفارش
            var user = await _userService.GetUserByIdAsync(userId);
            if (user != null)
            {
                await _smsService.SendActivatedAccountSmsAsync(user.PhoneNumber);
            }

            return order;
        }

        public async Task<Order> GetUserOpenOrderAsync(long userId)
        {
            var order = await _orderRepository.GetEntitiesQuery()
                .Include(s => s.OrderDetails)
                .ThenInclude(d => d.Product)
                .Include(s => s.OffCode)
                .SingleOrDefaultAsync(s => s.UserId == userId && !s.IsPay && !s.IsDelete);

            if (order == null)
            {
                order = await CreateOrderUserAsync(userId);
            }

            return order;
        }

        public async Task<decimal> CalculateOrderTotalPriceAsync(long orderId)
        {
            var order = await _orderRepository.GetEntitiesQuery()
                .Include(o => o.OrderDetails)
                .ThenInclude(d => d.Product)
                .Include(o => o.OffCode)
                .SingleOrDefaultAsync(o => o.Id == orderId && !o.IsDelete);

            if (order == null) return 0m;

            decimal subtotal = 0m;
            foreach (var detail in order.OrderDetails.Where(d => !d.IsDelete))
            {
                subtotal += detail.Price * detail.Count;
            }

            if (order.OffCode != null && order.OffCode.ExpireDate >= DateTime.Now)
            {
                subtotal -= subtotal * (order.OffCode.DiscountPercentage / 100);
            }

            return subtotal;
        }
        #endregion

        #region Order Detail
        public async Task AddProductToOrderAsync(long userId, long productId, int count)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            var product = await _productService.GetProductById(productId);

            if (user == null || product == null) return;

            var order = await GetUserOpenOrderAsync(userId);
            if (count > 1) count = 1;

            var detail = await _orderDetailRepository.GetEntitiesQuery()
                .SingleOrDefaultAsync(d => d.OrderId == order.Id && d.ProductId == productId && !d.IsDelete);

            if (detail != null)
            {
                detail.Count += count;
                _orderDetailRepository.UpdateEntity(detail);
            }
            else
            {
                var newDetail = new OrderDetail
                {
                    OrderId = order.Id,
                    ProductId = productId,
                    Count = count,
                    Price = (int)product.Price
                };
                await _orderDetailRepository.AddEntity(newDetail);
            }

            await _orderDetailRepository.SaveChanges();

            // ارسال پیامک با جزئیات سفارش
            var basketDetails = await GetUserBasketDetailsAsync(userId);
            var totalPrice = await CalculateOrderTotalPriceAsync(order.Id);
            await _smsService.SendOrderSummarySmsAsync(user.PhoneNumber, basketDetails, totalPrice);
        }

        public async Task<List<OrderDetail>> GetOrderDetailsAsync(long orderId)
        {
            return await _orderDetailRepository.GetEntitiesQuery()
                .Include(d => d.Product)
                .Where(d => d.OrderId == orderId && !d.IsDelete)
                .ToListAsync();
        }

        public async Task<List<OrderBasketDetail>> GetUserBasketDetailsAsync(long userId)
        {
            var order = await GetUserOpenOrderAsync(userId);
            if (order == null) return null;

            var details = order.OrderDetails.Where(d => !d.IsDelete).ToList();
            var basket = new List<OrderBasketDetail>();

            foreach (var d in details)
            {
                basket.Add(new OrderBasketDetail
                {
                    Id = d.Id,
                    Count = d.Count,
                    Price = d.Price,
                    ProductName = d.Product.ProductName,
                    ImageName = PathTools.Domain + PathTools.ProductImagePath + d.Product.ImageName
                });
            }

            return basket;
        }

        public async Task DeleteOrderDetailsAsync(long detailId)
        {
            var detail = await _orderDetailRepository.GetEntitiesQuery()
                .SingleOrDefaultAsync(d => d.Id == detailId && !d.IsDelete);

            if (detail != null)
            {
                _orderDetailRepository.RemoveEntity(detail);
                await _orderDetailRepository.SaveChanges();
            }
        }
        #endregion

        #region Dispose
        public void Dispose()
        {
            _orderRepository?.Dispose();
            _orderDetailRepository?.Dispose();
        }
        #endregion
    }
}
