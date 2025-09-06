using DidMark.Core.DTO.Contact;
using DidMark.Core.Services.Interfaces;
using DidMark.Core.Utilities.Common;
using DidMark.Core.Utilities.Enums;
using DidMark.WebApi.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DidMark.WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ContactController : SiteBaseController
    {
        #region Fields
        private readonly IContactUs _contactUs;
        #endregion

        #region Constructor
        public ContactController(IContactUs contactUs)
        {
            _contactUs = contactUs ?? throw new ArgumentNullException(nameof(contactUs));
        }
        #endregion

        #region Submit Contact
        [HttpPost("submit-contact")]
        public async Task<IActionResult> SubmitContact([FromBody] ContactUsDTO contactUsDto)
        {
            if (contactUsDto is null)
                return JsonResponseStatus.BadRequest(new { message = "داده‌های پیام ارائه نشده است" });

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                                       .SelectMany(v => v.Errors)
                                       .Select(e => e.ErrorMessage)
                                       .ToList();

                return JsonResponseStatus.BadRequest(new { message = "داده‌های ورودی نامعتبر است", errors });
            }

            var result = await _contactUs.SendMessageAsync(contactUsDto);

            return result switch
            {
                ContactResult.Success => JsonResponseStatus.Success(new { message = "پیام با موفقیت ارسال شد" }),
                ContactResult.InvalidData => JsonResponseStatus.BadRequest(new { message = "اطلاعات پیام نامعتبر است" }),
                ContactResult.Error => JsonResponseStatus.Error(new { message = "خطایی در ارسال پیام رخ داد" }),
                _ => JsonResponseStatus.Error(new { message = "خطای ناشناخته رخ داد" })
            };
        }
        #endregion

        #region Get All Contacts
        [HttpGet("all-contacts")]
        public async Task<IActionResult> GetAllContacts()
        {
            var contacts = await _contactUs.GetAllContactsAsync();
            return JsonResponseStatus.Success(contacts);
        }
        #endregion

        #region Get Contact Detail
        [HttpGet("detail/{id:long}")]
        public async Task<IActionResult> GetContactDetail(long id)
        {
            if (id <= 0)
                return JsonResponseStatus.BadRequest(new { message = "شناسه پیام نامعتبر است" });

            var contact = await _contactUs.GetContactDetailByIdAsync(id);
            if (contact is null)
                return JsonResponseStatus.NotFound(new { message = "پیام موردنظر پیدا نشد" });

            return JsonResponseStatus.Success(contact);
        }
        #endregion

        #region Mark As Read
        [HttpPost("mark-as-read/{id:long}")]
        public async Task<IActionResult> MarkAsRead(long id)
        {
            var result = await _contactUs.MarkAsReadAsync(id);

            return result switch
            {
                ContactResult.Success => JsonResponseStatus.Success(new { message = "پیام با موفقیت علامت‌گذاری شد" }),
                ContactResult.NotFound => JsonResponseStatus.NotFound(new { message = "پیام موردنظر پیدا نشد" }),
                ContactResult.InvalidData => JsonResponseStatus.BadRequest(new { message = "شناسه پیام نامعتبر است" }),
                ContactResult.Error => JsonResponseStatus.Error(new { message = "خطایی در علامت‌گذاری پیام رخ داد" }),
                _ => JsonResponseStatus.Error(new { message = "خطای ناشناخته رخ داد" })
            };
        }
        #endregion

        #region Reply To Contact
        [HttpPost("reply")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> ReplyToContact([FromBody] ContactReplyEmailDTO replyDto)
        {
            if (replyDto is null || string.IsNullOrWhiteSpace(replyDto.UserEmail))
                return JsonResponseStatus.BadRequest(new { message = "اطلاعات پاسخ‌دهی ناقص است" });

            var result = await _contactUs.ReplyToContactAsync(replyDto);

            return result switch
            {
                ContactResult.Success => JsonResponseStatus.Success(new { message = "پاسخ ایمیل با موفقیت ارسال شد" }),
                ContactResult.InvalidData => JsonResponseStatus.BadRequest(new { message = "اطلاعات پاسخ‌دهی نامعتبر است" }),
                ContactResult.Error => JsonResponseStatus.Error(new { message = "خطایی در ارسال پاسخ رخ داد" }),
                _ => JsonResponseStatus.Error(new { message = "خطای ناشناخته رخ داد" })
            };
        }
        #endregion
    }
}
