using DidMark.Core.DTO.Products;
using DidMark.Core.DTO.Products.ProductWishList;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DidMark.Core.Services.Interfaces
{
    public interface IProductWishlistService
    {
        Task<bool> AddToWishlist(long userId, long productId);
        Task<bool> RemoveFromWishlist(long userId, long productId);
        Task<bool> ToggleWishlist(long userId, long productId);
        Task<bool> IsInWishlist(long userId, long productId);
        Task<List<ProductWishlistDto>> GetUserWishlist(long userId);
        Task<int> GetWishlistItemCount(long userId);
        Task<bool> ClearWishlist(long userId);
        Task<WishlistStatsDTO> GetWishlistStats(long userId, long? productId = null);

        void Dispose();
    }
}