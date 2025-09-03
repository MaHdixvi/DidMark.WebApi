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
using System.Text;
using System.Threading.Tasks;
using static DidMark.Core.DTO.Products.FilterProductsDTO;

namespace DidMark.Core.Services.Implementations
{
    public class ProductService : IProductService
    {
        #region constructor
        private IGenericRepository<Product> productRepository;
        private IGenericRepository<ProductCategories> productCategoriesRepository;
        private IGenericRepository<ProductGalleries> productGalleriesRepository;
        private IGenericRepository<ProductVisit> productVisitrepository;
        private IGenericRepository<ProductSelectedCategories> ProductSelectedCategoriesrepository;


        #endregion

        #region product categories
        public async Task<List<ProductCategories>> GetAllActiveProductCategories()
        {
            return await productCategoriesRepository.GetEntitiesQuery().Where(s => !s.IsDelete).ToListAsync();
        }

        #endregion

        #region ProductGallery
        public async Task<List<ProductGalleries>> GetProductActiveGalleries(long productId)
        {
            return await productGalleriesRepository
                .GetEntitiesQuery()
                .Where(s => s.ProductId == productId && s.IsDelete)
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

        #region dispose
        public void Dispose()
        {
            productCategoriesRepository?.Dispose();
            productGalleriesRepository?.Dispose();
            productVisitrepository?.Dispose();
            productRepository?.Dispose();
            ProductSelectedCategoriesrepository?.Dispose();

        }
        #endregion
        #region product 
        public async Task AddProduct(Product product)
        {
            await productRepository.AddEntity(product);
            await productRepository.SaveChanges();
        }
        public async Task UpdateProduct(Product product)
        {
            productRepository.UpdateEntity(product);
            await productRepository.SaveChanges();
        }
        public async Task<FilterProductsDTO> FilterProducts(FilterProductsDTO filter)
        {
            var productsQuery = productRepository.GetEntitiesQuery().AsQueryable();
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
            if (!string.IsNullOrEmpty(filter.Title))
            {
                productsQuery = productsQuery.Where(s => s.ProductName.Contains(filter.Title));
                productsQuery = productsQuery.Where(s => s.Price >= filter.StartPrice);
                if (filter.StartPrice != 0)
                {
                    productsQuery = productsQuery.Where(s => s.Price >= filter.EndPrice);
                }
                if (filter.EndPrice != 0)
                {
                    productsQuery = productsQuery.Where(s => s.Price <= filter.EndPrice);
                }
                if (filter.Categories != null && filter.Categories.Any()) ;
                {
                    productsQuery = productsQuery.SelectMany(s => s.ProductSelectedCategories.Where(f => filter.Categories.Contains(f.ProductCategoriesId)).Select(t => t.Product));
                }
            }

            var count = (int)Math.Ceiling(productsQuery.Count() / (double)filter.TakeEntity);
            var pager = Pager.Build(count, filter.PageId, filter.TakeEntity);
            var products = await productsQuery.Paging(pager).ToListAsync();
            return filter.SetProducts(products).SetPaging(pager);

        }
        public async Task<Product> GetProductById(long productId)
        {
            return await productRepository.GetEntitiesQuery().AsQueryable().SingleOrDefaultAsync(s => s.IsDelete && s.Id == productId);
        }

        public async Task<List<Product>> GetRelatedProducts(long productId)
        {
            var product = await productRepository.GetEntityById(productId);
            if (product == null)
            {
                return null;
            }
            var productCategoriesList = await ProductSelectedCategoriesrepository.GetEntitiesQuery().Where(s => s.ProductId == productId).Select(f => f.ProductCategoriesId).ToListAsync();
            var relatedProducts = await productRepository.GetEntitiesQuery().SelectMany(s => s.ProductSelectedCategories.Where(f => productCategoriesList.Contains(f.ProductCategoriesId)).Select(t => t.Product)).Where(s => s.Id != productId).OrderByDescending(s => s.CreateDate).Take(4).ToListAsync();
            return relatedProducts;
        }

        public async Task<bool> IsExistsProductById(long productId)
        {
            return await productRepository.GetEntitiesQuery().AnyAsync(s => s.Id == productId);
        }

        public async Task<Product> GetProductByUserOrder(long productId)
        {
            return await productRepository.GetEntitiesQuery().SingleOrDefaultAsync(p => p.Id == productId && !p.IsDelete);
        }

        public async Task<EditProductDTO> GetProductForEdit(long productId)
        {
            var product = await productRepository.GetEntitiesQuery().AsQueryable().SingleOrDefaultAsync(p => p.Id == productId);
            if (product == null) { return null; }
            return new EditProductDTO
            {
                Id = product.Id,
                CurrentImage = product.ImageName,
                Description = product.Description,
                IsExists = product.IsExists,
                IsSpecial = product.IsSpecial,
                Price = product.Price,
                ProductName = product.ProductName,
                ShortDescription = product.ShortDescription,





            };
        }

        public async Task EditProduct(EditProductDTO product)
        {
            var mainproduct = await productRepository.GetEntityById(product.Id);
            if (mainproduct == null)
            {
                mainproduct.ProductName = product.ProductName;
                mainproduct.Description = product.Description;
                mainproduct.IsExists = product.IsExists;
                mainproduct.IsSpecial = product.IsSpecial;
                mainproduct.Price = (int)product.Price;
                mainproduct.ShortDescription = product.ShortDescription;
                if (!string.IsNullOrEmpty(product.Base64Image))
                {
                    var imageFile = imageUploaderExtention.Base64ToImage(product.Base64Image);
                    var ImageName = Guid.NewGuid().ToString("N") + ".jpeg";
                    imageFile.AddImageToServer(ImageName, PathTools.ProductImagePath, mainproduct.ImageName);
                    mainproduct.ImageName = ImageName;
                }
                productRepository.UpdateEntity(mainproduct);
                await productRepository.SaveChanges();
            }
        }

        #endregion
    }

}
