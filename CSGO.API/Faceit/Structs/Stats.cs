using System.Text.Json.Serialization;

namespace CSGO.API.Faceit
{
    public struct Stats
    {
        [JsonPropertyName("Assists")]
        public string Assists { get; set; }

        [JsonPropertyName("Average Deaths")]
        public string AvgDeaths { get; set; }

        [JsonPropertyName("Average Headshots %")]
        public string AvgHeadshot { get; set; }

        [JsonPropertyName("Average K/D Ratio")]
        public string AvgKD { get; set; }

        [JsonPropertyName("Average K/R Ratio")]
        public string AvgKR { get; set; }

        [JsonPropertyName("Average Kills")]
        public string AvgKills { get; set; }

        [JsonPropertyName("Average MVPs")]
        public string AvgMVP { get; set; }

        [JsonPropertyName("Average Penta Kills")]
        public string AvgPentaKills { get; set; }

        [JsonPropertyName("Average Quadro Kills")]
        public string AvgQuadroKills { get; set; }

        [JsonPropertyName("Average Triple Kills")]
        public string AvgTripleKills { get; set; }

        [JsonPropertyName("Deaths")]
        public string Deaths { get; set; }

        [JsonPropertyName("Headshots")]
        public string Headshots { get; set; }

        [JsonPropertyName("Headshots per Match")]
        public string HeadshotsPerMatch { get; set; }

        [JsonPropertyName("K/D Ratio")]
        public string KD { get; set; }

        [JsonPropertyName("K/R Ratio")]
        public string KR { get; set; }

        [JsonPropertyName("Kills")]
        public string Kills { get; set; }

        [JsonPropertyName("Matches")]
        public string Matches { get; set; }

        [JsonPropertyName("MVPs")]
        public string MVP { get; set; }

        [JsonPropertyName("Penta Kills")]
        public string PentaKills { get; set; }

        [JsonPropertyName("Quadro Kills")]
        public string QuadroKills { get; set; }

        [JsonPropertyName("Rounds")]
        public string Rounds { get; set; }

        [JsonPropertyName("Total Headshots %")]
        public string HS { get; set; }

        [JsonPropertyName("Triple Kills")]
        public string TripleKills { get; set; }

        [JsonPropertyName("Win Rate %")]
        public string WinRate { get; set; }

        [JsonPropertyName("Wins")]
        public string Wins { get; set; }
    }
}
