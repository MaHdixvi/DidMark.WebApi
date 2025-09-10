using DidMark.Core.DTO.OffCodes;
using DidMark.Core.DTO.Orders;
using DidMark.Core.DTO.Products;
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
        private readonly IOffCodeService _offCodeService;
        #endregion

        #region Constructor
        public OrderService(
            IGenericRepository<Order> orderRepository,
            IGenericRepository<OrderDetail> orderDetailRepository,
            IUserService userService,
            IProductService productService,
            ISmsService smsService,
            IOffCodeService offCodeService
            )
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _orderDetailRepository = orderDetailRepository ?? throw new ArgumentNullException(nameof(orderDetailRepository));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _smsService = smsService ?? throw new ArgumentNullException(nameof(smsService));
            _offCodeService = offCodeService ?? throw new ArgumentNullException(nameof(offCodeService));
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
                OrderDetails = new List<OrderDetail>(),
                TotalPrice = 0m,
                Subtotal = 0m,
                DiscountAmount = 0m 
            };

            await _orderRepository.AddEntity(order);
            await _orderRepository.SaveChanges();

            // تبدیل به DTO
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

            if (order == null)
            {
                order = await CreateOrderUserAsync(userId);
            }

            //return new OrderDto
            //{
            //    Id = order.Id,
            //    IsPay = order.IsPay,
            //    PaymentDate = order.PaymentDate,
            //    TotalPrice = order.OrderDetails.Where(d => !d.IsDelete)
            //                                   .Sum(d => d.Price * d.Count
            //                                        * (order.OffCode != null && order.OffCode.ExpireDate >= DateTime.Now
            //                                           && (order.OffCode.MaxUsageCount == null || order.OffCode.UsedCount < order.OffCode.MaxUsageCount)
            //                                           ? (1 - order.OffCode.DiscountPercentage / 100) : 1)),
            //    OffCode = order.OffCode == null ? null : new OffCodeResultDto
            //    {
            //        Code = order.OffCode.Code,
            //        DiscountPercentage = order.OffCode.DiscountPercentage,
            //        ExpireDate = order.OffCode.ExpireDate
            //    },
            //    OrderDetails = order.OrderDetails.Where(d => !d.IsDelete).Select(d => new OrderDetailsDto
            //    {
            //        Id = d.Id,
            //        ProductName = d.Product.ProductName,
            //        Count = d.Count,
            //        Price = d.Price,
            //        ImageName = PathTools.Domain + PathTools.ProductImagePath + d.Product.ImageName
            //    }).ToList()
            //};
            return order;
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


            decimal subtotal = 0m;
            decimal discountAmount = 0m;

            foreach (var detail in order.OrderDetails.Where(d => !d.IsDelete))
            {
                decimal productTotal = detail.Price * detail.Count;
                subtotal += productTotal;

                if (order.OffCode != null)
                {
                    // بررسی اعتبار کلی کد تخفیف
                    bool isOffCodeValid = order.OffCode.ExpireDate >= DateTime.Now &&
                                          (order.OffCode.MaxUsageCount == null || order.OffCode.UsedCount < order.OffCode.MaxUsageCount);

                    // بررسی اختصاص کد به کاربران خاص
                    bool canUseForUser = !order.OffCode.UserOffCodes.Any() ||
                                         order.OffCode.UserOffCodes.Any(u => u.UserId == userId);

                    // بررسی محدودیت محصول/دسته‌بندی
                    bool hasProductRestriction = order.OffCode.OffCodeProducts.Any() || order.OffCode.OffCodeCategories.Any();
                    bool canApply = !hasProductRestriction ||
                                    order.OffCode.OffCodeProducts.Any(p => p.ProductId == detail.ProductId) ||
                                    order.OffCode.OffCodeCategories.Any(c => detail.Product.ProductSelectedCategories.Any(pc => pc.ProductCategoriesId == c.CategoryId));

                    if (isOffCodeValid && canUseForUser && canApply)
                    {
                        discountAmount += productTotal * (order.OffCode.DiscountPercentage / 100);
                    }
                }
            }

            order.Subtotal = subtotal;
            order.DiscountAmount = discountAmount;
            order.TotalPrice = subtotal - discountAmount;

            _orderRepository.UpdateEntity(order);
            await _orderRepository.SaveChanges();
        }


        #endregion

        #region Order Detail
        public async Task AddProductToOrderAsync(long userId, long productId, int count)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            var product = await _productService.GetProductById(productId);

            if (user == null || product == null) return;

            // بررسی اینکه موجودی کافی وجود داشته باشه
            if (product.NumberofProduct < count)
            {
                // می‌تونی اینجا Exception بزنی یا return کنی
                throw new InvalidOperationException("موجودی محصول کافی نیست");
            }

            var order = await GetUserOpenOrderAsync(userId);
            //if (count > 1) count = 1;

            var detail = await _orderDetailRepository.GetEntitiesQuery()
                .SingleOrDefaultAsync(d => d.OrderId == order.Id && d.ProductId == productId && !d.IsDelete);

            if (detail != null)
            {
                // دوباره هم باید چک کنیم موجودی کافی باشه
                if (product.NumberofProduct < detail.Count + count)
                    throw new InvalidOperationException("موجودی محصول کافی نیست");
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
                    Price = (int)product.FinalPrice
                };
                await _orderDetailRepository.AddEntity(newDetail);
            }

            // کم کردن موجودی محصول
            product.NumberofProduct -= count;
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
                CurrentImage = product.ImageName // اگر نیاز بود
            });

            await _orderDetailRepository.SaveChanges();

            // ارسال پیامک با جزئیات سفارش
            var basketDetails = await GetUserBasketDetailsAsync(userId);
            await CalculateOrderTotalPriceAsync(order.Id, order.UserId);
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
                ImageName = PathTools.Domain + PathTools.ProductImagePath + d.Product.ImageName
            }).ToList();
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
                .Include(d => d.Order)
                .SingleOrDefaultAsync(d => d.Id == detailId && !d.IsDelete);

            if (detail != null)
            {
                _orderDetailRepository.RemoveEntity(detail);
                await _orderDetailRepository.SaveChanges();
                await CalculateOrderTotalPriceAsync(detail.OrderId, detail.Order.UserId);
            }
        }
        #endregion


        #region OffCode
        //public async Task<bool> ApplyOffCodeAsync(long userId, string code)
        //{
        //    var order = await GetUserOpenOrderAsync(userId);
        //    if (order == null) return false;

        //    // بررسی اعتبار کد تخفیف
        //    var offCode = await _offCodeService.GetActiveOffCodeByCodeAsync(code);
        //    if (offCode == null) return false;

        //    // ست کردن کد تخفیف روی سفارش
        //    order.OffCodeId = offCode.Id;
        //    _orderRepository.UpdateEntity(order);
        //    await _orderRepository.SaveChanges();

        //    return true;
        //}
        public async Task<bool> ApplyOffCodeAsync(long userId, string code)
        {
            var order = await GetUserOpenOrderAsync(userId);
            if (order == null) return false;

            // بررسی اعتبار کد تخفیف با توجه به userId و محصولات سفارش
            OffCodeResultDto? appliedCode = null;
            foreach (var detail in order.OrderDetails.Where(d => !d.IsDelete))
            {
                appliedCode = await _offCodeService.GetActiveOffCodeByCodeAsync(
                    code,
                    userId,
                    productId: detail.ProductId,
                    categoryId: detail.Product.ProductSelectedCategories.FirstOrDefault()?.ProductCategoriesId
                );

                if (appliedCode == null)
                    return false; // یکی از محصولات اجازه استفاده از کد را ندارد
            }

            // ست کردن کد تخفیف روی سفارش
            order.OffCodeId = appliedCode.Id;
            _orderRepository.UpdateEntity(order);
            await _orderRepository.SaveChanges();

            return true;
        }

        public async Task IncreaseOffCodeUsageAsync(long orderId, long userId)
        {
            var order = await _orderRepository.GetEntityById(orderId);
            if (order == null || order.OffCodeId == null) return;

            await _offCodeService.IncreaseUsageAsync(order.OffCodeId.Value, userId);
        }

        #endregion
        #region Payment Status
        /// <summary>
        /// آپدیت وضعیت پرداخت سفارش
        /// </summary>
        /// <param name="orderId">شناسه سفارش</param>
        /// <param name="refId">شماره پیگیری پرداخت</param>
        public async Task<bool> MarkOrderAsPaidAsync(long orderId, long refId)
        {
            var order = await _orderRepository.GetEntityById(orderId);
            if (order == null) return false;

            order.IsPay = true;
            order.PaymentDate = DateTime.Now;
            order.PaymentRefId = refId; // اگر فیلدی برای RefId پرداخت داری

            _orderRepository.UpdateEntity(order);
            await _orderRepository.SaveChanges();
            return true;
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
