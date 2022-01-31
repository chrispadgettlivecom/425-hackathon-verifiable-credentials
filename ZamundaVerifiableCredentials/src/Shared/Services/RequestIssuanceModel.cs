using System.Collections.Generic;
using Newtonsoft.Json;

namespace Shared.Services
{
    public class RequestIssuanceModel
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("manifest")]
        public string Manifest { get; set; }

        [JsonProperty("claims")]
        public IDictionary<string, string> Claims { get; set; }

        [JsonProperty("pin")]
        public PinModel Pin { get; set; }
    }
}
