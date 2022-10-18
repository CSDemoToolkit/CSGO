using System.Text.Json.Serialization;

namespace CSGO.API.Gamer
{
    public struct Match
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("start_time")]
        public DateTime StartTime { get; set; }

        [JsonPropertyName("best_of")]
        public int BestOf { get; set; }

        [JsonPropertyName("competition_id")]
        public int CompetitionID { get; set; }

        [JsonPropertyName("matchupable_type")]
        public string MatchupableType { get; set; }

        [JsonPropertyName("matchupable_id")]
        public int MatchupableID { get; set; }

        [JsonPropertyName("home_score")]
        public int HomeScore { get; set; }

        [JsonPropertyName("away_score")]
        public int AwayScore { get; set; }

        [JsonPropertyName("walkover")]
        public bool Walkover { get; set; }

        [JsonPropertyName("summary")]
        public string Summary { get; set; }

        [JsonPropertyName("score_submit_user_id")]
        public int? ScoreSubmitUserID { get; set; }

        [JsonPropertyName("score_submit_team_id")]
        public int? ScoreSubmitTeamID { get; set; }

        [JsonPropertyName("forum_thread_id")]
        public int? ForumThreadID { get; set; }

        [JsonPropertyName("round_number")]
        public int RoundNumber { get; set; }

        [JsonPropertyName("round_identifier")]
        public string RoundIdentifier { get; set; }

        [JsonPropertyName("score_submit_time")]
        public DateTime? ScoreSubmitTime { get; set; }

        [JsonPropertyName("score_approve_user_id")]
        public int? ScoreApproveUserID { get; set; }

        [JsonPropertyName("score_approve_team_id")]
        public int? ScoreApproveTeamID { get; set; }

        [JsonPropertyName("ticket_id")]
        public int? TicketID { get; set; }

        [JsonPropertyName("bracket")]
        public string Bracket { get; set; }

        [JsonPropertyName("cancelled")]
        public bool Cancelled { get; set; }

        [JsonPropertyName("image_id")]
        public int? ImageID { get; set; }

        [JsonPropertyName("home_signup_id")]
        public int HomeSignupID { get; set; }

        [JsonPropertyName("away_signup_id")]
        public int AwaySignupID { get; set; }

        [JsonPropertyName("home_originating_matchup_id")]
        public int? HomeOriginatingMatchupID { get; set; }

        [JsonPropertyName("away_originating_matchup_id")]
        public int? AwayOriginatingMatchupID { get; set; }

        [JsonPropertyName("number")]
        public int Number { get; set; }

        [JsonPropertyName("finished_at")]
        public DateTime FinishedAt { get; set; }

        [JsonPropertyName("ready_at")]
        public DateTime ReadyAt { get; set; }

        [JsonPropertyName("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("round_id")]
        public int RoundID { get; set; }

        [JsonPropertyName("postponed")]
        public bool Postponed { get; set; }

        [JsonPropertyName("uri")]
        public Uri URI { get; set; }

        [JsonPropertyName("winning_side")]
        public string WinningSide { get; set; }

        [JsonPropertyName("round_identifier_text")]
        public string RoundIdentifierText { get; set; }

        [JsonPropertyName("url")]
        public Uri URL { get; set; }

        [JsonPropertyName("relative_url")]
        public Uri RelativeURL { get; set; }

        [JsonPropertyName("settings")]
        public MatchSettings Settings { get; set; }

        [JsonPropertyName("matchuptable")]
        public MatchupTable MatchupTable { get; set; }

        [JsonPropertyName("competition")]
        public Competition Competition { get; set; }

        [JsonPropertyName("home_signup")]
        public MatchSignup HomeSignup { get; set; }

        [JsonPropertyName("away_signup")]
        public MatchSignup AwaySignup { get; set; }

        [JsonPropertyName("matchup_users")]
        public List<MatchupUser> MatchupUsers { get; set; }

        [JsonPropertyName("matchupmaps")]
        public List<MatchupUser> MatchupMaps { get; set; }

        [JsonPropertyName("round")]
        public MatchRound Round { get; set; }
    }
}
