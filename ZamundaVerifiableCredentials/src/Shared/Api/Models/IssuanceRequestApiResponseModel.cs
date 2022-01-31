using Newtonsoft.Json;

namespace Shared.Api.Models
{
    public class IssuanceRequestApiResponseModel
    {
        [JsonProperty("requestId")]
        public string RequestId { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("expiry")]
        public int Expiry { get; set; }

        [JsonProperty("qrCode")]
        public string QrCode { get; set; }

        [JsonProperty("pinValue")]
        public string PinValue { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }
    }
}
