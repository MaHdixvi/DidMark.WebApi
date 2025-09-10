using Newtonsoft.Json;

namespace DidMark.Core.DTO.ZarinPal.ReFound
{
    public class ReFoundRequest
    {
        [JsonProperty("merchant_id")]
        public string MerchantId { get; set; }

        [JsonProperty("authority")]
        public string Authority { get; set; }

    }
}