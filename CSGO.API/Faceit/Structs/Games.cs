using System.Text.Json.Serialization;

namespace CSGO.API.Faceit
{
    public struct Games
    {
        [JsonPropertyName("csgo")]
        public Game CSGO { get; set; }

        [JsonPropertyName("pubg")]
        public Game PUBG { get; set; }
    }
}
