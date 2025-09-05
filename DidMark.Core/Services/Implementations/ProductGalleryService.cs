using DidMark.Core.DTO.Products;
using DidMark.Core.DTO.Products.NewFolder;
using DidMark.Core.DTO.Products.ProductGalleries;
using DidMark.Core.Services.Interfaces;
using DidMark.DataLayer.Entities.Product;
using DidMark.DataLayer.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DidMark.Core.Services.Implementations
{
    public class ProductGalleryService : IProductGalleryService
    {
        private readonly IGenericRepository<ProductGalleries> _galleryRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductGalleryService(IGenericRepository<ProductGalleries> galleryRepository, IWebHostEnvironment webHostEnvironment)
        {
            _galleryRepository = galleryRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        // افزودن گالری محصول
        public async Task<bool> AddProductGallery(AddProductGalleryDTO dto)
        {
            if (dto == null || dto.Image == null)
                return false;

            var imageUrl = await SaveImageToWwwroot(dto.Image, dto.ProductId);

            var gallery = new ProductGalleries
            {
                ProductId = dto.ProductId,
                ImageName = imageUrl, // مسیر نسبی ذخیره شده
                CreateDate = DateTime.Now,
                IsDelete = false
            };

            await _galleryRepository.AddEntity(gallery);
            await _galleryRepository.SaveChanges();
            return true;
        }

        // ویرایش گالری محصول (در صورت نیاز فعال شود)
        //public async Task<bool> EditProductGallery(EditProductGalleryDTO dto, long galleryId, long productId)
        //{
        //    var existingGallery = await _galleryRepository.GetEntitiesQuery()
        //        .FirstOrDefaultAsync(g => g.Id == galleryId && g.ProductId == productId);
        //    if (existingGallery == null)
        //        return false;
        //
        //    if (dto.Image != null)
        //    {
        //        DeleteImageFromWwwroot(existingGallery.ImageName);
        //        existingGallery.ImageName = await SaveImageToWwwroot(dto.Image, productId);
        //    }
        //
        //    existingGallery.LastUpdateDate = DateTime.Now;
        //    _galleryRepository.UpdateEntity(existingGallery);
        //    await _galleryRepository.SaveChanges();
        //    return true;
        //}

        // حذف گالری محصول
        public async Task<bool> DeleteProductGallery(long galleryId, long productId)
        {
            var gallery = await _galleryRepository.GetEntitiesQuery()
                .FirstOrDefaultAsync(g => g.Id == galleryId && g.ProductId == productId);

            if (gallery == null)
                return false;

            DeleteImageFromWwwroot(gallery.ImageName);
            _galleryRepository.RemoveEntity(gallery);
            await _galleryRepository.SaveChanges();
            return true;
        }

        // دریافت گالری‌های محصول
        public async Task<ICollection<ProductGalleryDTO>> GetProductGalleriesByProductId(long productId)
        {
            var galleries = await _galleryRepository.GetEntitiesQuery()
                .Where(g => g.ProductId == productId&&!g.IsDelete)
                .ToListAsync();

            return galleries.Select(g => new ProductGalleryDTO
            {
                Id = g.Id,
                ProductId = g.ProductId,
                ImageName = g.ImageName,
            }).ToList();
        }

        // ذخیره تصویر در wwwroot/uploads/product-galleries/{ProductId}
        private async Task<string> SaveImageToWwwroot(IFormFile image, long productId)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "product-galleries", productId.ToString());
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = $"{Guid.NewGuid()}_{image.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            // برگشت مسیر نسبی برای نمایش در وب
            return $"/uploads/product-galleries/{productId}/{uniqueFileName}";
        }

        // حذف تصویر از wwwroot
        private void DeleteImageFromWwwroot(string imageUrl)
        {
            if (!string.IsNullOrEmpty(imageUrl))
            {
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }
    }
}
