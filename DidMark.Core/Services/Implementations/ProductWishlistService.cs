using DidMark.Core.DTO.Products;
using DidMark.Core.DTO.Products.ProductWishList;
using DidMark.Core.Services.Interfaces;
using DidMark.DataLayer.Entities.Product;
using DidMark.DataLayer.Entities.Products;
using DidMark.DataLayer.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DidMark.Core.Services.Implementations
{
    public class ProductWishlistService : IProductWishlistService
    {
        private readonly IGenericRepository<ProductWishlist> _wishlistRepository;
        private readonly IGenericRepository<Product> _productRepository;

        public ProductWishlistService(
            IGenericRepository<ProductWishlist> wishlistRepository,
            IGenericRepository<Product> productRepository)
        {
            _wishlistRepository = wishlistRepository;
            _productRepository = productRepository;
        }

        public async Task<bool> AddToWishlist(long userId, long productId)
        {
            try
            {
                // بررسی وجود محصول
                var productExists = await _productRepository.GetEntitiesQuery()
                    .AnyAsync(p => p.Id == productId && !p.IsDelete && p.IsExists);
                if (!productExists) return false;

                // بررسی آیا قبلاً اضافه شده
                var existing = await _wishlistRepository.GetEntitiesQuery()
                    .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId && !w.IsDelete);

                if (existing != null) return true; // قبلاً اضافه شده

                var wishlistItem = new ProductWishlist
                {
                    UserId = userId,
                    ProductId = productId,
                    CreateDate = DateTime.Now,
                    IsDelete = false
                };

                await _wishlistRepository.AddEntity(wishlistItem);
                await _wishlistRepository.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveFromWishlist(long userId, long productId)
        {
            try
            {
                var wishlistItem = await _wishlistRepository.GetEntitiesQuery()
                    .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId && !w.IsDelete);

                if (wishlistItem == null) return false;

                wishlistItem.IsDelete = true;
                _wishlistRepository.UpdateEntity(wishlistItem);
                await _wishlistRepository.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ToggleWishlist(long userId, long productId)
        {
            try
            {
                var existing = await _wishlistRepository.GetEntitiesQuery()
                    .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId && !w.IsDelete);

                if (existing != null)
                {
                    // حذف از علاقه‌مندی
                    return await RemoveFromWishlist(userId, productId);
                }
                else
                {
                    // اضافه به علاقه‌مندی
                    return await AddToWishlist(userId, productId);
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> IsInWishlist(long userId, long productId)
        {
            return await _wishlistRepository.GetEntitiesQuery()
                .AnyAsync(w => w.UserId == userId && w.ProductId == productId && !w.IsDelete);
        }

        public async Task<List<ProductWishlistDto>> GetUserWishlist(long userId)
        {
            var wishlistItems = await _wishlistRepository.GetEntitiesQuery()
                .Where(w => w.UserId == userId && !w.IsDelete)
                .Include(w => w.Product)
                .OrderByDescending(w => w.CreateDate)
                .ToListAsync();

            return wishlistItems.Select(w => new ProductWishlistDto
            {
                Id = w.Id,
                UserId = w.UserId,
                ProductId = w.ProductId,
                CreateDate = w.CreateDate,
                ProductName = w.Product?.ProductName,
                ProductImage = w.Product?.ImageName,
                Price = w.Product?.Price ?? 0,
                FinalPrice = w.Product?.FinalPrice ?? 0,
                IsExists = w.Product?.IsExists ?? false,
                IsSpecial = w.Product?.IsSpecial ?? false,
                DiscountPercent = w.Product?.DiscountPercent
            }).ToList();
        }

        public async Task<int> GetWishlistItemCount(long userId)
        {
            return await _wishlistRepository.GetEntitiesQuery()
                .CountAsync(w => w.UserId == userId && !w.IsDelete);
        }

        public async Task<bool> ClearWishlist(long userId)
        {
            try
            {
                var wishlistItems = await _wishlistRepository.GetEntitiesQuery()
                    .Where(w => w.UserId == userId && !w.IsDelete)
                    .ToListAsync();

                foreach (var item in wishlistItems)
                {
                    item.IsDelete = true;
                    _wishlistRepository.UpdateEntity(item);
                }

                await _wishlistRepository.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<WishlistStatsDTO> GetWishlistStats(long userId, long? productId = null)
        {
            var totalItems = await GetWishlistItemCount(userId);

            bool isInWishlist = false;
            if (productId.HasValue)
            {
                isInWishlist = await IsInWishlist(userId, productId.Value);
            }

            return new WishlistStatsDTO
            {
                TotalItems = totalItems,
                IsInWishlist = isInWishlist
            };
        }

        #region Dispose
        public void Dispose()
        {
            _wishlistRepository?.Dispose();
            _productRepository?.Dispose();
        }
        #endregion
    }
}