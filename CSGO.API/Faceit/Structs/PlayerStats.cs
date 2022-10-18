using System.Text.Json.Serialization;

namespace CSGO.API.Faceit
{
    public struct PlayerStats
    {
        [JsonPropertyName("player_id")]
        public Guid PlayerID { get; set; }

        [JsonPropertyName("game_id")]
        public string GameID { get; set; }

        [JsonPropertyName("segments")]
        public List<MapStats> Segments { get; set; }
    }
}
