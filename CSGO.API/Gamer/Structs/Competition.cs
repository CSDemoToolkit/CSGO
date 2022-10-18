using System.Text.Json.Serialization;

namespace CSGO.API.Gamer
{
    public struct Competition
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("start_time")]
        public DateTime StartTime { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("description_html")]
        public string DescriptionHtml { get; set; }

        [JsonPropertyName("poster_image_id")]
        public int PosterImageID { get; set; }

        [JsonPropertyName("logo_image_id")]
        public int? LogoImageID { get; set; }

        [JsonPropertyName("banner_image_id")]
        public int BannerImageID { get; set; }

        [JsonPropertyName("ruleset_id")]
        public int RulesetID { get; set; }

        [JsonPropertyName("created_by_user_id")]
        public int CreatedByUserID { get; set; }

        [JsonPropertyName("type_id")]
        public int TypeID { get; set; }

        [JsonPropertyName("game_id")]
        public int GameID { get; set; }

        [JsonPropertyName("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("checkin_status")]
        public string CheckinStatus { get; set; }

        [JsonPropertyName("organizer_id")]
        public int OrganizerID { get; set; }

        [JsonPropertyName("hub_id")]
        public int HubID { get; set; }

        [JsonPropertyName("promoted")]
        public bool Promoted { get; set; }

        [JsonPropertyName("url")]
        public Uri URL { get; set; }

        [JsonPropertyName("relative_url")]
        public Uri RelativeURL { get; set; }

        [JsonPropertyName("template_text")]
        public string TemplateText { get; set; }

        [JsonPropertyName("team_size_text")]
        public string TeamSizeText { get; set; }

        [JsonPropertyName("has_divisions")]
        public bool HasDivisions { get; set; }
    }
}
