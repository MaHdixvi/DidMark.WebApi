using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DidMark.Core.DTO.OffCodes;
using DidMark.DataLayer.Entities.Orders;

namespace DidMark.Core.Services.Interfaces
{
    public interface IOffCodeService : IDisposable
    {
        #region User Side

        /// <summary>
        /// بررسی و گرفتن اطلاعات کد تخفیف فعال
        /// </summary>
        Task<OffCodeResultDto?> GetActiveOffCodeByCodeAsync(
            string code,
            long? userId = null,
            long? productId = null,
            long? categoryId = null);

        /// <summary>
        /// فقط بررسی معتبر بودن کد تخفیف
        /// </summary>
        Task<bool> IsCodeValidAsync(
            string code,
            long? userId = null,
            long? productId = null,
            long? categoryId = null);

        /// <summary>
        /// افزایش تعداد استفاده از کد (بعد از ثبت سفارش موفق)
        /// </summary>
        Task IncreaseUsageAsync(long offCodeId, long? userId = null);

        #endregion

        #region Admin Side

        /// <summary>
        /// افزودن کد تخفیف جدید
        /// </summary>
        Task AddOffCodeAsync(CreateOffCodeDto dto);

        /// <summary>
        /// دریافت همه کدهای تخفیف برای پنل ادمین
        /// </summary>
        Task<List<OffCodeAdminDto>> GetAllOffCodesAsync();

        Task EditOffCodeAsync(UpdateOffCodeDto dto);

        /// <summary>
        /// حذف (غیرفعال‌سازی) کد تخفیف
        /// </summary>
        Task DeleteOffCodeAsync(long id);

        /// <summary>
        /// اختصاص دادن کد تخفیف به یک کاربر خاص (چند‌به‌چند)
        /// </summary>
        Task AddOffCodeUserAsync(AddOffCodeUserDto dto);
        Task EditOffCodeProductAsync(EditOffCodeProductDto dto);

        Task DeleteOffCodeProductAsync(long id);

        /// <summary>
        /// افزودن کد تخفیف به محصول خاص
        /// </summary>
        Task AddOffCodeProductAsync(AddOffCodeProductDto dto);
        Task EditOffCodeCategoryAsync(EditOffCodeCategoryDto dto);
        Task DeleteOffCodeCategoryAsync(long id);

        /// <summary>
        /// افزودن کد تخفیف به دسته‌بندی خاص
        /// </summary>
        Task AddOffCodeCategoryAsync(AddOffCodeCategoryDto dto);
        Task EditOffCodeUserAsync(EditOffCodeUserDto dto);
        Task DeleteOffCodeUserAsync(long id);

        #endregion
    }
}
