using Newtonsoft.Json;

namespace DidMark.Core.DTO.ZarinPal.UnVerification
{
    public class UnVerificationRequest
    {
        [JsonProperty("merchant_id")]
        public string MerchantId { get; set; }
    }
}