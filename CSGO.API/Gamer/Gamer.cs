using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CSGO.API.Gamer
{
    public static class Gamer
    {
        const string BASE_URL = "https://www.gamer.no/api/paradise/v2/";

        public static HttpClient GetClient()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer 15|oC3Z4cFaf2Wi4yRzpdQv6O1esvrOMInIzkkS8SuZ");

            return client;
        }

        public static class User
        {
            const string EXT = "user";

            public static async Task<UserInfo> UserInfo(HttpClient client, int userID)
            {
                var response = await client.GetAsync($"{BASE_URL}{EXT}/{userID}");

                if (!response.IsSuccessStatusCode)
                    Console.WriteLine(await response.Content.ReadAsStringAsync());

                return JsonSerializer.Deserialize<UserInfo>(response.Content.ReadAsStream());
            }

            public static async Task<UserStats> UserStats(HttpClient client, int userID, string game)
            {
                var response = await client.GetAsync($"{BASE_URL}{EXT}/{userID}/stats/{game}");

                if (!response.IsSuccessStatusCode)
                    Console.WriteLine(await response.Content.ReadAsStringAsync());

                return JsonSerializer.Deserialize<UserStats>(response.Content.ReadAsStream());
            }
        }

        public static class Matchup
        {
            const string EXT = "matchup";

            public static async Task<Match> GetMatch(HttpClient client, int matchID)
            {
                var response = await client.GetAsync($"{BASE_URL}{EXT}/{matchID}");

                if (!response.IsSuccessStatusCode)
                    Console.WriteLine(await response.Content.ReadAsStringAsync());

                return JsonSerializer.Deserialize<Match>(response.Content.ReadAsStream());
            }
        }
    }
}
