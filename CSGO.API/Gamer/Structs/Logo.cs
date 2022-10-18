using System.Text.Json.Serialization;

namespace CSGO.API.Gamer
{
    public struct Logo
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("ration")]
        public float Ratio { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("credits")]
        public string Credits { get; set; }

        [JsonPropertyName("mimetype")]
        public string MimeType { get; set; }

        [JsonPropertyName("url")]
        public Uri URL { get; set; }

        [JsonPropertyName("relative_url")]
        public Uri RelativeURL { get; set; }

        [JsonPropertyName("aspect_string")]
        public string AspectString { get; set; }
    }
}
