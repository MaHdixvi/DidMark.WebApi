using DidMark.Core.DTO.Paging;
using DidMark.Core.DTO.Products;
using DidMark.Core.DTO.Products.ProductCategory;
using DidMark.Core.DTO.Products.ProductGalleries;
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
        private readonly IGenericRepository<ProductAttribute> productAttributeRepository;
        private readonly IGenericRepository<ProductCategories> productCategoriesRepository;
        private readonly IGenericRepository<ProductGalleries> productGalleriesRepository;
        private readonly IGenericRepository<ProductVisit> productVisitrepository;
        private readonly IGenericRepository<ProductSelectedCategories> productSelectedCategoriesRepository;
        private readonly string productImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "products");

        public ProductService(
            IGenericRepository<Product> productRepository,
            IGenericRepository<ProductAttribute> productAttributeRepository,
            IGenericRepository<ProductCategories> productCategoriesRepository,
            IGenericRepository<ProductGalleries> productGalleriesRepository,
            IGenericRepository<ProductVisit> productVisitrepository,
            IGenericRepository<ProductSelectedCategories> productSelectedCategoriesRepository)
        {
            this.productRepository = productRepository;
            this.productAttributeRepository = productAttributeRepository;
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
                //Color = productDto.Color,
                //Size = productDto.Size,
                //Code = productDto.Code,
                NumberofProduct = productDto.NumberofProduct,
                Price = productDto.Price,
                ShortDescription = productDto.ShortDescription,
                IsExists = productDto.IsExists,
                IsSpecial = productDto.IsSpecial,
                ImageName = imageName,
                CreateDate = DateTime.Now,
                IsDelete = false,
                IsActive = productDto.IsActive,
                // 🔹 تخفیف
                DiscountPercent = productDto.DiscountPercent,
                DiscountStartDate = productDto.DiscountStartDate,
                DiscountEndDate = productDto.DiscountEndDate
            };

            await productRepository.AddEntity(product);
            await productRepository.SaveChanges();

            if (productDto.Attributes != null && productDto.Attributes.Any())
            {
                var productAttributes = productDto.Attributes.Select(a => new ProductAttribute
                {
                    ProductId = product.Id,
                    PAttributeId = a.PAttributeId,
                    Value = a.Value
                }).ToList();

                await productAttributeRepository.AddRange(productAttributes);
                await productAttributeRepository.SaveChanges();
            }
        }

        public async Task UpdateProduct(EditProductDTO productDto)
        {
            var product = await productRepository.GetEntityById(productDto.Id);
            if (product == null) return;

            if (!string.IsNullOrEmpty(productDto.ProductName)) product.ProductName = productDto.ProductName;
            if (!string.IsNullOrEmpty(productDto.Description)) product.Description = productDto.Description;
            if (!string.IsNullOrEmpty(productDto.ShortDescription)) product.ShortDescription = productDto.ShortDescription;
            //if (!string.IsNullOrEmpty(productDto.Color)) product.Color = productDto.Color;
            //if (!string.IsNullOrEmpty(productDto.Size)) product.Size = productDto.Size;
            //if (productDto.Code.HasValue) product.Code = productDto.Code.Value;
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
            if (productDto.IsActive.HasValue)
            {
                product.IsActive = productDto.IsActive.Value;
            }
            if (productDto.DiscountPercent.HasValue)
                product.DiscountPercent = productDto.DiscountPercent.Value;

            if (productDto.DiscountStartDate.HasValue)
                product.DiscountStartDate = productDto.DiscountStartDate.Value;

            if (productDto.DiscountEndDate.HasValue)
                product.DiscountEndDate = productDto.DiscountEndDate.Value;


            if (productDto.Image != null)
            {
                DeleteImage(product.ImageName);
                product.ImageName = await SaveImage(productDto.Image);
            }

            productRepository.UpdateEntity(product);
            await productRepository.SaveChanges();

            if (productDto.Attributes != null && productDto.Attributes.Any())
            {
                // 🔹 حذف اتریبیوت‌های قدیمی و ثبت جدیدها
                var oldAttributes = await productAttributeRepository.GetEntitiesQuery()
                    .Where(a => a.ProductId == product.Id)
                    .ToListAsync();

                productAttributeRepository.DeleteEntities(oldAttributes);
                await productAttributeRepository.SaveChanges();


                var newAttributes = productDto.Attributes.Select(a => new ProductAttribute
                {
                    ProductId = product.Id,
                    PAttributeId = a.PAttributeId,
                    Value = a.Value
                }).ToList();

                await productAttributeRepository.AddRange(newAttributes);
                await productAttributeRepository.SaveChanges();
            }
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

        public async Task<ProductDto> GetProductById(long productId)
        {
            // دریافت محصول بدون Includeهای مشکل‌ساز
            var product = await productRepository.GetEntitiesQuery()
                .Include(p => p.ProductAttributes)
                    .ThenInclude(pa => pa.PAttribute)
                .Include(p => p.ProductSelectedCategories)
                    .ThenInclude(pc => pc.ProductCategories)
                // حذف Include مربوط به ProductGalleries - این باعث circular reference می‌شود
                // .Include(p => p.ProductGalleries)
                .SingleOrDefaultAsync(p => p.Id == productId);

            if (product == null) return null;

            // دریافت گالری‌ها به صورت جداگانه
            var galleries = await productGalleriesRepository.GetEntitiesQuery()
                .Where(g => g.ProductId == productId && !g.IsDelete)
                .Select(g => new ProductGalleryDTO
                {
                    ImageName = g.ImageName
                })
                .ToListAsync();

            // تبدیل به ProductDto
            return new ProductDto
            {
                Id = product.Id,
                ProductName = product.ProductName,
                Description = product.Description,
                ShortDescription = product.ShortDescription,
                Price = product.Price,
                NumberofProduct = product.NumberofProduct,
                IsExists = product.IsExists,
                IsSpecial = product.IsSpecial,
                IsActive = product.IsActive,
                ImageName = product.ImageName,
                FinalPrice = product.FinalPrice,
                DiscountPercent = product.DiscountPercent,
                DiscountStartDate = product.DiscountStartDate,
                DiscountEndDate = product.DiscountEndDate,
                Attributes = product.ProductAttributes?.Select(pa => new ProductAttributeDto
                {
                    PAttributeId = pa.PAttributeId,
                    Value = pa.Value,
                    Name = pa.PAttribute?.Name
                }).ToList(),
                Categories = product.ProductSelectedCategories?.Select(pc => new ProductCategoryDTO
                {
                    Id = pc.ProductCategoriesId,
                    Title = pc.ProductCategories?.Title,
                    ParentId = pc.ProductCategories?.ParentId,
                    UrlTitle = pc.ProductCategories?.UrlTitle
                }).ToList(),
                Galleries = galleries // استفاده از گالری‌های جداگانه
            };
        }


        public async Task<List<ProductDto>> GetRelatedProducts(long productId)
        {
            // دریافت محصول اصلی
            var product = await productRepository.GetEntitiesQuery()
                .Include(p => p.ProductSelectedCategories)
                .SingleOrDefaultAsync(p => p.Id == productId && !p.IsDelete && p.IsActive);

            if (product == null) return null;

            var categoryIds = product.ProductSelectedCategories
                .Select(s => s.ProductCategoriesId)
                .ToList();

            // دریافت محصولات مرتبط با همان دسته‌بندی‌ها
            var relatedProducts = await productRepository.GetEntitiesQuery()
                .Include(p => p.ProductAttributes)
                .ThenInclude(pa => pa.PAttribute)
                .Include(p => p.ProductSelectedCategories)
                    .ThenInclude(pc => pc.ProductCategories)
                .Include(p => p.ProductGalleries)
                .Where(p => p.Id != productId && p.ProductSelectedCategories.Any(pc => categoryIds.Contains(pc.ProductCategoriesId)) && !p.IsDelete)
                .OrderByDescending(p => p.CreateDate)
                .Take(4)
                .ToListAsync();

            // تبدیل به ProductDto
            return relatedProducts.Select(p => new ProductDto
            {
                Id = p.Id,
                ProductName = p.ProductName,
                Description = p.Description,
                ShortDescription = p.ShortDescription,
                Price = p.Price,
                NumberofProduct = p.NumberofProduct,
                IsExists = p.IsExists,
                IsSpecial = p.IsSpecial,
                IsActive = p.IsActive,
                ImageName = p.ImageName,
                FinalPrice = product.FinalPrice,
                DiscountPercent = product.DiscountPercent,
                DiscountStartDate = product.DiscountStartDate,
                DiscountEndDate = product.DiscountEndDate,
                Attributes = p.ProductAttributes?.Select(pa => new ProductAttributeDto
                {
                    PAttributeId = pa.PAttributeId,
                    Value = pa.Value,
                    Name = pa.PAttribute?.Name
                }).ToList(),
                Categories = p.ProductSelectedCategories?.Select(pc => new ProductCategoryDTO
                {
                    Id = pc.ProductCategoriesId,
                    Title = pc.ProductCategories?.Title,
                    ParentId = pc.ProductCategories?.ParentId,
                    UrlTitle = pc.ProductCategories?.UrlTitle
                }).ToList(),
                Galleries = product.ProductGalleries?.Where(g => !g.IsDelete)
                            .Select(g => new ProductGalleryDTO
                            {
                                ImageName = g.ImageName
                            }).ToList()
            }).ToList();
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
                //Color = product.Color,
                //Size = product.Size,
                //Code = product.Code,
                NumberofProduct = product.NumberofProduct,
                Price = product.Price,
                IsExists = product.IsExists,
                IsSpecial = product.IsSpecial,
                IsActive = product.IsActive,
                DiscountPercent = product.DiscountPercent,
                DiscountStartDate = product.DiscountStartDate,
                DiscountEndDate = product.DiscountEndDate,
                Attributes = product.Attributes.Select(pa => new EditProductAttributeDto
                {
                    Value = pa.Value,
                    PAttributeId = pa.PAttributeId,
                    ProductId = product.Id
                }).ToList()
            };
        }
        public async Task<ProductDto> GetProductByUserOrder(long productId)
        {
            // دریافت محصول همراه با اتریبیوت‌ها، دسته‌بندی‌ها و گالری‌ها
            var product = await productRepository.GetEntitiesQuery()
                .Include(p => p.ProductAttributes)
                .ThenInclude(pa => pa.PAttribute)
                .Include(p => p.ProductSelectedCategories)
                    .ThenInclude(pc => pc.ProductCategories)
                .Include(p => p.ProductGalleries)
                .SingleOrDefaultAsync(p => p.Id == productId && !p.IsDelete);

            if (product == null) return null;

            // تبدیل به ProductDto
            return new ProductDto
            {
                Id = product.Id,
                ProductName = product.ProductName,
                Description = product.Description,
                ShortDescription = product.ShortDescription,
                Price = product.Price,
                NumberofProduct = product.NumberofProduct,
                IsExists = product.IsExists,
                IsSpecial = product.IsSpecial,
                ImageName = product.ImageName,
                IsActive = product.IsActive,
                FinalPrice = product.FinalPrice,
                DiscountPercent = product.DiscountPercent,
                DiscountStartDate = product.DiscountStartDate,
                DiscountEndDate = product.DiscountEndDate,
                Attributes = product.ProductAttributes?.Select(pa => new ProductAttributeDto
                {
                    PAttributeId = pa.PAttributeId,
                    Value = pa.Value,
                    Name = pa.PAttribute?.Name
                }).ToList(),
                Categories = product.ProductSelectedCategories?.Select(pc => new ProductCategoryDTO
                {
                    Id = pc.ProductCategoriesId,
                    Title = pc.ProductCategories?.Title,
                    ParentId = pc.ProductCategories?.ParentId,
                    UrlTitle = pc.ProductCategories?.UrlTitle
                }).ToList(),
                Galleries = product.ProductGalleries?.Where(g => !g.IsDelete)
                            .Select(g => new ProductGalleryDTO
                            {
                                ImageName = g.ImageName
                            }).ToList()
            };
        }


        public async Task<bool> IsExistsProductById(long productId)
        {
            return await productRepository.GetEntitiesQuery()
                .AnyAsync(p => p.Id == productId && !p.IsExists && !p.IsDelete);
        }

        public async Task<FilterProductsDTO> FilterProducts(FilterProductsDTO filter)
        {
            var productsQuery = productRepository.GetEntitiesQuery()
                .Include(p => p.ProductAttributes)
                .ThenInclude(pa => pa.PAttribute)
                .Include(p => p.ProductSelectedCategories) // مطمئن شو categories هم لود میشن
                .Where(p => !p.IsDelete)
                .AsQueryable();

            // فیلتر بر اساس عنوان
            if (!string.IsNullOrEmpty(filter.Title))
                productsQuery = productsQuery.Where(p => p.ProductName.Contains(filter.Title));

            // فیلتر بر اساس قیمت
            if (filter.StartPrice > 0)
                productsQuery = productsQuery.Where(p => p.Price >= filter.StartPrice);

            if (filter.EndPrice > 0)
                productsQuery = productsQuery.Where(p => p.Price <= filter.EndPrice);

            // فیلتر بر اساس دسته‌بندی‌ها
            if (filter.Categories != null && filter.Categories.Any())
            {
                // گرفتن همه شناسه‌های فرزندان برای هر دسته‌بندی انتخاب شده
                var allCategoryIds = new List<long>();
                foreach (var catId in filter.Categories)
                {
                    allCategoryIds.Add(catId);
                    allCategoryIds.AddRange(await GetAllChildCategoryIds(catId));
                }

                productsQuery = productsQuery.Where(p =>
                    p.ProductSelectedCategories.Any(c => allCategoryIds.Contains(c.ProductCategoriesId))
                );
            }
            // 🔹 فیلتر بر اساس تخفیف
            if (filter.OnlyDiscounted)
            {
                productsQuery = productsQuery.Where(p => p.DiscountPercent > 0
                    && (!p.DiscountStartDate.HasValue || p.DiscountStartDate <= DateTime.Now)
                    && (!p.DiscountEndDate.HasValue || p.DiscountEndDate >= DateTime.Now));
            }

            // مرتب‌سازی
            if (filter.OrderBy != null)
            {
                switch (filter.OrderBy)
                {
                    case ProductOrderBy.PriceAsc:
                        productsQuery = productsQuery.OrderBy(x => x.Price);
                        break;
                    case ProductOrderBy.PriceDes:
                        productsQuery = productsQuery.OrderByDescending(x => x.FinalPrice);
                        break;
                    case ProductOrderBy.CreateDateAsc:
                        productsQuery = productsQuery.OrderBy(x => x.CreateDate);
                        break;
                    case ProductOrderBy.CreateDateDes:
                        productsQuery = productsQuery.OrderByDescending(x => x.CreateDate);
                        break;
                    case ProductOrderBy.IsSpecial:
                        productsQuery = productsQuery.OrderByDescending(x => x.IsSpecial);
                        break;
                    case ProductOrderBy.MaxDiscount: // 🔹 اضافه شد
                        productsQuery = productsQuery.OrderByDescending(x => x.DiscountPercent);
                        break;
                }
            }

            var count = (int)Math.Ceiling(productsQuery.Count() / (double)filter.TakeEntity);
            var pager = Pager.Build(count, filter.PageId, filter.TakeEntity);

            var products = await productsQuery.Paging(pager).ToListAsync();

            return filter.SetProducts(products).SetPaging(pager);
        }

        // تابع کمکی برای گرفتن همه فرزندان (بازگشتی)
        private async Task<List<long>> GetAllChildCategoryIds(long parentId)
        {
            var children = await productCategoriesRepository.GetEntitiesQuery()
                .Where(c => c.ParentId == parentId)
                .Select(c => c.Id)
                .ToListAsync();

            var allIds = new List<long>();

            foreach (var childId in children)
            {
                allIds.Add(childId);
                allIds.AddRange(await GetAllChildCategoryIds(childId));
            }

            return allIds;
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
        public async Task AddProductVisit(long productId, string userIp)
        {
            var visit = new ProductVisit
            {
                ProductId = productId,
                UserIp = userIp, // اگر مدل شما int است
                CreateDate = DateTime.Now,
                IsDelete = false
            };

            await productVisitrepository.AddEntity(visit);
            await productVisitrepository.SaveChanges();
        }
        public async Task<List<ProductVisitDto>> GetProductVisits(long productId)
        {
            var visits = await productVisitrepository.GetEntitiesQuery()
                .Where(v => v.ProductId == productId && !v.IsDelete)
                .ToListAsync();

            return visits.Select(v => new ProductVisitDto
            {
                Id = v.Id,
                ProductId = v.ProductId,
                UserIp = v.UserIp.ToString(), // اگر int هست
                CreateDate = v.CreateDate
            }).ToList();
        }


    }
}
