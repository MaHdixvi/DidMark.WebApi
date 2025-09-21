using DidMark.Core.DTO.Slider;
using DidMark.Core.Services.Interfaces;
using DidMark.DataLayer.Entities.Site;
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
    public class SliderService : ISliderService
    {
        private readonly IGenericRepository<Slider> _sliderRepository;
        private readonly IWebHostEnvironment _env;

        public SliderService(IGenericRepository<Slider> sliderRepository, IWebHostEnvironment env)
        {
            _sliderRepository = sliderRepository;
            _env = env;
        }
        public async Task<List<SliderDTO>> GetAllSliders()
        {
            return await _sliderRepository.GetEntitiesQuery()
                .OrderBy(s => s.DisplayOrder)
                .Select(s => new SliderDTO
                {
                    Id = s.Id,
                    Title = s.ProductName,                  // ProductName → Title
                    Description = s.Description,
                    ImageUrl = s.ImageUrl,                     // Image → ImageUrl
                    Link = s.Link,                          // اگر فیلد Link در Entity هست
                    DisplayOrder = s.DisplayOrder,          // اضافه کردن DisplayOrder
                    IsActive = !s.IsDelete,
                    CreatedDate = s.CreateDate,
                    UpdatedDate = s.LastUpdateDate,
                    ProductName = s.ProductName
                })
                .ToListAsync();
        }

        public async Task<List<SliderDTO>> GetActiveSliders()
        {
            return await _sliderRepository.GetEntitiesQuery()
                .Where(s => !s.IsDelete)
                .OrderBy(s => s.DisplayOrder)
                .Select(s => new SliderDTO
                {
                    Id = s.Id,
                    Title = s.ProductName,
                    Description = s.Description,
                    ImageUrl = s.ImageUrl,
                    Link = s.Link,
                    DisplayOrder = s.DisplayOrder,
                    IsActive = true,
                    CreatedDate = s.CreateDate,
                    UpdatedDate = s.LastUpdateDate,
                    ProductName = s.ProductName
                })
                .ToListAsync();
        }

        public async Task<(bool Success, long Id)> AddSlider(AddSliderDTO dto)
        {
            var slider = new Slider
            {
                ProductName = dto.Title,                  // Title
                Description = dto.Description,
                ImageUrl = await SaveImage(dto.ImageUrl, "sliders"),
                Link = dto.Link,
                DisplayOrder = dto.DisplayOrder,
                IsDelete = !dto.IsActive,
                CreateDate = DateTime.UtcNow
            };

            await _sliderRepository.AddEntity(slider);
            await _sliderRepository.SaveChanges();

            return (true, slider.Id);
        }

        public async Task<(bool Success, long Id)> UpdateSlider(EditSliderDTO dto)
        {
            var slider = await _sliderRepository.GetEntityById(dto.Id);
            if (slider == null) return (false, 0);

            if (!string.IsNullOrEmpty(dto.Title)) slider.ProductName = dto.Title;
            if (!string.IsNullOrEmpty(dto.Description)) slider.Description = dto.Description;
            if (!string.IsNullOrEmpty(dto.Link)) slider.Link = dto.Link;
            if (!string.IsNullOrEmpty(dto.ProductName)) slider.ProductName = dto.ProductName;
            if (dto.DisplayOrder.HasValue) slider.DisplayOrder = dto.DisplayOrder.Value;

            if (dto.ImageUrl != null)
            {
                DeleteImage(slider.ImageUrl);
                slider.ImageUrl = await SaveImage(dto.ImageUrl, "sliders");
            }

            if (dto.IsActive.HasValue)
                slider.IsDelete = !dto.IsActive.Value;

            slider.LastUpdateDate = DateTime.UtcNow;

            _sliderRepository.UpdateEntity(slider);
            await _sliderRepository.SaveChanges();

            return (true, slider.Id);
        }


        public async Task<SliderDTO?> GetSliderById(long sliderId)
        {
            var s = await _sliderRepository.GetEntityById(sliderId);
            if (s == null) return null;

            return new SliderDTO
            {
                Id = s.Id,
                ProductName = s.ProductName,
                Description = s.Description,
                IsActive = !s.IsDelete,
                Title = s.Title,
                ImageUrl = s.ImageUrl,
                Link = s.Link,
                DisplayOrder = s.DisplayOrder,
                CreatedDate = s.CreateDate,
                UpdatedDate = s.LastUpdateDate,
            };
        }

        public async Task<bool> DeleteSlider(long sliderId)
        {
            var slider = await _sliderRepository.GetEntityById(sliderId);
            if (slider == null) return false;

            DeleteImage(slider.ImageUrl);

            slider.IsDelete = true;
            _sliderRepository.UpdateEntity(slider);
            await _sliderRepository.SaveChanges();

            return true;
        }
        public void Dispose()
        {
            _sliderRepository?.Dispose();
        }
        private async Task<string> SaveImage(IFormFile image, string subFolder)
        {
            if (image == null) return string.Empty;

            var folderPath = Path.Combine(_env.WebRootPath, "uploads", subFolder);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid()}_{image.FileName}";
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            return $"/uploads/{subFolder}/{fileName}";
        }
        private void DeleteImage(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath)) return;

            // مسیر نسبی مثل: /uploads/sliders/xxx.jpg
            var fullPath = Path.Combine(_env.WebRootPath, imagePath.TrimStart('/'));

            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }

    }
}
