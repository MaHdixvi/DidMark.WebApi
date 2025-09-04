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
        Task AddProduct(AddProductDTO product);
        Task UpdateProduct(EditProductDTO product);
        Task<FilterProductsDTO> FilterProducts(FilterProductsDTO filter);
        Task<Product> GetProductById(long productId);
        Task<List<Product>> GetRelatedProducts(long productId);
        Task<bool> IsExistsProductById(long productId);
        Task<Product> GetProductByUserOrder(long productId);
        Task<EditProductDTO> GetProductForEdit(long productId);
        #endregion
        #region product categories
        Task<List<ProductCategories>> GetAllActiveProductCategories();


        /// <summary>
        /// اضافه کردن دسته‌بندی به محصول
        /// </summary>
        Task AddCategoryToProduct(long productId, long categoryId);

        /// <summary>
        /// جایگزین کردن دسته‌بندی‌های یک محصول (مثلاً هنگام ویرایش محصول)
        /// </summary>
        Task UpdateProductCategories(long productId, List<long> categoryIds);

        /// <summary>
        /// گرفتن لیست دسته‌بندی‌های یک محصول
        /// </summary>
        Task<List<ProductCategories>> GetProductCategories(long productId);

        /// <summary>
        /// حذف دسته‌بندی از محصول
        /// </summary>
        Task RemoveCategoryFromProduct(long productId, long categoryId);

        /// <summary>
        /// چک کردن اینکه آیا محصول داخل یک دسته خاص هست یا نه
        /// </summary>
        Task<bool> IsProductInCategory(long productId, long categoryId);

        #endregion
        #region Product Gallery
        Task<List<ProductGalleries>> GetProductActiveGalleries(long productId);
        #endregion
    }
}
