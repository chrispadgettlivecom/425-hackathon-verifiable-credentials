using Newtonsoft.Json;

namespace Shared.Services
{
    public class IssuanceRequestServiceResponseModel
    {
        [JsonProperty("requestId")]
        public string RequestId { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("expiry")]
        public int Expiry { get; set; }

        [JsonProperty("qrCode")]
        public string QrCode { get; set; }
    }
}
