using System.Text.Json.Serialization;

namespace CSGO.API.Gamer
{
    public struct MatchSignup
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("remote_id")]
        public int? RemoteID { get; set; }

        [JsonPropertyName("competition_id")]
        public int CompetitionID { get; set; }

        [JsonPropertyName("team_id")]
        public int TeamID { get; set; }

        [JsonPropertyName("signupable_type")]
        public string SignupableType { get; set; }

        [JsonPropertyName("signupable_id")]
        public int SignupableID { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("signup_time")]
        public DateTime SignupTime { get; set; }

        [JsonPropertyName("signup_user_id")]
        public int SignupUserID { get; set; }

        [JsonPropertyName("checkin_time")]
        public DateTime? CheckinTime { get; set; }

        [JsonPropertyName("checkin_user_id")]
        public int? CheckinUserID { get; set; }

        [JsonPropertyName("retire_time")]
        public DateTime? RetireTime { get; set; }

        [JsonPropertyName("retire_user_id")]
        public int? RetireUserID { get; set; }

        [JsonPropertyName("paid")]
        public string Paid { get; set; }

        [JsonPropertyName("placement")]
        public int Placement { get; set; }

        [JsonPropertyName("seed")]
        public int Seed { get; set; }

        [JsonPropertyName("points")]
        public int Points { get; set; }

        [JsonPropertyName("score_for")]
        public int ScoreFor { get; set; }

        [JsonPropertyName("score_against")]
        public int ScoreAgainst { get; set; }

        [JsonPropertyName("penalty")]
        public int Penalty { get; set; }

        [JsonPropertyName("bonus")]
        public int Bonus { get; set; }

        [JsonPropertyName("wins")]
        public int Wins { get; set; }

        [JsonPropertyName("draws")]
        public int Draws { get; set; }

        [JsonPropertyName("losses")]
        public int Losses { get; set; }

        [JsonPropertyName("played")]
        public int Played { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        [JsonPropertyName("data")]
        public string Data { get; set; }

        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }

        [JsonPropertyName("team")]
        public Team Team { get; set; }
    }
}
