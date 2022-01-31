using Newtonsoft.Json;

namespace Shared.Services
{
    public class RequestRegistrationModel
    {
        [JsonProperty("clientName")]
        public string ClientName { get; set; }

        [JsonProperty("logoUrl")]
        public string LogoUrl { get; set; }

        [JsonProperty("termsOfServiceUrl")]
        public string TermsOfServiceUrl { get; set; }
    }
}
