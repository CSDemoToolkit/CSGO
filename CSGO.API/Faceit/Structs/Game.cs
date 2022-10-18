using System.Text.Json.Serialization;

namespace CSGO.API.Faceit
{
    public struct Game
    {
        [JsonPropertyName("region")]
        public string Region { get; set; }

        [JsonPropertyName("game_player_id")]
        public string GamePlayerId { get; set; }

        [JsonPropertyName("skill_level")]
        public int SkillLevel { get; set; }

        [JsonPropertyName("faceit_elo")]
        public int FaceitElo { get; set; }

        [JsonPropertyName("game_player_name")]
        public string GamePlayerName { get; set; }

        [JsonPropertyName("skill_level_label")]
        public string SkillLevelLabel { get; set; }

        [JsonPropertyName("game_profile_id")]
        public string GameProfileId { get; set; }
    }
}
