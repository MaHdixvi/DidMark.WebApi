using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DidMark.Core.DTO.OffCodes;
using DidMark.Core.Services.Interfaces;
using DidMark.DataLayer.Entities.Orders;
using DidMark.DataLayer.Repository;
using Microsoft.EntityFrameworkCore;

namespace DidMark.Core.Services.Implementations
{
    public class OffCodeService : IOffCodeService, IDisposable
    {
        private readonly IGenericRepository<OffCode> _offCodeRepository;
        private readonly IGenericRepository<OffCodeProduct> _offCodeProductRepository;
        private readonly IGenericRepository<OffCodeCategory> _offCodeCategoryRepository;
        private readonly IGenericRepository<OffCodeUser> _offCodeUserRepository;

        public OffCodeService(
            IGenericRepository<OffCode> offCodeRepository,
            IGenericRepository<OffCodeProduct> offCodeProductRepository,
            IGenericRepository<OffCodeCategory> offCodeCategoryRepository,
            IGenericRepository<OffCodeUser> offCodeUserRepository
            )
        {
            _offCodeRepository = offCodeRepository ?? throw new ArgumentNullException(nameof(offCodeRepository));
            _offCodeProductRepository = offCodeProductRepository ?? throw new ArgumentNullException(nameof(offCodeProductRepository));
            _offCodeCategoryRepository = offCodeCategoryRepository ?? throw new ArgumentNullException(nameof(offCodeCategoryRepository));
            _offCodeUserRepository = offCodeUserRepository ?? throw new ArgumentNullException(nameof(offCodeUserRepository));
        }

        #region User Side

        public async Task<OffCodeResultDto?> GetActiveOffCodeByCodeAsync(
            string code,
            long? userId = null,
            long? productId = null,
            long? categoryId = null)
        {
            var query = _offCodeRepository.GetEntitiesQuery()
                .Include(c => c.OffCodeProducts)
                .Include(c => c.OffCodeCategories)
                .Include(c => c.UserOffCodes)
                .Where(c =>
                    c.Code == code &&
                    !c.IsDelete &&
                    c.ExpireDate >= DateTime.Now &&
                    (c.MaxUsageCount == null || c.UsedCount < c.MaxUsageCount));

            var offCode = await query.SingleOrDefaultAsync();
            if (offCode == null) return null;

            // 🔹 محدودیت برای UserId مستقیم (یک‌به‌چند)
            if (offCode.UserId.HasValue && offCode.UserId != userId)
                return null;

            // 🔹 محدودیت برای چندبه‌چند (OffCodeUser)
            if (offCode.UserOffCodes.Any() && userId.HasValue)
            {
                var userLink = offCode.UserOffCodes.FirstOrDefault(u => u.UserId == userId);
                if (userLink == null || (userLink.UsedCount >= offCode.MaxUsageCount && offCode.MaxUsageCount.HasValue))
                    return null;
            }

            // 🔹 محدودیت محصول
            if (offCode.OffCodeProducts.Any() &&
                (productId == null || !offCode.OffCodeProducts.Any(p => p.ProductId == productId)))
                return null;

            // 🔹 محدودیت کتگوری
            if (offCode.OffCodeCategories.Any() &&
                (categoryId == null || !offCode.OffCodeCategories.Any(c => c.CategoryId == categoryId)))
                return null;

            return new OffCodeResultDto
            {
                Code = offCode.Code,
                DiscountPercentage = offCode.DiscountPercentage,
                ExpireDate = offCode.ExpireDate
            };
        }

        public async Task<bool> IsCodeValidAsync(
            string code,
            long? userId = null,
            long? productId = null,
            long? categoryId = null)
        {
            var result = await GetActiveOffCodeByCodeAsync(code, userId, productId, categoryId);
            return result != null;
        }

        public async Task IncreaseUsageAsync(long offCodeId, long? userId = null)
        {
            var offCode = await _offCodeRepository.GetEntitiesQuery()
                .Include(c => c.UserOffCodes)
                .FirstOrDefaultAsync(c => c.Id == offCodeId);

            if (offCode == null) return;

            offCode.UsedCount++;

            // 🔹 افزایش تعداد استفاده برای کاربر خاص در حالت چند‌به‌چند
            if (userId.HasValue)
            {
                var userLink = offCode.UserOffCodes.FirstOrDefault(u => u.UserId == userId);
                if (userLink != null)
                    userLink.UsedCount++;
            }

            _offCodeRepository.UpdateEntity(offCode);
            await _offCodeRepository.SaveChanges();
        }

        #endregion

        #region Admin Side

        public async Task AddOffCodeAsync(CreateOffCodeDto dto)
        {
            var offCode = new OffCode
            {
                Code = dto.Code,
                DiscountPercentage = dto.DiscountPercentage,
                ExpireDate = dto.ExpireDate,
                MaxUsageCount = dto.MaxUsageCount,
                UserId = dto.UserId,
                UsedCount = 0,
                IsDelete = false
            };

            await _offCodeRepository.AddEntity(offCode);
            await _offCodeRepository.SaveChanges();
        }
        public async Task<List<OffCodeAdminDto>> GetAllOffCodesAsync()
        {
            return await _offCodeRepository.GetEntitiesQuery()
                .Include(c => c.UserOffCodes)
                .Include(c => c.OffCodeProducts)
                .Include(c => c.OffCodeCategories)
                .Select(c => new OffCodeAdminDto
                {
                    Id = c.Id,
                    Code = c.Code,
                    DiscountPercentage = c.DiscountPercentage,
                    ExpireDate = c.ExpireDate,
                    MaxUsageCount = c.MaxUsageCount,
                    UsedCount = c.UsedCount,
                    UserId = c.UserId,

                    // 👇 پر کردن DTO های مرتبط
                    Users = c.UserOffCodes.Select(u => new OffCodeUserDTO
                    {
                        Id = u.UserId,
                        Username = u.User.Username,
                        PhoneNumber = u.User.PhoneNumber
                    }).ToList(),

                    Products = c.OffCodeProducts.Select(p => new OffCodeProductDTO
                    {
                        Id = p.ProductId,
                        ProductName = p.Product.ProductName,
                        Price = p.Product.Price
                    }).ToList(),

                    Categories = c.OffCodeCategories.Select(cat => new OffCodeCategoryDTO
                    {
                        Id = cat.CategoryId,
                        Title = cat.Category.Title
                    }).ToList()
                })
                .ToListAsync();
        }
        public async Task EditOffCodeAsync(UpdateOffCodeDto dto)
        {
            var offCode = await _offCodeRepository.GetEntityById(dto.Id);
            if (offCode == null)
                throw new Exception("کد تخفیف یافت نشد");

            offCode.Code = dto.Code;
            offCode.DiscountPercentage = dto.DiscountPercentage;
            offCode.ExpireDate = dto.ExpireDate;
            offCode.MaxUsageCount = dto.MaxUsageCount;
            offCode.UserId = dto.UserId;

            _offCodeRepository.UpdateEntity(offCode);
            await _offCodeRepository.SaveChanges();
        }


        public async Task DeleteOffCodeAsync(long id)
        {
            var offCode = await _offCodeRepository.GetEntityById(id);
            if (offCode == null) return;

            offCode.IsDelete = true;
            _offCodeRepository.UpdateEntity(offCode);
            await _offCodeRepository.SaveChanges();
        }

        public async Task AddOffCodeProductAsync(AddOffCodeProductDto dto)
        {
            var offCode = await _offCodeRepository.GetEntitiesQuery()
                .Include(x => x.OffCodeProducts)
                .FirstOrDefaultAsync(x => x.Id == dto.OffCodeId);

            if (offCode == null)
                throw new Exception("کد تخفیف یافت نشد");

            if (offCode.OffCodeProducts.Any(x => x.ProductId == dto.ProductId))
                return;

            offCode.OffCodeProducts.Add(new OffCodeProduct
            {
                OffCodeId = dto.OffCodeId,
                ProductId = dto.ProductId
            });

            _offCodeRepository.UpdateEntity(offCode);
            await _offCodeRepository.SaveChanges();
        }

        public async Task EditOffCodeProductAsync(EditOffCodeProductDto dto)
        {
            var offCodeProduct = await _offCodeProductRepository.GetEntityById(dto.Id);
            if (offCodeProduct == null)
                throw new Exception("محصول یافت نشد");

            offCodeProduct.ProductId = dto.ProductId;

            _offCodeProductRepository.UpdateEntity(offCodeProduct);
            await _offCodeProductRepository.SaveChanges();
        }

        public async Task DeleteOffCodeProductAsync(long id)
        {
            var entity = await _offCodeProductRepository.GetEntityById(id);
            if (entity == null) return;

            _offCodeProductRepository.RemoveEntity(entity);
            await _offCodeProductRepository.SaveChanges();
        }


        public async Task AddOffCodeCategoryAsync(AddOffCodeCategoryDto dto)
        {
            var offCode = await _offCodeRepository.GetEntitiesQuery()
                .Include(x => x.OffCodeCategories)
                .FirstOrDefaultAsync(x => x.Id == dto.OffCodeId); 

            if (offCode == null)
                throw new Exception("کد تخفیف یافت نشد");

            if (offCode.OffCodeCategories.Any(x => x.CategoryId == dto.CategoryId))
                return;

            offCode.OffCodeCategories.Add(new OffCodeCategory
            {
                OffCodeId = dto.OffCodeId,
                CategoryId = dto.CategoryId
            });

            _offCodeRepository.UpdateEntity(offCode);
            await _offCodeRepository.SaveChanges();
        }

        public async Task EditOffCodeCategoryAsync(EditOffCodeCategoryDto dto)
        {
            var offCodeCategory = await _offCodeCategoryRepository.GetEntityById(dto.Id);
            if (offCodeCategory == null)
                throw new Exception("دسته‌بندی یافت نشد");

            offCodeCategory.CategoryId = dto.CategoryId;

            _offCodeCategoryRepository.UpdateEntity(offCodeCategory);
            await _offCodeCategoryRepository.SaveChanges();
        }

        public async Task DeleteOffCodeCategoryAsync(long id)
        {
            var entity = await _offCodeCategoryRepository.GetEntityById(id);
            if (entity == null) return;

            _offCodeCategoryRepository.RemoveEntity(entity);
            await _offCodeCategoryRepository.SaveChanges();
        }

        public async Task AddOffCodeUserAsync(AddOffCodeUserDto dto)
        {
            var offCode = await _offCodeRepository.GetEntitiesQuery()
                .Include(x => x.UserOffCodes)
                .FirstOrDefaultAsync(x => x.Id == dto.OffCodeId);

            if (offCode == null)
                throw new Exception("کد تخفیف یافت نشد");

            if (offCode.UserOffCodes.Any(u => u.UserId == dto.UserId))
                return;

            offCode.UserOffCodes.Add(new OffCodeUser
            {
                OffCodeId = dto.OffCodeId,
                UserId = dto.UserId,
                UsedCount = 0,
                AssignedDate = DateTime.Now
            });

            _offCodeRepository.UpdateEntity(offCode);
            await _offCodeRepository.SaveChanges();
        }
        public async Task EditOffCodeUserAsync(EditOffCodeUserDto dto)
        {
            var offCodeUser = await _offCodeUserRepository.GetEntityById(dto.Id);
            if (offCodeUser == null)
                throw new Exception("کاربر یافت نشد");

            offCodeUser.UserId = dto.UserId;

            _offCodeUserRepository.UpdateEntity(offCodeUser);
            await _offCodeUserRepository.SaveChanges();
        }

        public async Task DeleteOffCodeUserAsync(long id)
        {
            var entity = await _offCodeUserRepository.GetEntityById(id);
            if (entity == null) return;

            _offCodeUserRepository.RemoveEntity(entity);
            await _offCodeUserRepository.SaveChanges();
        }


        #endregion

        public void Dispose()
        {
            _offCodeRepository?.Dispose();
        }
    }
}
