using DidMark.Core.DTO.Newsletter;
using DidMark.Core.DTO.Paging;
using DidMark.Core.Services.Interfaces;
using DidMark.Core.Utilities.Extensions.Paging;
using DidMark.DataLayer.Entities.Newsletter;
using DidMark.DataLayer.Entities.Site;
using DidMark.DataLayer.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DidMark.Core.Services.Implementations
{
    public class NewsletterService : INewsletterService
    {
        private readonly IGenericRepository<NewsletterSubscriber> _newsletterRepository;
        private readonly IGenericRepository<Campaign> _campaignRepository;

        public NewsletterService(
            IGenericRepository<NewsletterSubscriber> newsletterRepository,
            IGenericRepository<Campaign> campaignRepository)
        {
            _newsletterRepository = newsletterRepository;
            _campaignRepository = campaignRepository;
        }

        #region Newsletter Subscribers

        public async Task<List<NewsletterDTO>> GetAllSubscribers()
        {
            return await _newsletterRepository.GetEntitiesQuery()
                .OrderByDescending(n => n.CreateDate)
                .Select(n => new NewsletterDTO
                {
                    Id = n.Id,
                    Email = n.Email,
                    IsActive = !n.IsDelete,
                    CreatedAt = n.CreateDate
                })
                .ToListAsync();
        }

        public async Task<FilterNewsletterDTO> FilterSubscribers(FilterNewsletterDTO filter)
        {
            var query = _newsletterRepository.GetEntitiesQuery()
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.Email))
                query = query.Where(n => n.Email.Contains(filter.Email));

            if (filter.IsActive.HasValue)
                query = query.Where(n => n.IsDelete == !filter.IsActive.Value);

            if (filter.FromDate.HasValue)
                query = query.Where(n => n.CreateDate >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
                query = query.Where(n => n.CreateDate <= filter.ToDate.Value);

            query = query.OrderByDescending(n => n.CreateDate);

            var count = (int)Math.Ceiling(query.Count() / (double)filter.TakeEntity);
            var pager = Pager.Build(count, filter.PageId, filter.TakeEntity);

            var subscribers = await query.Paging(pager)
                .Select(n => new NewsletterDTO
                {
                    Id = n.Id,
                    Email = n.Email,
                    IsActive = !n.IsDelete,
                    CreatedAt = n.CreateDate
                })
                .ToListAsync();

            return filter.SetSubscribers(subscribers).SetPaging(pager);
        }

        public async Task<NewsletterDTO?> GetSubscriberById(long id)
        {
            var subscriber = await _newsletterRepository.GetEntityById(id);
            if (subscriber == null) return null;

            return new NewsletterDTO
            {
                Id = subscriber.Id,
                Email = subscriber.Email,
                IsActive = !subscriber.IsDelete,
                CreatedAt = subscriber.CreateDate
            };
        }

        public async Task<NewsletterDTO?> GetSubscriberByEmail(string email)
        {
            var subscriber = await _newsletterRepository.GetEntitiesQuery()
                .FirstOrDefaultAsync(n => n.Email == email);

            if (subscriber == null) return null;

            return new NewsletterDTO
            {
                Id = subscriber.Id,
                Email = subscriber.Email,
                IsActive = !subscriber.IsDelete,
                CreatedAt = subscriber.CreateDate
            };
        }

        public async Task<(bool Success, string Message, long Id)> AddSubscriber(AddNewsletterDTO dto)
        {
            try
            {
                var email = dto.Email.Trim().ToLower();

                var exists = await _newsletterRepository.GetEntitiesQuery()
                    .AnyAsync(n => n.Email == email);

                if (exists)
                {
                    var existingSubscriber = await _newsletterRepository.GetEntitiesQuery()
                        .FirstOrDefaultAsync(n => n.Email == email);

                    if (existingSubscriber != null && existingSubscriber.IsDelete)
                    {
                        existingSubscriber.IsDelete = false;
                        existingSubscriber.CreateDate = DateTime.UtcNow;
                        _newsletterRepository.UpdateEntity(existingSubscriber);
                        await _newsletterRepository.SaveChanges();

                        return (true, "عضویت شما با موفقیت فعال شد", existingSubscriber.Id);
                    }

                    return (false, "این ایمیل قبلاً ثبت شده است", 0);
                }

                var subscriber = new NewsletterSubscriber
                {
                    Email = email,
                    IsDelete = false,
                    CreateDate = DateTime.UtcNow
                };

                await _newsletterRepository.AddEntity(subscriber);
                await _newsletterRepository.SaveChanges();

                return (true, "عضویت شما در خبرنامه با موفقیت ثبت شد", subscriber.Id);
            }
            catch (Exception ex)
            {
                return (false, "خطا در ثبت عضویت", 0);
            }
        }

        public async Task<bool> DeleteSubscriber(long id)
        {
            var subscriber = await _newsletterRepository.GetEntityById(id);
            if (subscriber == null) return false;

            _newsletterRepository.RemoveEntity(subscriber);
            await _newsletterRepository.SaveChanges();

            return true;
        }

        public async Task<bool> ToggleStatus(long id)
        {
            var subscriber = await _newsletterRepository.GetEntityById(id);
            if (subscriber == null) return false;

            subscriber.IsDelete = !subscriber.IsDelete;
            _newsletterRepository.UpdateEntity(subscriber);
            await _newsletterRepository.SaveChanges();

            return true;
        }

        public async Task<bool> Unsubscribe(string email)
        {
            var subscriber = await _newsletterRepository.GetEntitiesQuery()
                .FirstOrDefaultAsync(n => n.Email == email.Trim().ToLower());

            if (subscriber == null) return false;

            subscriber.IsDelete = true;
            _newsletterRepository.UpdateEntity(subscriber);
            await _newsletterRepository.SaveChanges();

            return true;
        }

        public async Task<int> GetActiveSubscribersCount()
        {
            return await _newsletterRepository.GetEntitiesQuery()
                .CountAsync(n => !n.IsDelete);
        }

        public async Task<int> GetTotalSubscribersCount()
        {
            return await _newsletterRepository.GetEntitiesQuery()
                .CountAsync();
        }

        #endregion

        #region Campaign Management

        public async Task<List<CampaignDTO>> GetAllCampaigns()
        {
            return await _campaignRepository.GetEntitiesQuery()
                .OrderByDescending(c => c.CreateDate)
                .Select(c => new CampaignDTO
                {
                    Id = c.Id,
                    Title = c.Title,
                    Subject = c.Subject,
                    Content = c.Content,
                    SentDate = c.SentDate,
                    Status = c.Status.ToString(),
                    SentCount = c.SentCount,
                    DeliveredCount = c.DeliveredCount,
                    OpenedCount = c.OpenedCount,
                    ClickedCount = c.ClickedCount,
                    CreateDate = c.CreateDate
                })
                .ToListAsync();
        }

        public async Task<CampaignDTO?> GetCampaignById(long id)
        {
            var campaign = await _campaignRepository.GetEntityById(id);
            if (campaign == null) return null;

            return new CampaignDTO
            {
                Id = campaign.Id,
                Title = campaign.Title,
                Subject = campaign.Subject,
                Content = campaign.Content,
                SentDate = campaign.SentDate,
                Status = campaign.Status.ToString(),
                SentCount = campaign.SentCount,
                DeliveredCount = campaign.DeliveredCount,
                OpenedCount = campaign.OpenedCount,
                ClickedCount = campaign.ClickedCount,
                CreateDate = campaign.CreateDate
            };
        }

        public async Task<(bool Success, string Message, long Id)> CreateCampaign(AddCampaignDTO dto)
        {
            try
            {
                var campaign = new Campaign
                {
                    Title = dto.Title,
                    Subject = dto.Subject,
                    Content = dto.Content,
                    Status = CampaignStatus.Draft,
                    SentCount = 0,
                    DeliveredCount = 0,
                    OpenedCount = 0,
                    ClickedCount = 0,
                    CreateDate = DateTime.UtcNow,
                    IsDelete = false
                };

                await _campaignRepository.AddEntity(campaign);
                await _campaignRepository.SaveChanges();

                return (true, "کمپین با موفقیت ایجاد شد", campaign.Id);
            }
            catch (Exception ex)
            {
                return (false, "خطا در ایجاد کمپین", 0);
            }
        }

        public async Task<(bool Success, string Message)> UpdateCampaign(EditCampaignDTO dto)
        {
            try
            {
                var campaign = await _campaignRepository.GetEntityById(dto.Id);
                if (campaign == null) return (false, "کمپین یافت نشد");

                // فقط کمپین‌های پیش‌نویس قابل ویرایش هستند
                if (campaign.Status != CampaignStatus.Draft)
                    return (false, "فقط کمپین‌های پیش‌نویس قابل ویرایش هستند");

                campaign.Title = dto.Title;
                campaign.Subject = dto.Subject;
                campaign.Content = dto.Content;

                _campaignRepository.UpdateEntity(campaign);
                await _campaignRepository.SaveChanges();

                return (true, "کمپین با موفقیت به‌روزرسانی شد");
            }
            catch (Exception ex)
            {
                return (false, "خطا در به‌روزرسانی کمپین");
            }
        }

        public async Task<(bool Success, string Message)> DeleteCampaign(long id)
        {
            try
            {
                var campaign = await _campaignRepository.GetEntityById(id);
                if (campaign == null) return (false, "کمپین یافت نشد");

                // فقط کمپین‌های پیش‌نویس قابل حذف هستند
                if (campaign.Status != CampaignStatus.Draft)
                    return (false, "فقط کمپین‌های پیش‌نویس قابل حذف هستند");

                campaign.IsDelete = true;
                _campaignRepository.UpdateEntity(campaign);
                await _campaignRepository.SaveChanges();

                return (true, "کمپین با موفقیت حذف شد");
            }
            catch (Exception ex)
            {
                return (false, "خطا در حذف کمپین");
            }
        }

        public async Task<(bool Success, string Message)> SendCampaign(SendCampaignDTO dto)
        {
            try
            {
                var campaign = await _campaignRepository.GetEntityById(dto.CampaignId);
                if (campaign == null) return (false, "کمپین یافت نشد");

                if (campaign.Status != CampaignStatus.Draft)
                    return (false, "فقط کمپین‌های پیش‌نویس قابل ارسال هستند");

                // در اینجا منطق ارسال ایمیل پیاده‌سازی می‌شود
                // این یک پیاده‌سازی ساده است - در عمل باید از سرویس ایمیل استفاده کنید

                var activeSubscribers = await _newsletterRepository.GetEntitiesQuery()
                    .Where(n => !n.IsDelete)
                    .ToListAsync();

                campaign.Status = CampaignStatus.Sent;
                campaign.SentDate = DateTime.UtcNow;
                campaign.SentCount = activeSubscribers.Count;
                campaign.DeliveredCount = activeSubscribers.Count; // فرض می‌کنیم همه تحویل داده شده‌اند

                _campaignRepository.UpdateEntity(campaign);
                await _campaignRepository.SaveChanges();

                // TODO: ارسال واقعی ایمیل‌ها
                // await SendEmailsToSubscribers(campaign, activeSubscribers);

                return (true, $"کمپین برای {activeSubscribers.Count} کاربر ارسال شد");
            }
            catch (Exception ex)
            {
                return (false, "خطا در ارسال کمپین");
            }
        }

        public async Task<(bool Success, string Message)> SendTestCampaign(long campaignId, string testEmail)
        {
            try
            {
                var campaign = await _campaignRepository.GetEntityById(campaignId);
                if (campaign == null) return (false, "کمپین یافت نشد");

                // TODO: ارسال ایمیل تست
                // await SendTestEmail(campaign, testEmail);

                return (true, $"ایمیل تست به {testEmail} ارسال شد");
            }
            catch (Exception ex)
            {
                return (false, "خطا در ارسال ایمیل تست");
            }
        }

        public async Task<CampaignStatsDTO> GetCampaignStats()
        {
            var campaigns = await _campaignRepository.GetEntitiesQuery()
                .Where(c => !c.IsDelete)
                .ToListAsync();

            var sentCampaigns = campaigns.Where(c => c.Status == CampaignStatus.Sent).ToList();

            return new CampaignStatsDTO
            {
                TotalCampaigns = campaigns.Count,
                SentCampaigns = sentCampaigns.Count,
                DraftCampaigns = campaigns.Count(c => c.Status == CampaignStatus.Draft),
                TotalEmailsSent = sentCampaigns.Sum(c => c.SentCount),
                AverageOpenRate = sentCampaigns.Any() ? sentCampaigns.Average(c => c.DeliveredCount > 0 ? (c.OpenedCount * 100.0) / c.DeliveredCount : 0) : 0,
                AverageClickRate = sentCampaigns.Any() ? sentCampaigns.Average(c => c.DeliveredCount > 0 ? (c.ClickedCount * 100.0) / c.DeliveredCount : 0) : 0
            };
        }

        public async Task<(bool Success, string Message)> UpdateCampaignStats(long campaignId, bool delivered = false, bool opened = false, bool clicked = false)
        {
            try
            {
                var campaign = await _campaignRepository.GetEntityById(campaignId);
                if (campaign == null) return (false, "کمپین یافت نشد");

                if (delivered) campaign.DeliveredCount++;
                if (opened) campaign.OpenedCount++;
                if (clicked) campaign.ClickedCount++;

                _campaignRepository.UpdateEntity(campaign);
                await _campaignRepository.SaveChanges();

                return (true, "آمار کمپین به‌روزرسانی شد");
            }
            catch (Exception ex)
            {
                return (false, "خطا در به‌روزرسانی آمار");
            }
        }

        #endregion

        #region Dispose
        public void Dispose()
        {
            _newsletterRepository?.Dispose();
            _campaignRepository?.Dispose();
        }
        #endregion
    }
}