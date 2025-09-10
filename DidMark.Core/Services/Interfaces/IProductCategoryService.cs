using DidMark.Core.DTO.Products;
using DidMark.Core.DTO.Products.ProductCategory;
using DidMark.DataLayer.Entities.Product;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DidMark.Core.Services.Interfaces
{
    public interface IProductCategoryService
    {
        // ProductCategories CRUD
        Task<List<ProductCategoryDTO>> GetAllActiveCategories();
        Task<ProductCategoryDTO> GetCategoryById(long id);
        Task AddCategory(AddProductCategoryDTO dto);
        Task UpdateCategory(EditProductCategoryDTO dto);
        Task<bool> DeleteCategory(long id);
        Task<List<ProductCategoryDTO>> GetRootCategories();
        Task<List<ProductCategoryDTO>> GetChildCategories(long parentId);
        Task<List<AttributeDto>> GetCategoryAttributesAsync(int categoryId);


        // مدیریت محصولات مرتبط با دسته
        Task<List<ProductCategoryDTO>> GetCategoriesOfProduct(long productId);
        Task AddCategoryToProduct(long productId, long categoryId);
        Task RemoveCategoryFromProduct(long productId, long categoryId);
        Task UpdateProductCategories(long productId, List<long> categoryIds);
        Task<bool> IsProductInCategory(long productId, long categoryId);
    }
}
