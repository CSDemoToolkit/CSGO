using System.Text.Json.Serialization;

namespace CSGO.API.Faceit
{
    public struct Match
    {
        [JsonPropertyName("match_id")]
        public Guid MatchID { get; set; }

        [JsonPropertyName("best_of")]
        public int BestOf { get; set; }
    }
}
