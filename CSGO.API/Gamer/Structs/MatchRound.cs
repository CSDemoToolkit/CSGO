using System.Text.Json.Serialization;

namespace CSGO.API.Gamer
{
    public struct MatchRound
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("roundable_type")]
        public string RoundableType { get; set; }

        [JsonPropertyName("roundable_id")]
        public int RoundableID { get; set; }

        [JsonPropertyName("number")]
        public int Number { get; set; }

        [JsonPropertyName("identifier")]
        public string Identifier { get; set; }

        [JsonPropertyName("bracket")]
        public string Bracket { get; set; }

        [JsonPropertyName("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("competition_id")]
        public int CompetitionID { get; set; }

        [JsonPropertyName("identifier_text")]
        public string IdentifierText { get; set; }
    }
}
