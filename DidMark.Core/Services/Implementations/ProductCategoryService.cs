using DidMark.Core.DTO.Products;
using DidMark.Core.DTO.Products.ProductCategory;
using DidMark.Core.Services.Interfaces;
using DidMark.DataLayer.Entities.Product;
using DidMark.DataLayer.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DidMark.Core.Services.Implementations
{
    public class ProductCategoryService : IProductCategoryService, IDisposable
    {
        private readonly IGenericRepository<ProductCategories> _categoryRepo;
        private readonly IGenericRepository<ProductSelectedCategories> _productCategoryRepo;
        private readonly IGenericRepository<PAttribute> _producyAttributeRepo;

        public ProductCategoryService(
            IGenericRepository<ProductCategories> categoryRepo,
            IGenericRepository<ProductSelectedCategories> productCategoryRepo,
            IGenericRepository<PAttribute> productAttributeRepo)
        {
            _categoryRepo = categoryRepo;
            _productCategoryRepo = productCategoryRepo;
            _producyAttributeRepo = productAttributeRepo;
        }

        #region Category CRUD

        public async Task<List<ProductCategoryDTO>> GetAllActiveCategories()
        {
            return await _categoryRepo.GetEntitiesQuery()
               .Where(c => !c.IsDelete)
               .Select(c => new ProductCategoryDTO
               {
                   Id = c.Id,
                   Title = c.Title,
                   UrlTitle = c.UrlTitle,
                   ParentId = c.ParentId
               })
               .ToListAsync();
        }

        public async Task<ProductCategoryDTO> GetCategoryById(long id)
        {
            return await _categoryRepo.GetEntitiesQuery()
            .Where(c => c.Id == id && !c.IsDelete)
            .Select(c => new ProductCategoryDTO
            {
                Id = c.Id,
                Title = c.Title,
                UrlTitle = c.UrlTitle,
                ParentId = c.ParentId
            })
            .FirstOrDefaultAsync();
        }

        public async Task AddCategory(AddProductCategoryDTO dto)
        {
            var entity = new ProductCategories
            {
                Title = dto.Title,
                UrlTitle = dto.UrlTitle,
                ParentId = dto.ParentId,
                CreateDate = DateTime.Now,
                IsDelete = false
            };
            await _categoryRepo.AddEntity(entity);
            await _categoryRepo.SaveChanges();
        }

        public async Task UpdateCategory(EditProductCategoryDTO dto)
        {
            var entity = await _categoryRepo.GetEntityById(dto.Id);
            if (entity == null) return;

            entity.Title = dto.Title;
            entity.UrlTitle = dto.UrlTitle;
            entity.ParentId = dto.ParentId;

            _categoryRepo.UpdateEntity(entity);
            await _categoryRepo.SaveChanges();
        }

        public async Task<bool> DeleteCategory(long id)
        {
            // پیدا کردن دسته‌بندی اصلی
            var entity = await _categoryRepo.GetEntityById(id);
            if (entity == null) return false;

            // فراخوانی متد بازگشتی برای حذف تمام زیرمجموعه‌ها
            await DeleteCategoryRecursive(entity);

            await _categoryRepo.SaveChanges();
            return true;
        }

        // متد بازگشتی برای حذف دسته‌بندی و تمام زیرمجموعه‌ها
        private async Task DeleteCategoryRecursive(ProductCategories category)
        {
            // پیدا کردن زیرمجموعه‌های مستقیم
            var children = await _categoryRepo.GetEntitiesQuery()
                .Where(c => c.ParentId == category.Id && !c.IsDelete)
                .ToListAsync();

            // حذف تمام زیرمجموعه‌ها به صورت بازگشتی
            foreach (var child in children)
            {
                await DeleteCategoryRecursive(child);
            }

            // حذف دسته‌بندی فعلی
            category.IsDelete = true;
            _categoryRepo.UpdateEntity(category);
        }

        // دسته‌بندی‌های سطح اول (بدون Parent)
        public async Task<List<ProductCategoryDTO>> GetRootCategories()
        {
            return await _categoryRepo.GetEntitiesQuery()
                .Where(c => !c.IsDelete && c.ParentId == null)
                .Select(c => new ProductCategoryDTO
                {
                    Id = c.Id,
                    Title = c.Title,
                    UrlTitle = c.UrlTitle,
                    ParentId = c.ParentId
                })
                .ToListAsync();
        }

        // دسته‌بندی‌های فرزند یک دسته‌بندی مشخص
        public async Task<List<ProductCategoryDTO>> GetChildCategories(long parentId)
        {
            return await _categoryRepo.GetEntitiesQuery()
             .Where(c => !c.IsDelete && c.ParentId == parentId)
             .Select(c => new ProductCategoryDTO
             {
                 Id = c.Id,
                 Title = c.Title,
                 UrlTitle = c.UrlTitle,
                 ParentId = c.ParentId
             })
             .ToListAsync();
        }


                public async Task<List<AttributeDto>> GetCategoryAttributesAsync(int categoryId)
        {
            return await _producyAttributeRepo.GetEntitiesQuery()
                .Where(a => a.CategoryId == categoryId)
                .Select(a => new AttributeDto
                {
                    Id = a.Id,
                    Name = a.Name
                })
                .ToListAsync();
        }

        #endregion

        #region Product-Category Management

        public async Task<List<ProductCategoryDTO>> GetCategoriesOfProduct(long productId)
        {
            return await _productCategoryRepo.GetEntitiesQuery()
              .Where(pc => pc.ProductId == productId && !pc.IsDelete)
              .Include(pc => pc.ProductCategories)
              .Select(pc => new ProductCategoryDTO
              {
                  Id = pc.ProductCategories.Id,
                  Title = pc.ProductCategories.Title,
                  UrlTitle = pc.ProductCategories.UrlTitle,
                  ParentId = pc.ProductCategories.ParentId
              })
              .ToListAsync();
        }

        public async Task AddCategoryToProduct(long productId, long categoryId)
        {
            var exists = await _productCategoryRepo.GetEntitiesQuery()
                .AnyAsync(pc => pc.ProductId == productId && pc.ProductCategoriesId == categoryId);
            if (exists) return;

            var entity = new ProductSelectedCategories
            {
                ProductId = productId,
                ProductCategoriesId = categoryId,
                CreateDate = DateTime.Now,
                IsDelete = false
            };
            await _productCategoryRepo.AddEntity(entity);
            await _productCategoryRepo.SaveChanges();
        }

        public async Task RemoveCategoryFromProduct(long productId, long categoryId)
        {
            var entity = await _productCategoryRepo.GetEntitiesQuery()
                .FirstOrDefaultAsync(pc => pc.ProductId == productId && pc.ProductCategoriesId == categoryId);
            if (entity == null) return;

            _productCategoryRepo.RemoveEntity(entity);
            await _productCategoryRepo.SaveChanges();
        }

        public async Task UpdateProductCategories(long productId, List<long> categoryIds)
        {
            var existing = await _productCategoryRepo.GetEntitiesQuery()
                .Where(pc => pc.ProductId == productId)
                .ToListAsync();

            _productCategoryRepo.DeleteEntities(existing);

            var newEntities = categoryIds.Select(c => new ProductSelectedCategories
            {
                ProductId = productId,
                ProductCategoriesId = c,
                CreateDate = DateTime.Now,
                IsDelete = false
            }).ToList();

            await _productCategoryRepo.AddRange(newEntities);
            await _productCategoryRepo.SaveChanges();
        }

        public async Task<bool> IsProductInCategory(long productId, long categoryId)
        {
            return await _productCategoryRepo.GetEntitiesQuery()
                .AnyAsync(pc => pc.ProductId == productId && pc.ProductCategoriesId == categoryId && !pc.IsDelete);
        }

        #endregion

        public void Dispose()
        {
            _categoryRepo?.Dispose();
            _productCategoryRepo?.Dispose();
        }
    }
}
