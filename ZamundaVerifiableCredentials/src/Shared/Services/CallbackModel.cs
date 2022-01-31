using System.Collections.Generic;
using Newtonsoft.Json;

namespace Shared.Services
{
    public class CallbackModel
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("headers")]
        public IDictionary<string, string> Headers { get; set; }
    }
}
