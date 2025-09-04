using DidMark.Core.Security.ContactUs;
using DidMark.Core.Services.Interfaces;
using DidMark.Core.Utilities.Common;
using Microsoft.AspNetCore.Mvc;
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
        /// <summary>
        /// Initializes a new instance of the <see cref="ContactController"/> class.
        /// </summary>
        /// <param name="contactUs">The contact us service for handling contact-related operations.</param>
        public ContactController(IContactUs contactUs)
        {
            _contactUs = contactUs ?? throw new ArgumentNullException(nameof(contactUs));
        }
        #endregion

        #region Submit Contact
        /// <summary>
        /// Submits a contact message using the provided contact data.
        /// </summary>
        /// <param name="contactUsDto">The contact data to submit.</param>
        /// <returns>A JSON response indicating success or failure.</returns>
        [HttpPost("submit-contact")]
        public async Task<IActionResult> SubmitContact([FromBody] ContactUsDTO contactUsDto)
        {
            if (contactUsDto == null)
            {
                return JsonResponseStatus.BadRequest(new { message = "داده‌های پیام ارائه نشده است" });
            }

            if (!ModelState.IsValid)
            {
                return JsonResponseStatus.Error(new
                {
                    message = "داده‌های ورودی نامعتبر است",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            var result = await _contactUs.SendMessage(contactUsDto);
            return result switch
            {
                ContactResult.Error => JsonResponseStatus.Error(new { message = "خطایی در ارسال پیام رخ داد" }),
                _ => JsonResponseStatus.Success(new { message = "پیام با موفقیت ارسال شد" })
            };
        }
        #endregion
    }
}
