using Newtonsoft.Json;

namespace DidMark.Core.DTO.ZarinPal.Verification
{
    public class VerificationRequest
    {
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("merchant_id")]
        public string MerchantId { get; set; }

        [JsonProperty("authority")]
        public string Authority { get; set; }
    }
}
