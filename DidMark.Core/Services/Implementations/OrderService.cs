using DidMark.Core.DTO.OffCodes;
using DidMark.Core.DTO.Orders;
using DidMark.Core.DTO.Products;
using DidMark.Core.Services.Interfaces;
using DidMark.Core.Utilities.Common;
using DidMark.DataLayer.Entities.Orders;
using DidMark.DataLayer.Entities.Product;
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
        private readonly IOffCodeService _offCodeService;
        #endregion

        #region Constructor
        public OrderService(
            IGenericRepository<Order> orderRepository,
            IGenericRepository<OrderDetail> orderDetailRepository,
            IUserService userService,
            IProductService productService,
            ISmsService smsService,
            IOffCodeService offCodeService)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _orderDetailRepository = orderDetailRepository ?? throw new ArgumentNullException(nameof(orderDetailRepository));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _smsService = smsService ?? throw new ArgumentNullException(nameof(smsService));
            _offCodeService = offCodeService ?? throw new ArgumentNullException(nameof(offCodeService));
        }
        #endregion

        #region Order Management
        public async Task<Order> CreateOrderUserAsync(long userId)
        {
            var order = new Order
            {
                UserId = userId,
                IsPay = false,
                PaymentDate = null,
                OrderDetails = new List<OrderDetail>(),
                TotalPrice = 0m,
                Subtotal = 0m,
                DiscountAmount = 0m
            };

            await _orderRepository.AddEntity(order);
            await _orderRepository.SaveChanges();

            return order;
        }

        public async Task<Order> GetUserOpenOrderAsync(long userId)
        {
            var order = await _orderRepository.GetEntitiesQuery()
                .Include(s => s.OrderDetails)
                .ThenInclude(d => d.Product)
                .ThenInclude(p => p.ProductSelectedCategories)
                .Include(s => s.OffCode)
                .SingleOrDefaultAsync(s => s.UserId == userId && !s.IsPay && !s.IsDelete);

            return order ?? await CreateOrderUserAsync(userId);
        }

        public async Task<OrderDto> GetUserOpenOrderDtoAsync(long userId)
        {
            var order = await GetUserOpenOrderAsync(userId);
            return await MapOrderToDtoAsync(order);
        }

        public async Task<List<OrderDto>> GetAllOrdersAsync(int page = 1, int pageSize = 10, bool? isPay = null, string? search = null)
        {
            var query = _orderRepository.GetEntitiesQuery()
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Include(o => o.OffCode)
                .Where(o => !o.IsDelete)
                .AsQueryable();

            // اعمال فیلترها
            if (isPay.HasValue)
                query = query.Where(o => o.IsPay == isPay.Value);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(o =>
                    o.User.Username.Contains(search) ||
                    o.User.Email.Contains(search) ||
                    o.Id.ToString().Contains(search));

            var orders = await query
                .OrderByDescending(o => o.CreateDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var orderDtos = new List<OrderDto>();
            foreach (var order in orders)
            {
                orderDtos.Add(await MapOrderToDtoAsync(order));
            }

            return orderDtos;
        }

        public async Task<OrderDto> GetOrderByIdAsync(long orderId)
        {
            var order = await _orderRepository.GetEntitiesQuery()
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Include(o => o.OffCode)
                .FirstOrDefaultAsync(o => o.Id == orderId && !o.IsDelete);

            if (order == null)
                throw new KeyNotFoundException("سفارش یافت نشد");

            return await MapOrderToDtoAsync(order);
        }

        public async Task<List<OrderDto>> SearchOrdersAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<OrderDto>();

            var orders = await _orderRepository.GetEntitiesQuery()
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Include(o => o.OffCode)
                .Where(o =>
                    o.Id.ToString().Contains(query) ||
                    o.User.Username.Contains(query) ||
                    o.User.Email.Contains(query) ||
                    o.OrderDetails.Any(od => od.Product.ProductName.Contains(query)))
                .OrderByDescending(o => o.CreateDate)
                .Take(20)
                .ToListAsync();

            var orderDtos = new List<OrderDto>();
            foreach (var order in orders)
            {
                orderDtos.Add(await MapOrderToDtoAsync(order));
            }

            return orderDtos;
        }

        public async Task<List<OrderDto>> GetTodayOrdersAsync()
        {
            var today = DateTime.Today;

            var orders = await _orderRepository.GetEntitiesQuery()
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Include(o => o.OffCode)
                .Where(o => o.CreateDate.Date == today && !o.IsDelete)
                .OrderByDescending(o => o.CreateDate)
                .ToListAsync();

            var orderDtos = new List<OrderDto>();
            foreach (var order in orders)
            {
                orderDtos.Add(await MapOrderToDtoAsync(order));
            }

            return orderDtos;
        }

        public async Task<bool> DeleteOrderAsync(long orderId)
        {
            var order = await _orderRepository.GetEntityById(orderId);
            if (order == null || order.IsDelete)
                return false;

            order.IsDelete = true;
            _orderRepository.UpdateEntity(order);
            await _orderRepository.SaveChanges();

            return true;
        }
        #endregion

        #region Order Status & Payment
        public async Task<bool> UpdateOrderPaymentStatusAsync(long orderId, bool isPay)
        {
            var order = await _orderRepository.GetEntityById(orderId);
            if (order == null || order.IsDelete)
                return false;

            order.IsPay = isPay;
            order.PaymentDate = isPay ? DateTime.Now : null;

            _orderRepository.UpdateEntity(order);
            await _orderRepository.SaveChanges();

            return true;
        }

        public async Task<bool> MarkOrderAsPaidAsync(long orderId, long refId)
        {
            var order = await _orderRepository.GetEntityById(orderId);
            if (order == null)
                return false;

            order.IsPay = true;
            order.PaymentDate = DateTime.Now;
            order.PaymentRefId = refId;

            _orderRepository.UpdateEntity(order);
            await _orderRepository.SaveChanges();

            if (order.OffCodeId.HasValue)
            {
                await IncreaseOffCodeUsageAsync(orderId, order.UserId);
            }

            return true;
        }

        public async Task CalculateOrderTotalPriceAsync(long orderId, long userId)
        {
            var order = await _orderRepository.GetEntitiesQuery()
                .Include(o => o.OrderDetails)
                .ThenInclude(d => d.Product)
                .ThenInclude(p => p.ProductSelectedCategories)
                .Include(o => o.OffCode)
                .ThenInclude(oc => oc.UserOffCodes)
                .SingleOrDefaultAsync(o => o.Id == orderId && !o.IsDelete);

            if (order == null)
                return;

            decimal subtotal = 0m;
            decimal discountAmount = 0m;

            foreach (var detail in order.OrderDetails.Where(d => !d.IsDelete))
            {
                decimal originalProductTotal = detail.Product.Price * detail.Count;
                decimal discountedProductTotal = detail.Product.FinalPrice * detail.Count;

                subtotal += originalProductTotal;
                decimal productDiscount = originalProductTotal - discountedProductTotal;
                discountAmount += productDiscount;

                if (order.OffCode != null && IsOffCodeApplicable(order.OffCode, userId, detail))
                {
                    decimal offCodeDiscount = discountedProductTotal * (order.OffCode.DiscountPercentage / 100);
                    discountAmount += offCodeDiscount;
                }
            }

            order.Subtotal = subtotal;
            order.DiscountAmount = discountAmount;
            order.TotalPrice = subtotal - discountAmount;

            _orderRepository.UpdateEntity(order);
            await _orderRepository.SaveChanges();
        }
        #endregion

        #region Order Details & Basket
        public async Task AddProductToOrderAsync(long userId, long productId, int count)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            var product = await _productService.GetProductById(productId);

            if (user == null || product == null)
                throw new InvalidOperationException("کاربر یا محصول یافت نشد");

            if (product.NumberofProduct < count)
                throw new InvalidOperationException("موجودی محصول کافی نیست");

            var order = await GetUserOpenOrderAsync(userId);
            var detail = await _orderDetailRepository.GetEntitiesQuery()
                .SingleOrDefaultAsync(d => d.OrderId == order.Id && d.ProductId == productId && !d.IsDelete);

            if (detail != null)
            {
                if (product.NumberofProduct < detail.Count + count)
                    throw new InvalidOperationException("موجودی محصول کافی نیست");

                detail.Count += count;
                detail.Price = product.FinalPrice;
                _orderDetailRepository.UpdateEntity(detail);
            }
            else
            {
                var newDetail = new OrderDetail
                {
                    OrderId = order.Id,
                    ProductId = productId,
                    Count = count,
                    Price = product.FinalPrice,
                    IsDelete = false
                };
                await _orderDetailRepository.AddEntity(newDetail);
            }

            await UpdateProductInventory(new Product()
            {
                Id = product.Id,
                ProductName = product.ProductName,
                Price = product.Price,
                Description = product.Description,
                ShortDescription = product.ShortDescription,
                NumberofProduct = product.NumberofProduct,
                IsExists = product.IsExists,
                IsSpecial = product.IsSpecial,
                ImageName = product.ImageName
            }, -count);
            await _orderDetailRepository.SaveChanges();
            await CalculateOrderTotalPriceAsync(order.Id, order.UserId);

            // ارسال پیامک
            var basketDetails = await GetUserBasketDetailsAsync(userId);
            await _smsService.SendOrderSummarySmsAsync(user.PhoneNumber, basketDetails, order.TotalPrice);
        }

        public async Task<List<OrderDetailsDto>> GetOrderDetailsAsync(long orderId)
        {
            var details = await _orderDetailRepository.GetEntitiesQuery()
                .Include(d => d.Product)
                .Where(d => d.OrderId == orderId && !d.IsDelete)
                .ToListAsync();

            return details.Select(d => new OrderDetailsDto
            {
                Id = d.Id,
                ProductName = d.Product.ProductName,
                Count = d.Count,
                Price = d.Price,
                ImageName = d.Product.ImageName,
                IsDelete = d.IsDelete
            }).ToList();
        }

        public async Task<List<OrderBasketDetail>> GetUserBasketDetailsAsync(long userId)
        {
            var order = await GetUserOpenOrderAsync(userId);
            return order == null ? new List<OrderBasketDetail>() : await GetOrderBasketDetailsAsync(order.Id);
        }

        public async Task<List<OrderBasketDetail>> GetOrderBasketDetailsAsync(long orderId)
        {
            var details = await _orderDetailRepository.GetEntitiesQuery()
                .Include(d => d.Product)
                .Include(d => d.Order)
                .ThenInclude(o => o.OffCode)
                .Where(d => d.OrderId == orderId && !d.IsDelete)
                .ToListAsync();

            return details.Select(d => new OrderBasketDetail
            {
                Id = d.Id,
                Count = d.Count,
                Price = d.Price,
                FinalPrice = d.Product.FinalPrice,
                DiscountPercent = d.Product.DiscountPercent,
                ProductName = d.Product.ProductName,
                ImageName = d.Product.ImageName,
                Color = null,
                Code = null,
                Size = null,
                OffCode = d.Order.OffCode?.Code
            }).ToList();
        }

        public async Task<BasketResponse> GetOrderBasketAsync(long orderId)
        {
            var basketItems = await GetOrderBasketDetailsAsync(orderId);
            var order = await _orderRepository.GetEntityById(orderId);

            return new BasketResponse
            {
                Items = basketItems,
                Subtotal = order?.Subtotal ?? basketItems.Sum(item => item.Price * item.Count),
                DiscountAmount = order?.DiscountAmount ?? 0,
                TotalPrice = order?.TotalPrice ?? basketItems.Sum(item => item.FinalPrice * item.Count)
            };
        }

        public async Task DeleteOrderDetailsAsync(long detailId)
        {
            var detail = await _orderDetailRepository.GetEntitiesQuery()
                .Include(d => d.Order)
                .Include(d => d.Product)
                .SingleOrDefaultAsync(d => d.Id == detailId && !d.IsDelete);

            if (detail != null)
            {
                await UpdateProductInventory(detail.Product, detail.Count);
                _orderDetailRepository.RemoveEntity(detail);
                await _orderDetailRepository.SaveChanges();
                await CalculateOrderTotalPriceAsync(detail.OrderId, detail.Order.UserId);
            }
        }

        public async Task UpdateOrderDetailCountAsync(long detailId, int newCount, long userId)
        {
            var detail = await _orderDetailRepository.GetEntitiesQuery()
                .Include(d => d.Order)
                .Include(d => d.Product)
                .SingleOrDefaultAsync(d => d.Id == detailId && !d.IsDelete);

            if (detail == null || detail.Order.UserId != userId)
                throw new InvalidOperationException("جزئیات سفارش یافت نشد");

            var countDifference = newCount - detail.Count;
            if (detail.Product.NumberofProduct < countDifference)
                throw new InvalidOperationException("موجودی محصول کافی نیست");

            detail.Count = newCount;
            await UpdateProductInventory(detail.Product, -countDifference);

            _orderDetailRepository.UpdateEntity(detail);
            await _orderDetailRepository.SaveChanges();
            await CalculateOrderTotalPriceAsync(detail.OrderId, userId);
        }

        public async Task<OrderDetail> GetOrderDetailWithOrderAsync(long detailId)
        {
            return await _orderDetailRepository.GetEntitiesQuery()
                .Include(d => d.Order)
                .FirstOrDefaultAsync(d => d.Id == detailId && !d.IsDelete);
        }
        #endregion

        #region OffCode Management
        public async Task<bool> ApplyOffCodeAsync(long userId, string code)
        {
            var order = await GetUserOpenOrderAsync(userId);
            if (order == null)
                return false;

            foreach (var detail in order.OrderDetails.Where(d => !d.IsDelete))
            {
                var appliedCode = await _offCodeService.GetActiveOffCodeByCodeAsync(
                    code, userId, detail.ProductId,
                    detail.Product.ProductSelectedCategories.FirstOrDefault()?.ProductCategoriesId);

                if (appliedCode == null)
                    return false;
            }

            var offCode = await _offCodeService.GetActiveOffCodeByCodeAsync(code, userId);
            if (offCode != null)
            {
                order.OffCodeId = offCode.Id;
                _orderRepository.UpdateEntity(order);
                await _orderRepository.SaveChanges();
                await CalculateOrderTotalPriceAsync(order.Id, userId);
                return true;
            }

            return false;
        }

        public async Task IncreaseOffCodeUsageAsync(long orderId, long userId)
        {
            var order = await _orderRepository.GetEntityById(orderId);
            if (order?.OffCodeId != null)
            {
                await _offCodeService.IncreaseUsageAsync(order.OffCodeId.Value, userId);
            }
        }
        #endregion

        #region Statistics & Reports
        public async Task<OrderStatsDto> GetOrderStatsAsync()
        {
            var today = DateTime.Today;

            var totalOrders = await _orderRepository.GetEntitiesQuery()
                .CountAsync(o => !o.IsDelete);

            var totalRevenue = await _orderRepository.GetEntitiesQuery()
                .Where(o => o.IsPay && !o.IsDelete)
                .SumAsync(o => o.TotalPrice);

            var ordersToday = await _orderRepository.GetEntitiesQuery()
                .CountAsync(o => o.CreateDate.Date == today && !o.IsDelete);

            var paidOrdersToday = await _orderRepository.GetEntitiesQuery()
                .CountAsync(o => o.IsPay && o.PaymentDate.Value.Date == today && !o.IsDelete);

            var recentOrders = await GetRecentOrdersAsync(5);

            return new OrderStatsDto
            {
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                TotalOrdersToday = ordersToday,
                PaidOrdersToday = paidOrdersToday,
                RecentOrders = recentOrders
            };
        }
        #endregion

        #region Private Methods
        private async Task<OrderDto> MapOrderToDtoAsync(Order order)
        {
            var orderDetails = await GetOrderDetailsAsync(order.Id);

            return new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                IsPay = order.IsPay,
                PaymentDate = order.PaymentDate,
                TotalPrice = order.TotalPrice,
                OffCode = order.OffCode != null ? new OffCodeResultDto
                {
                    Id = order.OffCode.Id,
                    Code = order.OffCode.Code,
                    DiscountPercentage = order.OffCode.DiscountPercentage, 
                } : null,
                OrderDetails = orderDetails
            };
        }

        private async Task<List<OrderDto>> GetRecentOrdersAsync(int count)
        {
            var orders = await _orderRepository.GetEntitiesQuery()
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Include(o => o.OffCode)
                .Where(o => o.IsPay && !o.IsDelete)
                .OrderByDescending(o => o.PaymentDate)
                .Take(count)
                .ToListAsync();

            var orderDtos = new List<OrderDto>();
            foreach (var order in orders)
            {
                orderDtos.Add(await MapOrderToDtoAsync(order));
            }

            return orderDtos;
        }

        private bool IsOffCodeApplicable(OffCode offCode, long userId, OrderDetail detail)
        {
            bool isOffCodeValid = offCode.ExpireDate >= DateTime.Now &&
                                  (offCode.MaxUsageCount == null || offCode.UsedCount < offCode.MaxUsageCount);

            bool canUseForUser = !offCode.UserOffCodes.Any() ||
                                 offCode.UserOffCodes.Any(u => u.UserId == userId);

            bool hasProductRestriction = offCode.OffCodeProducts.Any() || offCode.OffCodeCategories.Any();
            bool canApply = !hasProductRestriction ||
                            offCode.OffCodeProducts.Any(p => p.ProductId == detail.ProductId) ||
                            offCode.OffCodeCategories.Any(c => detail.Product.ProductSelectedCategories.Any(pc => pc.ProductCategoriesId == c.CategoryId));

            return isOffCodeValid && canUseForUser && canApply;
        }

        private async Task UpdateProductInventory(Product product, int quantityChange)
        {
            product.NumberofProduct += quantityChange;
            await _productService.UpdateProduct(new EditProductDTO
            {
                Id = product.Id,
                ProductName = product.ProductName,
                Price = product.Price,
                Description = product.Description,
                ShortDescription = product.ShortDescription,
                NumberofProduct = product.NumberofProduct,
                IsExists = product.IsExists,
                IsSpecial = product.IsSpecial,
                CurrentImage = product.ImageName
            });
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