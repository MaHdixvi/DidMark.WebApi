using DidMark.Core.DTO.Products;
using DidMark.DataLayer.Entities.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.Services.Interfaces
{
    public interface IProductService : IDisposable
    {
        #region product 
        Task AddProduct(Product product);
        Task UpdateProduct(Product product);
        Task<FilterProductsDTO> FilterProducts(FilterProductsDTO filter);
        Task<Product> GetProductById(long productId);
        Task<List<Product>> GetRelatedProducts(long productId);
        Task<bool> IsExistsProductById(long productId);
        Task<Product> GetProductByUserOrder(long productId);
        Task<EditProductDTO> GetProductForEdit(long productId);
        Task EditProduct(EditProductDTO product);
        #endregion
        #region product categories
        Task<List<ProductCategories>> GetAllActiveProductCategories();

        #endregion
        #region Product Gallery
        Task<List<ProductGalleries>> GetProductActiveGalleries(long productId);
        #endregion
    }
}
