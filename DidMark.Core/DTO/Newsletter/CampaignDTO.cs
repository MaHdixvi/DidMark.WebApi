using System;

namespace DidMark.Core.DTO.Newsletter
{
    public class CampaignDTO
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public DateTime? SentDate { get; set; }
        public string Status { get; set; }
        public int SentCount { get; set; }
        public int DeliveredCount { get; set; }
        public int OpenedCount { get; set; }
        public int ClickedCount { get; set; }
        public DateTime CreateDate { get; set; }
        public double DeliveryRate => SentCount > 0 ? (DeliveredCount * 100.0) / SentCount : 0;
        public double OpenRate => DeliveredCount > 0 ? (OpenedCount * 100.0) / DeliveredCount : 0;
        public double ClickRate => DeliveredCount > 0 ? (ClickedCount * 100.0) / DeliveredCount : 0;
    }
}