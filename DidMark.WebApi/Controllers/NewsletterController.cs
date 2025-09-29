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

        #region Public Routes
        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] AddNewsletterDTO dto)
        {
            var (success, message, subscriberId) = await _newsletterService.AddSubscriber(dto);

            if (!success)
                return JsonResponseStatus.Success(new { success = false, message = message });

            return JsonResponseStatus.Success(new { success = true, data = new { email = dto.Email, id = subscriberId }, message = message });
        }

        [HttpPost("unsubscribe")]
        public async Task<IActionResult> Unsubscribe([FromBody] string email)
        {
            var success = await _newsletterService.Unsubscribe(email);
            if (!success)
                return JsonResponseStatus.NotFound(new { message = "ایمیل یافت نشد" });

            return JsonResponseStatus.Success(new { message = "لغو عضویت با موفقیت انجام شد" });
        }
        #endregion

        #region Admin Routes - Subscribers
        [HttpGet("subscribers")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> GetAllSubscribers()
        {
            var subscribers = await _newsletterService.GetAllSubscribers();
            return JsonResponseStatus.Success(subscribers);
        }

        [HttpGet("subscribers/filter")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> FilterSubscribers([FromQuery] FilterNewsletterDTO filter)
        {
            var result = await _newsletterService.FilterSubscribers(filter);
            return JsonResponseStatus.Success(result);
        }

        [HttpGet("subscribers/{id:long}")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> GetSubscriber(long id)
        {
            var subscriber = await _newsletterService.GetSubscriberById(id);
            if (subscriber == null)
                return JsonResponseStatus.NotFound(new { message = "عضو خبرنامه یافت نشد" });

            return JsonResponseStatus.Success(subscriber);
        }

        [HttpDelete("subscribers/{id:long}")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> DeleteSubscriber(long id)
        {
            var success = await _newsletterService.DeleteSubscriber(id);
            if (!success)
                return JsonResponseStatus.NotFound(new { message = "عضو خبرنامه یافت نشد" });

            return JsonResponseStatus.Success(new { message = "عضو خبرنامه حذف شد" });
        }

        [HttpPut("subscribers/toggle-status/{id:long}")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> ToggleSubscriberStatus(long id)
        {
            var success = await _newsletterService.ToggleStatus(id);
            if (!success)
                return JsonResponseStatus.NotFound(new { message = "عضو خبرنامه یافت نشد" });

            return JsonResponseStatus.Success(new { message = "وضعیت عضو خبرنامه تغییر یافت" });
        }

        [HttpGet("subscribers/stats")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> GetSubscriberStats()
        {
            var activeCount = await _newsletterService.GetActiveSubscribersCount();
            var totalCount = await _newsletterService.GetTotalSubscribersCount();

            return JsonResponseStatus.Success(new
            {
                activeSubscribers = activeCount,
                totalSubscribers = totalCount,
                inactiveSubscribers = totalCount - activeCount
            });
        }
        #endregion

        #region Admin Routes - Campaigns
        [HttpGet("campaigns")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> GetAllCampaigns()
        {
            var campaigns = await _newsletterService.GetAllCampaigns();
            return JsonResponseStatus.Success(campaigns);
        }

        [HttpGet("campaigns/{id:long}")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> GetCampaign(long id)
        {
            var campaign = await _newsletterService.GetCampaignById(id);
            if (campaign == null)
                return JsonResponseStatus.NotFound(new { message = "کمپین یافت نشد" });

            return JsonResponseStatus.Success(campaign);
        }

        [HttpPost("campaigns")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> CreateCampaign([FromBody] AddCampaignDTO dto)
        {
            var (success, message, campaignId) = await _newsletterService.CreateCampaign(dto);

            if (!success)
                return JsonResponseStatus.Error(new { message = message });

            return JsonResponseStatus.Success(new { success = true, data = new { id = campaignId }, message = message });
        }

        [HttpPut("campaigns")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> UpdateCampaign([FromBody] EditCampaignDTO dto)
        {
            var (success, message) = await _newsletterService.UpdateCampaign(dto);

            if (!success)
                return JsonResponseStatus.Error(new { message = message });

            return JsonResponseStatus.Success(new { success = true, message = message });
        }

        [HttpDelete("campaigns/{id:long}")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> DeleteCampaign(long id)
        {
            var (success, message) = await _newsletterService.DeleteCampaign(id);

            if (!success)
                return JsonResponseStatus.Error(new { message = message });

            return JsonResponseStatus.Success(new { success = true, message = message });
        }

        [HttpPost("campaigns/send")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> SendCampaign([FromBody] SendCampaignDTO dto)
        {
            var (success, message) = await _newsletterService.SendCampaign(dto);

            if (!success)
                return JsonResponseStatus.Error(new { message = message });

            return JsonResponseStatus.Success(new { success = true, message = message });
        }

        [HttpPost("campaigns/send-test")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> SendTestCampaign([FromBody] SendTestCampaignDTO dto)
        {
            var (success, message) = await _newsletterService.SendTestCampaign(dto.CampaignId, dto.TestEmail);

            if (!success)
                return JsonResponseStatus.Error(new { message = message });

            return JsonResponseStatus.Success(new { success = true, message = message });
        }

        [HttpGet("campaigns/stats")]
        [PermissionChecker("Admin")]
        public async Task<IActionResult> GetCampaignStats()
        {
            var stats = await _newsletterService.GetCampaignStats();
            return JsonResponseStatus.Success(stats);
        }
        #endregion
    }

    public class SendTestCampaignDTO
    {
        public long CampaignId { get; set; }
        public string TestEmail { get; set; }
    }
}