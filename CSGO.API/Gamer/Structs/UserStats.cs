using System.Text.Json.Serialization;

namespace CSGO.API.Gamer
{
    public struct UserStats
    {
        [JsonPropertyName("paradise_user_id")]
        public int ParadiseUserID { get; set; }

        [JsonPropertyName("player_name")]
        public string PlayerName { get; set; }

        [JsonPropertyName("maps_played")]
        public int MapsPlayed { get; set; }

        [JsonPropertyName("maps_won")]
        public int MapsWon { get; set; }

        [JsonPropertyName("rounds_played")]
        public int RoundsPlayed { get; set; }

        [JsonPropertyName("rounds_won")]
        public int RoundsWon { get; set; }

        [JsonPropertyName("kills")]
        public int Kills { get; set; }

        [JsonPropertyName("kills_per_round")]
        public string KillsPerRound { get; set; }

        [JsonPropertyName("kills_per_map")]
        public string KillsPerMap { get; set; }

        [JsonPropertyName("kd_diff")]
        public int KDDiff { get; set; }

        [JsonPropertyName("kd_ratio")]
        public string KDRatio { get; set; }

        [JsonPropertyName("assists")]
        public int Assists { get; set; }

        [JsonPropertyName("assists_per_round")]
        public string AssistsPerRound { get; set; }

        [JsonPropertyName("deaths")]
        public int Deaths { get; set; }

        [JsonPropertyName("deaths_per_map")]
        public string DeathsPerMap { get; set; }

        [JsonPropertyName("deaths_per_round")]
        public string DeathsPerRound { get; set; }

        [JsonPropertyName("teamkills")]
        public int Teamkills { get; set; }

        [JsonPropertyName("defuses")]
        public int Defuses { get; set; }

        [JsonPropertyName("headshots")]
        public int Headshots { get; set; }

        [JsonPropertyName("headshots_per_map")]
        public string HeadshotsPerMap { get; set; }

        [JsonPropertyName("headshots_ratio")]
        public string HeadshotsRatio { get; set; }

        [JsonPropertyName("rounds_with_1_kill")]
        public int RoundsWith1Kill { get; set; }

        [JsonPropertyName("rounds_with_2_kills")]
        public int RoundsWith2Kill { get; set; }

        [JsonPropertyName("rounds_with_3_kills")]
        public int RoundsWith3Kill { get; set; }

        [JsonPropertyName("rounds_with_4_kills")]
        public int RoundsWith4Kill { get; set; }

        [JsonPropertyName("rounds_with_5_kills")]
        public int RoundsWith5Kill { get; set; }

        [JsonPropertyName("rounds_with_multiple_kills")]
        public int RoundsWithMultipleKills { get; set; }

        [JsonPropertyName("firstkills")]
        public int Firstkills { get; set; }

        [JsonPropertyName("won_1v1")]
        public int Won1v1 { get; set; }

        [JsonPropertyName("won_1v2")]
        public int Won1v2 { get; set; }

        [JsonPropertyName("won_1v3")]
        public int Won1v3 { get; set; }

        [JsonPropertyName("won_1v4")]
        public int Won1v4 { get; set; }

        [JsonPropertyName("won_1v5")]
        public int Won1v5 { get; set; }

        [JsonPropertyName("clutches_won")]
        public int ClutchesWon { get; set; }

        [JsonPropertyName("survival_ratio")]
        public string SurvivalRatio { get; set; }

        [JsonPropertyName("damage_per_round")]
        public string DamagePerRound { get; set; }

        [JsonPropertyName("damage_diff")]
        public int DamageDiff { get; set; }

        [JsonPropertyName("damage_diff_per_round")]
        public string DamageDiffPerRound { get; set; }

        [JsonPropertyName("rating")]
        public string Rating { get; set; }
    }
}
