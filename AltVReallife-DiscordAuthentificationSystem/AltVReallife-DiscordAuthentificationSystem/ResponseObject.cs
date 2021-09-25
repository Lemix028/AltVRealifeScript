using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AltVReallife_DiscordAuthentificationSystem
{
    public struct ResponseObject
    {
        [JsonPropertyName("access_token")]
        public string access_token { get; set; }
        [JsonPropertyName("token_type")]
        public string token_type { get; set; }
        [JsonPropertyName("expires_in")]
        public int expires_in { get; set; }
        [JsonPropertyName("refresh_token")]
        public string refresh_token { get; set; }
        [JsonPropertyName("scope")]
        public string scope { get; set; }
        [JsonPropertyName("datetime")]
        public DateTime datetime { get; set; }
    }
}
