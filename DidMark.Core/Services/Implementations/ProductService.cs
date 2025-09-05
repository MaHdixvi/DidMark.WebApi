using DidMark.Core.DTO.Paging;
using DidMark.Core.DTO.Products;
using DidMark.Core.Services.Interfaces;
using DidMark.Core.Utilities.Extensions.FileExtentions;
using DidMark.Core.Utilities.Extensions.Paging;
using DidMark.DataLayer.Entities.Product;
using DidMark.DataLayer.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static DidMark.Core.DTO.Products.FilterProductsDTO;

namespace DidMark.Core.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IGenericRepository<Product> productRepository;
        private readonly IGenericRepository<ProductCategories> productCategoriesRepository;
        private readonly IGenericRepository<ProductGalleries> productGalleriesRepository;
        private readonly IGenericRepository<ProductVisit> productVisitrepository;
        private readonly IGenericRepository<ProductSelectedCategories> productSelectedCategoriesRepository;
        private readonly string productImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "products");

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

            if (!Directory.Exists(productImagePath))
                Directory.CreateDirectory(productImagePath);
        }

        #region Product Management

        public async Task AddProduct(AddProductDTO productDto)
        {
            var imageName = string.Empty;
            if (productDto.ImageName != null)
                imageName = await SaveImage(productDto.ImageName);

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
                ImageName = imageName,
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

            if (!string.IsNullOrEmpty(productDto.ProductName)) product.ProductName = productDto.ProductName;
            if (!string.IsNullOrEmpty(productDto.Description)) product.Description = productDto.Description;
            if (!string.IsNullOrEmpty(productDto.ShortDescription)) product.ShortDescription = productDto.ShortDescription;
            if (!string.IsNullOrEmpty(productDto.Color)) product.Color = productDto.Color;
            if (!string.IsNullOrEmpty(productDto.Size)) product.Size = productDto.Size;
            if (productDto.Code.HasValue) product.Code = productDto.Code.Value;
            if (productDto.NumberofProduct.HasValue) product.NumberofProduct = productDto.NumberofProduct.Value;
            if (productDto.Price.HasValue) product.Price = productDto.Price.Value;

            if (productDto.IsExists.HasValue)
            {
                product.IsExists = productDto.IsExists.Value;
            }
            if (productDto.IsSpecial.HasValue)
            {
                product.IsSpecial = productDto.IsSpecial.Value;
            }


            if (productDto.Image != null)
            {
                DeleteImage(product.ImageName);
                product.ImageName = await SaveImage(productDto.Image);
            }

            productRepository.UpdateEntity(product);
            await productRepository.SaveChanges();
        }

        public async Task<bool> DeleteProduct(long productId)
        {
            var product = await productRepository.GetEntityById(productId);
            if (product == null) return false;

            DeleteImage(product.ImageName);

            product.IsDelete = true;
            productRepository.UpdateEntity(product);
            await productRepository.SaveChanges();

            return true;
        }

        public async Task<Product> GetProductById(long productId)
        {
            return await productRepository.GetEntitiesQuery()
                .SingleOrDefaultAsync(p => p.Id == productId && !p.IsDelete);
        }

        public async Task<List<Product>> GetRelatedProducts(long productId)
        {
            var product = await GetProductById(productId);
            if (product == null) return null;

            var categoryIds = await productSelectedCategoriesRepository.GetEntitiesQuery()
                .Where(s => s.ProductId == productId)
                .Select(s => s.ProductCategoriesId)
                .ToListAsync();

            return await productRepository.GetEntitiesQuery()
                .SelectMany(s => s.ProductSelectedCategories
                    .Where(f => categoryIds.Contains(f.ProductCategoriesId))
                    .Select(t => t.Product))
                .Where(s => s.Id != productId)
                .OrderByDescending(s => s.CreateDate)
                .Take(4)
                .ToListAsync();
        }

        public async Task<EditProductDTO> GetProductForEdit(long productId)
        {
            var product = await GetProductById(productId);
            if (product == null) return null;

            return new EditProductDTO
            {
                Id = product.Id,
                CurrentImage = product.ImageName,
                ProductName = product.ProductName,
                Description = product.Description,
                ShortDescription = product.ShortDescription,
                Color = product.Color,
                Size = product.Size,
                Code = product.Code,
                NumberofProduct = product.NumberofProduct,
                Price = product.Price,
                IsExists = product.IsExists,
                IsSpecial = product.IsSpecial
            };
        }
        public async Task<Product> GetProductByUserOrder(long productId)
        {
            return await productRepository.GetEntitiesQuery()
                .SingleOrDefaultAsync(p => p.Id == productId && !p.IsDelete);
        }

        public async Task<bool> IsExistsProductById(long productId)
        {
            return await productRepository.GetEntitiesQuery()
                .AnyAsync(p => p.Id == productId && !p.IsExists && !p.IsDelete);
        }

        public async Task<FilterProductsDTO> FilterProducts(FilterProductsDTO filter)
        {
            var productsQuery = productRepository.GetEntitiesQuery().AsQueryable();

            // فیلتر و مرتب‌سازی طبق filter
            if (!string.IsNullOrEmpty(filter.Title))
                productsQuery = productsQuery.Where(p => p.ProductName.Contains(filter.Title));

            if (filter.StartPrice > 0)
                productsQuery = productsQuery.Where(p => p.Price >= filter.StartPrice);

            if (filter.EndPrice > 0)
                productsQuery = productsQuery.Where(p => p.Price <= filter.EndPrice);

            // مرتب‌سازی
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

            var count = (int)Math.Ceiling(productsQuery.Count() / (double)filter.TakeEntity);
            var pager = Pager.Build(count, filter.PageId, filter.TakeEntity);

            var products = await productsQuery.Paging(pager).ToListAsync();

            return filter.SetProducts(products).SetPaging(pager);
        }


        #endregion

        #region Product Categories

        public async Task<List<ProductCategories>> GetAllActiveProductCategories()
        {
            return await productCategoriesRepository.GetEntitiesQuery()
                .Where(s => !s.IsDelete)
                .ToListAsync();
        }

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

        #region Product Gallery

        public async Task<List<ProductGalleries>> GetProductActiveGalleries(long productId)
        {
            return await productGalleriesRepository.GetEntitiesQuery()
                .Where(s => s.ProductId == productId && !s.IsDelete)
                .ToListAsync();
        }

        #endregion

        #region Image Helpers

        private async Task<string> SaveImage(IFormFile image)
        {
            if (image == null) return string.Empty;

            var fileName = $"{Guid.NewGuid()}_{image.FileName}";
            var filePath = Path.Combine(productImagePath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            return $"/uploads/products/{fileName}";
        }

        private void DeleteImage(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath)) return;

            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath.TrimStart('/'));

            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }

        #endregion

        #region Dispose
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
