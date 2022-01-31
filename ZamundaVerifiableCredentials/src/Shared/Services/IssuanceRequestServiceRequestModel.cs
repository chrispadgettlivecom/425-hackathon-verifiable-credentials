using Newtonsoft.Json;

namespace Shared.Services
{
    public class IssuanceRequestServiceRequestModel
    {
        [JsonProperty("includeQRCode")]
        public bool? IncludeQrCode { get; set; }

        [JsonProperty("authority")]
        public string Authority { get; set; }

        [JsonProperty("registration")]
        public RequestRegistrationModel Registration { get; set; }

        [JsonProperty("issuance")]
        public RequestIssuanceModel Issuance { get; set; }

        [JsonProperty("callback")]
        public CallbackModel Callback { get; set; }
    }
}
