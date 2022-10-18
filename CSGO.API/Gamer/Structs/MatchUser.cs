using System.Text.Json.Serialization;

namespace CSGO.API.Gamer
{
    public struct MatchUser
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("user_name")]
        public string Username { get; set; }

        [JsonPropertyName("team_id")]
        public int TeamID { get; set; }

        [JsonPropertyName("image_id")]
        public int ImageID { get; set; }

        [JsonPropertyName("poster_image_id")]
        public int PosterImageID { get; set; }

        [JsonPropertyName("about")]
        public string About { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("nationality")]
        public string Nationality { get; set; }

        [JsonPropertyName("birth_country")]
        public string BirthCountry { get; set; }

        [JsonPropertyName("deactivated_at")]
        public DateTime? DeactivatedAt { get; set; }

        [JsonPropertyName("anonymized_at")]
        public DateTime? AnonymizedAt { get; set; }

        [JsonPropertyName("common_name")]
        public string CommonName { get; set; }

        [JsonPropertyName("url")]
        public Uri URL { get; set; }

        [JsonPropertyName("relative_url")]
        public Uri RelativeURL { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }
    }
}
