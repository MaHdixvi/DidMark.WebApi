using DidMark.Core.DTO.Newsletter;
using DidMark.Core.Services.Interfaces;
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

        public NewsletterService(IGenericRepository<NewsletterSubscriber> newsletterRepository)
        {
            _newsletterRepository = newsletterRepository;
        }

        public async Task<List<NewsletterDTO>> GetAllSubscribers()
        {
            return await _newsletterRepository.GetEntitiesQuery()
                .OrderByDescending(n => n.CreateDate)
                .Select(n => new NewsletterDTO
                {
                    Id = n.Id,
                    Email = n.Email,
                    IsActive = n.IsDelete,
                    CreatedAt = n.CreateDate
                })
                .ToListAsync();
        }

        public async Task<NewsletterDTO?> GetSubscriberById(long id)
        {
            var subscriber = await _newsletterRepository.GetEntityById(id);
            if (subscriber == null) return null;

            return new NewsletterDTO
            {
                Id = subscriber.Id,
                Email = subscriber.Email,
                IsActive = subscriber.IsDelete,
                CreatedAt = subscriber.CreateDate
            };
        }

        public async Task<(bool Success, long Id)> AddSubscriber(AddNewsletterDTO dto)
        {
            // بررسی ایمیل تکراری
            var exists = await _newsletterRepository.GetEntitiesQuery()
                .AnyAsync(n => n.Email == dto.Email);

            if (exists)
                return (false, 0);

            var subscriber = new NewsletterSubscriber
            {
                Email = dto.Email,
                IsDelete = true,
                CreateDate = DateTime.UtcNow
            };

            await _newsletterRepository.AddEntity(subscriber);
            await _newsletterRepository.SaveChanges();

            return (true, subscriber.Id);
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

            subscriber.IsDelete = !subscriber.IsDelete; // تغییر وضعیت فعال/غیرفعال
            _newsletterRepository.UpdateEntity(subscriber);
            await _newsletterRepository.SaveChanges();

            return true;
        }


        public void Dispose()
        {
            _newsletterRepository?.Dispose();
        }
    }
}
