using System.Text.Json.Serialization;

namespace CSGO.API.Gamer
{
    public struct UserAccount
    {
        [JsonPropertyName("account_id")]
        public string AccountID { get; set; }

        [JsonPropertyName("nickname")]
        public string Nickname { get; set; }

        [JsonPropertyName("region")]
        public string Region { get; set; }

        [JsonPropertyName("provider")]
        public string Provider { get; set; }
    }
}
