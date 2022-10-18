using System.Text.Json.Serialization;

namespace CSGO.API.Gamer
{
    public struct MatchupTable
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("start_time")]
        public DateTime StartTime { get; set; }

        [JsonPropertyName("competition_id")]
        public int CompetitionID { get; set; }

        [JsonPropertyName("deleted_at")]
        public DateTime DeletedAt { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("order")]
        public int Order { get; set; }

        [JsonPropertyName("status_text")]
        public string StatusText { get; set; }
    }
}
