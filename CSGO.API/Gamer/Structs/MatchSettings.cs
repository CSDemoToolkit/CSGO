using System.Text.Json.Serialization;

namespace CSGO.API.Gamer
{
    public struct MatchSettings
    {
        [JsonPropertyName("veto_opens_at")]
        public DateTime VetoOpensAt { get; set; }

        [JsonPropertyName("walkover_available_at")]
        public DateTime WalkoverAvailableAt { get; set; }

        [JsonPropertyName("best_of")]
        public int BestOf { get; set; }

        [JsonPropertyName("matchup_start_time")]
        public DateTime MatchupStartTime { get; set; }

        [JsonPropertyName("team_size")]
        public int TeamSize { get; set; }

        [JsonPropertyName("squad_max_size")]
        public int SquadMaxSize { get; set; }

        [JsonPropertyName("require_anti_cheat")]
        public bool RequireAntiCheat { get; set; }

        [JsonPropertyName("debug")]
        public bool Debug { get; set; }

        [JsonPropertyName("require_checkin")]
        public bool RequireCheckin { get; set; }

        [JsonPropertyName("veto_opens_at_preset")]
        public string VetoOpensAtPreset { get; set; }

        [JsonPropertyName("walkover_available_at_preset")]
        public string WalkoverAvailableAtPreset { get; set; }

        [JsonPropertyName("admins_can_play")]
        public bool AdminsCanPlay { get; set; }

        [JsonPropertyName("allow_scheduling")]
        public bool AllowScheduling { get; set; }

        [JsonPropertyName("aggregate_map_scores")]
        public bool AggregateMapScores { get; set; }

        [JsonPropertyName("use_live_table")]
        public bool UseLiveTable { get; set; }

        [JsonPropertyName("play_all_maps")]
        public bool PlayAllMaps { get; set; }

        [JsonPropertyName("starts_automatically")]
        public bool StartsAutomatically { get; set; }

        [JsonPropertyName("signup_opens_at_preset")]
        public string SignupOpensAtPreset { get; set; }

        [JsonPropertyName("signup_opens_at_amount")]
        public int SignupOpensAtAmount { get; set; }

        [JsonPropertyName("signup_opens_at_unit")]
        public string SignupOpensAtUnit { get; set; }

        [JsonPropertyName("signup_deadline_unit")]
        public string SignupDeadlineUnit { get; set; }

        [JsonPropertyName("checkin_opens_at_preset")]
        public string CheckinOpensAtPreset { get; set; }

        [JsonPropertyName("checkin_opens_at_amount")]
        public int CheckinOpensAtAmount { get; set; }

        [JsonPropertyName("checkin_opens_at_unit")]
        public string CheckinOpensAtUnit { get; set; }

        [JsonPropertyName("checkin_deadline_preset")]
        public string CheckinDeadlinePreset { get; set; }

        [JsonPropertyName("checkin_deadline_amount")]
        public int CheckinDeadlineAmount { get; set; }

        [JsonPropertyName("checkin_deadline_unit")]
        public string CheckinDeadlineUnit { get; set; }

        [JsonPropertyName("template")]
        public string Template { get; set; }

        [JsonPropertyName("round_robin_count")]
        public int RoundRobinCount { get; set; }

        [JsonPropertyName("checkin_deadline")]
        public DateTime CheckinDeadline { get; set; }

        [JsonPropertyName("private")]
        public bool Private { get; set; }

        [JsonPropertyName("max_teams")]
        public int MaxTeams { get; set; }

        [JsonPropertyName("enable_spectating")]
        public bool EnableSpectating { get; set; }

        [JsonPropertyName("signp_deadline_amount")]
        public int SignupDeadlineAmount { get; set; }

        [JsonPropertyName("signp_deadline_preset")]
        public string SignupDeadlinePreset { get; set; }

        [JsonPropertyName("signp_deadline")]
        public DateTime SignpDeadline { get; set; }

        [JsonPropertyName("enable_demo_upload")]
        public bool EnableDemoUpload { get; set; }

        [JsonPropertyName("enable_screenshot_upload")]
        public bool EnableScreenshotUpload { get; set; }

        [JsonPropertyName("enable_walkover_request")]
        public bool EnableWalkoverRequest { get; set; }

        [JsonPropertyName("enable_clinching")]
        public bool EnableClinching { get; set; }

        [JsonPropertyName("enable_overtime")]
        public bool EnableOvertime { get; set; }
    }
}
