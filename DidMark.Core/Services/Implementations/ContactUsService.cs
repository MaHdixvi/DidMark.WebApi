using DidMark.Core.DTO.Contact;
using DidMark.Core.Security;
using DidMark.Core.Services.Interfaces;
using DidMark.Core.Utilities.Common;
using DidMark.Core.Utilities.Convertors;
using DidMark.Core.Utilities.Enums;
using DidMark.DataLayer.Entities.Site;
using DidMark.DataLayer.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DidMark.Core.Services.Implementations
{
    public class ContactUsService : IContactUs
    {
        #region Fields & Constructor

        private readonly IGenericRepository<Contact> _contactRepository;
        private readonly IMailSender _mailSender;
        private readonly IViewRenderService _renderView;

        public ContactUsService(
            IGenericRepository<Contact> contactRepository,
            IMailSender mailSender,
            IViewRenderService renderView)
        {
            _contactRepository = contactRepository ?? throw new ArgumentNullException(nameof(contactRepository));
            _mailSender = mailSender ?? throw new ArgumentNullException(nameof(mailSender));
            _renderView = renderView ?? throw new ArgumentNullException(nameof(renderView));
        }

        #endregion

        #region Queries

        public async Task<List<ContactListDTO>> GetAllContactsAsync()
        {
            return await _contactRepository.GetEntitiesQuery()
                .OrderByDescending(c => c.CreateDate)
                .AsNoTracking()
                .Select(c => new ContactListDTO
                {
                    Id = c.Id,
                    FullName = c.FullName,
                    Email = c.Email,
                    PhoneNumber = c.PhoneNumber,
                    Title = c.Title,
                    CreateDate = c.CreateDate,
                    IsRead = c.IsRead,
                    IsReplied = c.IsReplied
                })
                .ToListAsync();
        }

        public async Task<ContactDetailDTO?> GetContactDetailByIdAsync(long id)
        {
            if (id <= 0) return null;

            var contact = await _contactRepository.GetEntitiesQuery()
                .AsNoTracking()
                .SingleOrDefaultAsync(c => c.Id == id);

            if (contact == null) return null;

            return new ContactDetailDTO
            {
                Id = contact.Id,
                FullName = contact.FullName,
                Email = contact.Email,
                PhoneNumber = contact.PhoneNumber,
                Title = contact.Title,
                Description = contact.Description,
                CreateDate = contact.CreateDate,
                IsRead = contact.IsRead,
                IsReplied = contact.IsReplied,
                ReplyDate = contact.ReplyDate
            };
        }

        #endregion

        #region Commands

        public async Task<ContactResult> SendMessageAsync(ContactUsDTO contact)
        {
            if (contact == null) return ContactResult.InvalidData;

            try
            {
                var entity = new Contact
                {
                    FullName = contact.FullName?.SanitizeText(),
                    Email = contact.Email?.SanitizeText(),
                    PhoneNumber = contact.PhoneNumber?.SanitizeText(),
                    Title = contact.Title?.SanitizeText(),
                    Description = contact.Description?.SanitizeText(),
                    CreateDate = DateTime.Now,
                    IsRead = false,
                    IsReplied = false
                };

                await _contactRepository.AddEntity(entity);
                await _contactRepository.SaveChanges();

                return ContactResult.Success;
            }
            catch
            {
                return ContactResult.Error;
            }
        }

        public async Task<ContactResult> MarkAsReadAsync(long id)
        {
            if (id <= 0) return ContactResult.InvalidData;

            var contact = await _contactRepository.GetEntityById(id);
            if (contact == null) return ContactResult.NotFound;

            try
            {
                contact.IsRead = true;
                _contactRepository.UpdateEntity(contact);
                await _contactRepository.SaveChanges();

                return ContactResult.Success;
            }
            catch
            {
                return ContactResult.Error;
            }
        }

        /// <summary>
        /// پاسخ دادن به پیام و ارسال ایمیل به کاربر
        /// </summary>
        public async Task<ContactResult> ReplyToContactAsync(ContactReplyEmailDTO reply)
        {
            if (reply == null || string.IsNullOrWhiteSpace(reply.UserEmail))
                return ContactResult.InvalidData;

            try
            {
                // رندر قالب ایمیل
                var body = await _renderView.RenderToStringAsync("Email/EmailContactReply", reply);

                // ارسال ایمیل
                _mailSender.Send(reply.UserEmail, $"پاسخ به پیام شما: {reply.Title}", body);

                // بروزرسانی وضعیت پیام
                var contact = await _contactRepository.GetEntitiesQuery()
                    .SingleOrDefaultAsync(c => c.Email == reply.UserEmail && c.Title == reply.Title && !c.IsReplied);

                if (contact != null)
                {
                    contact.IsReplied = true;
                    contact.ReplyDate = DateTime.Now;
                    _contactRepository.UpdateEntity(contact);
                    await _contactRepository.SaveChanges();
                }

                return ContactResult.Success;
            }
            catch
            {
                return ContactResult.Error;
            }
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            _contactRepository?.Dispose();
        }

        #endregion
    }
}
