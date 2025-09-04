using DidMark.Core.DTO.Paging;
using DidMark.Core.DTO.Products;
using DidMark.Core.Services.Interfaces;
using DidMark.Core.Utilities.Common;
using DidMark.Core.Utilities.Extensions.FileExtentions;
using DidMark.Core.Utilities.Extensions.Paging;
using DidMark.DataLayer.Entities.Product;
using DidMark.DataLayer.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static DidMark.Core.DTO.Products.FilterProductsDTO;

namespace DidMark.Core.Services.Implementations
{
    public class ProductService : IProductService
    {
        #region constructor
        private readonly IGenericRepository<Product> productRepository;
        private readonly IGenericRepository<ProductCategories> productCategoriesRepository;
        private readonly IGenericRepository<ProductGalleries> productGalleriesRepository;
        private readonly IGenericRepository<ProductVisit> productVisitrepository;
        private readonly IGenericRepository<ProductSelectedCategories> productSelectedCategoriesRepository;

        public ProductService(
            IGenericRepository<Product> productRepository,
            IGenericRepository<ProductCategories> productCategoriesRepository,
            IGenericRepository<ProductGalleries> productGalleriesRepository,
            IGenericRepository<ProductVisit> productVisitrepository,
            IGenericRepository<ProductSelectedCategories> productSelectedCategoriesRepository)
        {
            this.productRepository = productRepository;
            this.productCategoriesRepository = productCategoriesRepository;
            this.productGalleriesRepository = productGalleriesRepository;
            this.productVisitrepository = productVisitrepository;
            this.productSelectedCategoriesRepository = productSelectedCategoriesRepository;
        }
        #endregion

        #region product categories
        public async Task<List<ProductCategories>> GetAllActiveProductCategories()
        {
            return await productCategoriesRepository.GetEntitiesQuery()
                .Where(s => !s.IsDelete)
                .ToListAsync();
        }
        #endregion

        #region ProductGallery
        public async Task<List<ProductGalleries>> GetProductActiveGalleries(long productId)
        {
            return await productGalleriesRepository
                .GetEntitiesQuery()
                .Where(s => s.ProductId == productId && !s.IsDelete)
                .Select(s => new ProductGalleries
                {
                    ProductId = s.ProductId,
                    Id = s.Id,
                    ImageName = s.ImageName,
                    CreateDate = s.CreateDate
                })
                .ToListAsync();
        }
        #endregion

        #region product 
        public async Task AddProduct(AddProductDTO productDto)
        {
            var product = new Product
            {
                ProductName = productDto.ProductName,
                Description = productDto.Description,
                Color = productDto.Color,
                Size = productDto.Size,
                Code = productDto.Code,
                NumberofProduct = productDto.NumberofProduct,
                Price = productDto.Price,
                ShortDescription = productDto.ShortDescription,
                IsExists = productDto.IsExists,
                IsSpecial = productDto.IsSpecial,
                ImageName = productDto.ImageName,
                CreateDate = DateTime.Now,
                IsDelete = false
            };

            await productRepository.AddEntity(product);
            await productRepository.SaveChanges();
        }

        public async Task UpdateProduct(EditProductDTO productDto)
        {
            var product = await productRepository.GetEntityById(productDto.Id);
            if (product == null) return;

            product.ProductName = productDto.ProductName;
            product.Description = productDto.Description;
            product.ShortDescription = productDto.ShortDescription;
            product.IsExists = productDto.IsExists;
            product.IsSpecial = productDto.IsSpecial;
            product.Price = (int)productDto.Price;

            if (!string.IsNullOrEmpty(productDto.Base64Image))
            {
                var imageFile = imageUploaderExtention.Base64ToImage(productDto.Base64Image);
                var imageName = Guid.NewGuid().ToString("N") + ".jpeg";
                imageFile.AddImageToServer(imageName, PathTools.ProductImagePath, product.ImageName);
                product.ImageName = imageName;
            }

            productRepository.UpdateEntity(product);
            await productRepository.SaveChanges();
        }

        public async Task<FilterProductsDTO> FilterProducts(FilterProductsDTO filter)
        {
            var productsQuery = productRepository.GetEntitiesQuery().AsQueryable();

            // Ordering
            if (filter.OrderBy != null)
            {
                switch (filter.OrderBy)
                {
                    case ProductOrderBy.PriceAsc:
                        productsQuery = productsQuery.OrderBy(x => x.Price);
                        break;
                    case ProductOrderBy.PriceDes:
                        productsQuery = productsQuery.OrderByDescending(x => x.Price);
                        break;
                    case ProductOrderBy.CreateDataAsc:
                        productsQuery = productsQuery.OrderBy(x => x.CreateDate);
                        break;
                    case ProductOrderBy.CreateDataDes:
                        productsQuery = productsQuery.OrderByDescending(x => x.CreateDate);
                        break;
                    case ProductOrderBy.IsSpecial:
                        productsQuery = productsQuery.OrderByDescending(x => x.IsSpecial);
                        break;
                }
            }

            // Filters
            if (!string.IsNullOrEmpty(filter.Title))
                productsQuery = productsQuery.Where(s => s.ProductName.Contains(filter.Title));

            if (filter.StartPrice > 0)
                productsQuery = productsQuery.Where(s => s.Price >= filter.StartPrice);

            if (filter.EndPrice > 0)
                productsQuery = productsQuery.Where(s => s.Price <= filter.EndPrice);

            if (filter.Categories != null && filter.Categories.Any())
            {
                productsQuery = productsQuery
                    .SelectMany(s => s.ProductSelectedCategories
                        .Where(f => filter.Categories.Contains(f.ProductCategoriesId))
                        .Select(t => t.Product));
            }

            // Paging
            var count = (int)Math.Ceiling(productsQuery.Count() / (double)filter.TakeEntity);
            var pager = Pager.Build(count, filter.PageId, filter.TakeEntity);

            var products = await productsQuery.Paging(pager).ToListAsync();

            return filter.SetProducts(products).SetPaging(pager);
        }

        public async Task<Product> GetProductById(long productId)
        {
            return await productRepository.GetEntitiesQuery()
                .AsQueryable()
                .SingleOrDefaultAsync(s => !s.IsDelete && s.Id == productId);
        }

        public async Task<List<Product>> GetRelatedProducts(long productId)
        {
            var product = await productRepository.GetEntityById(productId);
            if (product == null) return null;

            var productCategoriesList = await productSelectedCategoriesRepository.GetEntitiesQuery()
                .Where(s => s.ProductId == productId)
                .Select(f => f.ProductCategoriesId)
                .ToListAsync();

            var relatedProducts = await productRepository.GetEntitiesQuery()
                .SelectMany(s => s.ProductSelectedCategories
                    .Where(f => productCategoriesList.Contains(f.ProductCategoriesId))
                    .Select(t => t.Product))
                .Where(s => s.Id != productId)
                .OrderByDescending(s => s.CreateDate)
                .Take(4)
                .ToListAsync();

            return relatedProducts;
        }

        public async Task<bool> IsExistsProductById(long productId)
        {
            return await productRepository.GetEntitiesQuery()
                .AnyAsync(s => s.Id == productId && !s.IsDelete);
        }

        public async Task<Product> GetProductByUserOrder(long productId)
        {
            return await productRepository.GetEntitiesQuery()
                .SingleOrDefaultAsync(p => p.Id == productId && !p.IsDelete);
        }

        public async Task<EditProductDTO> GetProductForEdit(long productId)
        {
            var product = await productRepository.GetEntitiesQuery()
                .AsQueryable()
                .SingleOrDefaultAsync(p => p.Id == productId);

            if (product == null) return null;

            return new EditProductDTO
            {
                Id = product.Id,
                CurrentImage = product.ImageName,
                Description = product.Description,
                IsExists = product.IsExists,
                IsSpecial = product.IsSpecial,
                Price = product.Price,
                ProductName = product.ProductName,
                ShortDescription = product.ShortDescription
            };
        }
        #endregion

        #region Product Selected Categories

        public async Task AddCategoryToProduct(long productId, long categoryId)
        {
            var exists = await productSelectedCategoriesRepository.GetEntitiesQuery()
                .AnyAsync(s => s.ProductId == productId && s.ProductCategoriesId == categoryId);

            if (exists) return;

            var entity = new ProductSelectedCategories
            {
                ProductId = productId,
                ProductCategoriesId = categoryId,
                CreateDate = DateTime.Now,
                IsDelete = false
            };

            await productSelectedCategoriesRepository.AddEntity(entity);
            await productSelectedCategoriesRepository.SaveChanges();
        }

        public async Task UpdateProductCategories(long productId, List<long> categoryIds)
        {
            var existingCategories = await productSelectedCategoriesRepository.GetEntitiesQuery()
                .Where(s => s.ProductId == productId)
                .ToListAsync();

            productSelectedCategoriesRepository.DeleteEntities(existingCategories);

            var newCategories = categoryIds.Select(c => new ProductSelectedCategories
            {
                ProductId = productId,
                ProductCategoriesId = c,
                CreateDate = DateTime.Now,
                IsDelete = false
            }).ToList();

            await productSelectedCategoriesRepository.AddRange(newCategories);
            await productSelectedCategoriesRepository.SaveChanges();
        }

        public async Task<List<ProductCategories>> GetProductCategories(long productId)
        {
            return await productSelectedCategoriesRepository.GetEntitiesQuery()
                .Where(s => s.ProductId == productId)
                .Include(s => s.ProductCategories)
                .Select(s => s.ProductCategories)
                .ToListAsync();
        }

        public async Task RemoveCategoryFromProduct(long productId, long categoryId)
        {
            var entity = await productSelectedCategoriesRepository.GetEntitiesQuery()
                .FirstOrDefaultAsync(s => s.ProductId == productId && s.ProductCategoriesId == categoryId);

            if (entity == null) return;

            productSelectedCategoriesRepository.RemoveEntity(entity);
            await productSelectedCategoriesRepository.SaveChanges();
        }

        public async Task<bool> IsProductInCategory(long productId, long categoryId)
        {
            return await productSelectedCategoriesRepository.GetEntitiesQuery()
                .AnyAsync(s => s.ProductId == productId && s.ProductCategoriesId == categoryId);
        }

        #endregion

        #region dispose
        public void Dispose()
        {
            productCategoriesRepository?.Dispose();
            productGalleriesRepository?.Dispose();
            productVisitrepository?.Dispose();
            productRepository?.Dispose();
            productSelectedCategoriesRepository?.Dispose();
        }
        #endregion
    }
}
