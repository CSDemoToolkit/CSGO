using System.Text.Json.Serialization;

namespace CSGO.API.Gamer
{
    public struct MatchupUser
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("user_id")]
        public int UserID { get; set; }

        [JsonPropertyName("matchup_id")]
        public int MatchupID { get; set; }

        [JsonPropertyName("team_id")]
        public int TeamID { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("Stats")]
        public List<MatchUserStats> Stats { get; set; }

        [JsonPropertyName("user")]
        public MatchUser User { get; set; }
    }
}
