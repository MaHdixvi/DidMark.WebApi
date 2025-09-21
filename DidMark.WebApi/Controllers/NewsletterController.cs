using DidMark.Core.DTO.Newsletter;
using DidMark.Core.Services.Interfaces;
using DidMark.Core.Utilities.Common;
using DidMark.WebApi.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DidMark.WebApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]

    public class NewsletterController : SiteBaseController
    {
        private readonly INewsletterService _newsletterService;

        public NewsletterController(INewsletterService newsletterService)
        {
            _newsletterService = newsletterService;
        }

        [HttpGet]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> GetAllSubscribers()
        {
            var subscribers = await _newsletterService.GetAllSubscribers();
            return JsonResponseStatus.Success(subscribers);
        }

        [HttpGet("{id:long}")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> GetSubscriber(long id)
        {
            var subscriber = await _newsletterService.GetSubscriberById(id);
            if (subscriber == null)
                return JsonResponseStatus.NotFound(new { message = "عضو خبرنامه یافت نشد" });

            return JsonResponseStatus.Success(subscriber);
        }

        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] AddNewsletterDTO dto)
        {
            var (success, subscriberId) = await _newsletterService.AddSubscriber(dto);

            if (!success)
                return JsonResponseStatus.Success(new { success = false, data = (object)null, message = "ایمیل قبلاً ثبت شده است" });

            return JsonResponseStatus.Success(new { success = true, data = new { email = dto.Email, id = subscriberId }, message = "عضویت شما با موفقیت ثبت شد" });
        }


        [HttpDelete("{id:long}")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> DeleteSubscriber(long id)
        {
            var success = await _newsletterService.DeleteSubscriber(id);
            if (!success)
                return JsonResponseStatus.NotFound(new { message = "عضو خبرنامه یافت نشد" });

            return JsonResponseStatus.Success(new { message = "عضو خبرنامه حذف شد" });
        }
        [HttpPut("toggle-status/{id:long}")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> ToggleStatus(long id)
        {
            var success = await _newsletterService.ToggleStatus(id);
            if (!success)
                return JsonResponseStatus.NotFound(new { message = "عضو خبرنامه یافت نشد" });

            return JsonResponseStatus.Success(new { message = "وضعیت عضو خبرنامه تغییر یافت" });
        }

    }
}
