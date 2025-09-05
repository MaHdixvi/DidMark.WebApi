using DidMark.Core.DTO.Products;
using DidMark.DataLayer.Entities.Product;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DidMark.Core.Services.Interfaces
{
    public interface IProductService : IDisposable
    {
        #region Product Management

        Task AddProduct(AddProductDTO product);
        Task UpdateProduct(EditProductDTO product);
        Task<bool> DeleteProduct(long productId);
        Task<FilterProductsDTO> FilterProducts(FilterProductsDTO filter);
        Task<Product> GetProductById(long productId);
        Task<List<Product>> GetRelatedProducts(long productId);
        Task<bool> IsExistsProductById(long productId);
        Task<Product> GetProductByUserOrder(long productId);
        Task<EditProductDTO> GetProductForEdit(long productId);

        #endregion

        #region Product Gallery

        Task<List<ProductGalleries>> GetProductActiveGalleries(long productId);

        #endregion

    }
}
