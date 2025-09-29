using DidMark.Core.DTO.Newsletter;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DidMark.Core.Services.Interfaces
{
    public interface INewsletterService
    {
        #region Newsletter Subscribers
        Task<List<NewsletterDTO>> GetAllSubscribers();
        Task<FilterNewsletterDTO> FilterSubscribers(FilterNewsletterDTO filter);
        Task<NewsletterDTO?> GetSubscriberById(long id);
        Task<NewsletterDTO?> GetSubscriberByEmail(string email);
        Task<(bool Success, string Message, long Id)> AddSubscriber(AddNewsletterDTO dto);
        Task<bool> DeleteSubscriber(long id);
        Task<bool> ToggleStatus(long id);
        Task<bool> Unsubscribe(string email);
        Task<int> GetActiveSubscribersCount();
        Task<int> GetTotalSubscribersCount();
        #endregion

        #region Campaign Management
        Task<List<CampaignDTO>> GetAllCampaigns();
        Task<CampaignDTO?> GetCampaignById(long id);
        Task<(bool Success, string Message, long Id)> CreateCampaign(AddCampaignDTO dto);
        Task<(bool Success, string Message)> UpdateCampaign(EditCampaignDTO dto);
        Task<(bool Success, string Message)> DeleteCampaign(long id);
        Task<(bool Success, string Message)> SendCampaign(SendCampaignDTO dto);
        Task<(bool Success, string Message)> SendTestCampaign(long campaignId, string testEmail);
        Task<CampaignStatsDTO> GetCampaignStats();
        Task<(bool Success, string Message)> UpdateCampaignStats(long campaignId, bool delivered = false, bool opened = false, bool clicked = false);
        #endregion

        void Dispose();
    }
}