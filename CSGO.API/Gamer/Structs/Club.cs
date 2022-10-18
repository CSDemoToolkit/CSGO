using System.Text.Json.Serialization;

namespace CSGO.API.Gamer
{
    public struct Club
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("abbrevation")]
        public string Abbrevation { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("irc")]
        public string IRC { get; set; }

        [JsonPropertyName("email")]
        public string EMail { get; set; }

        [JsonPropertyName("homepage")]
        public string Homepage { get; set; }

        [JsonPropertyName("facebook")]
        public string Facebook { get; set; }

        [JsonPropertyName("twitter")]
        public string Twitter { get; set; }

        [JsonPropertyName("linkedin")]
        public string Linkedin { get; set; }

        [JsonPropertyName("youtube")]
        public string Youtube { get; set; }

        [JsonPropertyName("logo_image_id")]
        public int LogoImageID { get; set; }

        [JsonPropertyName("poster_image_id")]
        public int PosterImageID { get; set; }

        [JsonPropertyName("created_by_user_id")]
        public int CreatedByUserID { get; set; }

        [JsonPropertyName("owned_by_user_id")]
        public int OwnedByUserID { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        [JsonPropertyName("deactivated_at")]
        public DateTime? DeactivatedAt { get; set; }

        [JsonPropertyName("var_number")]
        public int VarNumber { get; set; }

        [JsonPropertyName("billing_info")]
        public int? BillingInfo { get; set; }

        [JsonPropertyName("url")]
        public Uri URL { get; set; }

        [JsonPropertyName("relative_url")]
        public Uri RelativeURL { get; set; }
    }
}
