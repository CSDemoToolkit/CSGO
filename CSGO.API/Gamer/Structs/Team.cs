using System.Text.Json.Serialization;

namespace CSGO.API.Gamer
{
    public struct Team
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("club_id")]
        public int ClubID { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("abbrevation")]
        public string Abbrevation { get; set; }

        [JsonPropertyName("common_name")]
        public string CommonName { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("skill")]
        public string Skill { get; set; }

        [JsonPropertyName("game_id")]
        public int GameID { get; set; }

        [JsonPropertyName("logo_image_id")]
        public int LogoImageID { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        [JsonPropertyName("nationality")]
        public string Nationality { get; set; }

        [JsonPropertyName("deactivated_at")]
        public DateTime? DeactivatedAt { get; set; }

        [JsonPropertyName("elo")]
        public int Elo { get; set; }

        [JsonPropertyName("url")]
        public Uri URL { get; set; }

        [JsonPropertyName("relative_url")]
        public Uri RelativeURL { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("logo")]
        public Logo Logo { get; set; }

        [JsonPropertyName("club")]
        public Club Club { get; set; }
    }
}
