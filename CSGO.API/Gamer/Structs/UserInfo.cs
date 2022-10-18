using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CSGO.API.Gamer
{
    public struct UserInfo
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("user_name")]
        public string Username { get; set; }

        [JsonPropertyName("common_name")]
        public string CommonName { get; set; }

        [JsonPropertyName("about")]
        public string About { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("nationality")]
        public string Nationality { get; set; }

        [JsonPropertyName("birth_country")]
        public string BirthCountry { get; set; }

        [JsonPropertyName("url")]
        public Uri URL { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("accounts")]
        public List<UserAccount> Accounts { get; set; }
    }
}
