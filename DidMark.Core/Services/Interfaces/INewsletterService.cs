using DidMark.Core.DTO.Newsletter;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DidMark.Core.Services.Interfaces
{
    public interface INewsletterService : IDisposable
    {
        Task<List<NewsletterDTO>> GetAllSubscribers();
        Task<NewsletterDTO?> GetSubscriberById(long id);
        Task<(bool Success, long Id)> AddSubscriber(AddNewsletterDTO dto);
        Task<bool> DeleteSubscriber(long id);
        Task<bool> ToggleStatus(long id);

    }
}
