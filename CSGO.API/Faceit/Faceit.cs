using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CSGO.API.Faceit
{
    public static class Faceit
    {
        const string BASE_URL = "https://open.faceit.com/data/v4/";

        public static HttpClient GetClient()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer e828a3b5-1fc9-41ca-8912-97ea6c0ccc98");

            return client;
        }

        public static class Players
        {
            const string EXT = "players";

            public static async Task<PlayerInfo> FindPlayer(HttpClient client, string nickname)
            {
                var response = await client.GetAsync($"{BASE_URL}{EXT}?nickname={nickname}");

                if (!response.IsSuccessStatusCode)
                    Console.WriteLine(await response.Content.ReadAsStringAsync());

                return JsonSerializer.Deserialize<PlayerInfo>(response.Content.ReadAsStream());
            }

            public static async Task<PlayerInfo> PlayerDetails(HttpClient client, Guid playerID)
            {
                var response = await client.GetAsync($"{BASE_URL}{EXT}/{playerID}");

                if (!response.IsSuccessStatusCode)
                    Console.WriteLine(await response.Content.ReadAsStringAsync());

                return JsonSerializer.Deserialize<PlayerInfo>(response.Content.ReadAsStream());
            }

            public static async Task<PlayerStats> PlayerStats(HttpClient client, Guid playerID, string gameID)
            {
                var response = await client.GetAsync($"{BASE_URL}{EXT}/{playerID}/stats/{gameID}");

                if (!response.IsSuccessStatusCode)
                    Console.WriteLine(await response.Content.ReadAsStringAsync());

                return JsonSerializer.Deserialize<PlayerStats>(response.Content.ReadAsStream());
            }
        }
    }
}
