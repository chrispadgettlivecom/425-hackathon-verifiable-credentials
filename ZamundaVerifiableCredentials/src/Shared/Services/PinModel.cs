using Newtonsoft.Json;

namespace Shared.Services
{
    public class PinModel
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("length")]
        public int? Length { get; set; }

        [JsonProperty("salt")]
        public string Salt { get; set; }

        [JsonProperty("alg")]
        public string Alg { get; set; }

        [JsonProperty("iterations")]
        public int? Iterations { get; set; }
    }
}
