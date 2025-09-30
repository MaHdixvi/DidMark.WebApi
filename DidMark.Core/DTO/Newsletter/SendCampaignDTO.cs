namespace DidMark.Core.DTO.Newsletter
{
    public class SendCampaignDTO
    {
        public long CampaignId { get; set; }
        public bool SendToAll { get; set; } = true;
        public string[] TestEmails { get; set; }
    }
}