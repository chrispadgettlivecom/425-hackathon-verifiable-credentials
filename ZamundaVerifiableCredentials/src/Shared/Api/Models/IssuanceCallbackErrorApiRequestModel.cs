using Newtonsoft.Json;

namespace Shared.Api.Models
{
    public class IssuanceCallbackErrorApiRequestModel
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
