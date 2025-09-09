using DidMark.Core.DTO.Products;
using DidMark.DataLayer.Entities.Product;
using Microsoft.AspNetCore.Mvc;
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
        Task<ProductDto> GetProductById(long productId);
        Task<List<ProductDto>> GetRelatedProducts(long productId);
        Task<bool> IsExistsProductById(long productId);
        Task<ProductDto> GetProductByUserOrder(long productId);
        Task<EditProductDTO> GetProductForEdit(long productId);

        #endregion

        #region Product Gallery

        Task<List<ProductGalleries>> GetProductActiveGalleries(long productId);

        #endregion
        Task AddProductVisit(long productId, string userIp);
        Task<List<ProductVisitDto>> GetProductVisits(long productId);
    }
}
