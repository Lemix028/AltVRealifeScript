using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AltVReallife_DiscordAuthentificationSystem
{
    public struct DiscordUserData
    {
        [JsonPropertyName("id")]
        public string id { get; set; }
        [JsonPropertyName("username")]
        public string username { get; set; }
        [JsonPropertyName("avatar")] //https://discord.com/developers/docs/reference#image-data
        public string avatar { get; set; }
        [JsonPropertyName("discriminator")]
        public string discriminator { get; set; }
        [JsonPropertyName("public_flags")]
        public int public_flags { get; set; }
        [JsonPropertyName("flags")]
        public int flags { get; set; }
        [JsonPropertyName("email")]
        public string email { get; set; }
        [JsonPropertyName("verified")]
        public bool verified { get; set; }
        [JsonPropertyName("locale")]
        public string locale { get; set; } // ex: de
        [JsonPropertyName("mfa_enabled")]
        public bool mfa_enabled { get; set; }
    }
}
