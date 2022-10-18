using System.Text.Json.Serialization;

namespace CSGO.API.Gamer
{
    public struct MatchupResource
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("game_id")]
        public int GameID { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("image_id")]
        public int ImageID { get; set; }

        [JsonPropertyName("remote_id")]
        public string RemoteID { get; set; }

        [JsonPropertyName("preset")]
        public string Preset { get; set; }

        [JsonPropertyName("sort_order")]
        public string SortOrder { get; set; }

        [JsonPropertyName("active")]
        public bool Active { get; set; }

        [JsonPropertyName("deleted_at")]
        public DateTime DeletedAt { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
