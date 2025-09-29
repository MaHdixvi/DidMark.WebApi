using DidMark.Core.DTO.Paging;
using System;
using System.Collections.Generic;

namespace DidMark.Core.DTO.Newsletter
{
    public class FilterNewsletterDTO : BasePaging
    {
        public string Email { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public List<NewsletterDTO> Subscribers { get; set; }

        public FilterNewsletterDTO()
        {
            Subscribers = new List<NewsletterDTO>();
        }

        public FilterNewsletterDTO SetPaging(BasePaging paging)
        {
            this.PageId = paging.PageId;
            this.PageCount = paging.PageCount;
            this.ActivePage = paging.ActivePage;
            this.StartPage = paging.StartPage;
            this.EndPage = paging.EndPage;
            this.TakeEntity = paging.TakeEntity;
            this.SkipEntity = paging.SkipEntity;
            return this;
        }

        public FilterNewsletterDTO SetSubscribers(List<NewsletterDTO> subscribers)
        {
            this.Subscribers = subscribers;
            return this;
        }
    }
}