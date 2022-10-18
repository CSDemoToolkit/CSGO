using System.Text.Json.Serialization;

namespace CSGO.API.Gamer
{
    public struct MatchupMap
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("game_resource_id")]
        public int GameResourceID { get; set; }

        [JsonPropertyName("matchup_id")]
        public int MatchupID { get; set; }

        [JsonPropertyName("map_number")]
        public int MapNumber { get; set; }

        [JsonPropertyName("home_score")]
        public int HomeScore { get; set; }

        [JsonPropertyName("away_score")]
        public int AwayScore { get; set; }

        [JsonPropertyName("deleted_at")]
        public DateTime DeletedAt { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("finished_at")]
        public DateTime FinishedAt { get; set; }

        [JsonPropertyName("picks_side")]
        public string PicksSide { get; set; }

        [JsonPropertyName("home_side_game_resource_id")]
        public int HomeSideGameResourceID { get; set; }

        [JsonPropertyName("away_side_game_resource_id")]
        public int AwaySideGameResourceID { get; set; }

        [JsonPropertyName("resource")]
        public MatchupResource Resource { get; set; }

        [JsonPropertyName("homeside")]
        public MatchupResource Homeside { get; set; }

        [JsonPropertyName("awayside")]
        public MatchupResource Awayside { get; set; }
    }
}
