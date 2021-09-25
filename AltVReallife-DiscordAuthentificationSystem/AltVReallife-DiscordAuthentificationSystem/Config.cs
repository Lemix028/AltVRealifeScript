using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AltVReallife_DiscordAuthentificationSystem
{
    public struct Config
    {
        [JsonPropertyName("client_id")]
        public string client_id { get; set; }
        [JsonPropertyName("client_secret")]
        public string client_secret { get; set; }
        [JsonPropertyName("redirect_url")]
        public string redirect_url { get; set; }
        [JsonPropertyName("port")]
        public int port { get; set; }
    }
}
