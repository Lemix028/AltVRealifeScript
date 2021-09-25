using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AltVReallifeScript.Structs
{
    public struct BodyAppearence
    {
        [JsonProperty("sex")]
        public int Sex { get; set; }
        [JsonProperty("faceFather")]
        public int faceFather { get; set; }
        [JsonProperty("faceMother")]
        public int faceMother { get; set; }
        [JsonProperty("skinFather")]
        public int skinFather { get; set; }
        [JsonProperty("skinMother")]
        public int skinMother { get; set; }
        [JsonProperty("faceMix")]
        public float faceMix { get; set; }
        [JsonProperty("skinMix")]
        public float skinMix { get; set; }
        [JsonProperty("structure")]
        public float[] structure { get; set; } // 20 Floats
        [JsonProperty("hair")]
        public int hair { get; set; }
        [JsonProperty("hairColor1")]
        public int hairColor1 { get; set; }
        [JsonProperty("hairColor2")]
        public int hairColor2 { get; set; }
        [JsonProperty("hairOverlay")]
        public SHairOverlay hairOverlay { get; set; }
        [JsonProperty("facialHair")]
        public int facialHair { get; set; }
        [JsonProperty("facialHairColor1")]
        public int facialHairColor1 { get; set; }
        [JsonProperty("facialHairOpacity")]
        public int facialHairOpacity { get; set; }
        [JsonProperty("eyebrows")]
        public int eyebrows { get; set; }
        [JsonProperty("eyebrowsOpacity")]
        public int eyebrowsOpacity { get; set; }
        [JsonProperty("eyebrowsColor1")]
        public int eyebrowsColor1 { get; set; }
        [JsonProperty("eyes")]
        public int eyes { get; set; }
        [JsonProperty("opacityOverlays")]
        public SOpacityOverlays[] opacityOverlays { get; set; }
        [JsonProperty("colorOverlays")]
        public SColorOverlays[] colorOverlays { get; set; }

        //[JsonProperty("colorOverlays")]
        //public ColorOverlays colorOverlays { get; set;}
    }

    public struct SHairOverlay // 1 Item
    {
        [JsonProperty("collection")]
        public string collection { get; set; }
        [JsonProperty("overlay")]
        public string overlay { get; set; }
    }
    public struct SOpacityOverlays // 6 Items
    {
        [JsonProperty("id")]
        public int id { get; set; }
        [JsonProperty("opacity")]
        public float opacity { get; set; }
        [JsonProperty("value")]
        public int value { get; set; }
    }
    public struct SColorOverlays // 3 items
    {
        [JsonProperty("id")]
        public int id { get; set; }
        [JsonProperty("opacity")]
        public float opacity { get; set; }
        [JsonProperty("color1")]
        public int color1 { get; set; }
        [JsonProperty("color2")]
        public int color2 { get; set; } // Only the first item have this
        [JsonProperty("value")]
        public int value { get; set; }
    }
    //public struct ColorOverlays
}
