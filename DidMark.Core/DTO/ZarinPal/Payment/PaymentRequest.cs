using Newtonsoft.Json;

namespace DidMark.Core.DTO.ZarinPal.Payment
{
    public class PaymentRequest
    {
        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("callback_url")]
        public string CallbackUrl { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("merchant_id")]
        public string MerchantId { get; set; }
    }
}
