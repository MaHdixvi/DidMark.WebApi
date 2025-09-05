using DidMark.Core.DTO.Products;
using DidMark.Core.DTO.Products.NewFolder;
using DidMark.Core.DTO.Products.ProductGalleries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DidMark.Core.Services.Interfaces
{
    public interface IProductGalleryService
    {
        Task<bool> AddProductGallery(AddProductGalleryDTO dto);
        //Task<bool> EditProductGallery(EditProductGalleryDTO dto, long galleryId);
        Task<bool> DeleteProductGallery(long galleryId, long productId);

        Task<ICollection<ProductGalleryDTO>> GetProductGalleriesByProductId(long productId);
    }
}
