using System.Text.Json.Serialization;

namespace CSGO.API.Gamer
{
    public struct MatchUserStats
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("matchup_user_id")]
        public int MatchupUserID { get; set; }

        [JsonPropertyName("character_resource_id")]
        public int CharacterResourceID { get; set; }

        [JsonPropertyName("map_id")]
        public int MapID { get; set; }

        [JsonPropertyName("kills")]
        public int Kills { get; set; }

        [JsonPropertyName("deaths")]
        public int Deaths { get; set; }

        [JsonPropertyName("assists")]
        public int Assists { get; set; }

        [JsonPropertyName("adr")]
        public string ADR { get; set; }

        [JsonPropertyName("rating")]
        public string Rating { get; set; }

        [JsonPropertyName("deleted_at")]
        public DateTime DeletedAt { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
