using DidMark.Core.DTO.Contact;
using DidMark.Core.Utilities.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DidMark.Core.Services.Interfaces
{
    public interface IContactUs : IDisposable
    {
        /// <summary>
        /// ارسال پیام جدید از سمت کاربر
        /// </summary>
        /// <param name="contact">اطلاعات پیام کاربر</param>
        /// <returns>نتیجه عملیات ارسال پیام</returns>
        Task<ContactResult> SendMessageAsync(ContactUsDTO contact);

        /// <summary>
        /// دریافت لیست پیام‌های تماس برای ادمین
        /// </summary>
        /// <returns>لیست پیام‌ها</returns>
        Task<List<ContactListDTO>> GetAllContactsAsync();

        /// <summary>
        /// دریافت جزئیات یک پیام
        /// </summary>
        /// <param name="id">شناسه پیام</param>
        /// <returns>جزئیات پیام</returns>
        Task<ContactDetailDTO?> GetContactDetailByIdAsync(long id);

        /// <summary>
        /// تغییر وضعیت پیام (خوانده‌شده یا خیر)
        /// </summary>
        /// <param name="id">شناسه پیام</param>
        /// <returns>نتیجه تغییر وضعیت</returns>
        Task<ContactResult> MarkAsReadAsync(long id);
        Task<ContactResult> ReplyToContactAsync(ContactReplyEmailDTO reply);
    }
}
