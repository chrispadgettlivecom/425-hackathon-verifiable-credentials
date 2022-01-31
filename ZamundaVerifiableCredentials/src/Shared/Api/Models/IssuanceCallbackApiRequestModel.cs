using Newtonsoft.Json;

namespace Shared.Api.Models
{
    public class IssuanceCallbackApiRequestModel
    {
        [JsonProperty("requestId")]
        public string RequestId { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("error")]
        public IssuanceCallbackErrorApiRequestModel Error { get; set; }
    }
}
