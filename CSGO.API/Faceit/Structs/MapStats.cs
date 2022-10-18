using System.Text.Json.Serialization;

namespace CSGO.API.Faceit
{
    public struct MapStats
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("mode")]
        public string Mode { get; set; }

        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("stats")]
        public Stats Stats { get; set; }
    }
}
