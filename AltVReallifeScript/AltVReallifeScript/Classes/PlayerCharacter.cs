using AltV.Net.Data;
using AltVReallifeScript.Entitys;
using AltVReallifeScript.Structs;
using AltVReallifeScript.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace AltVReallifeScript.Classes
{
    public class PlayerCharacter
    {
        public uint DBId { get; set; }
        public uint ownerId { get; set; }
        public MyPlayer owner { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public DateTime birthday { get; set; }
        public UInt16 health { get; set; }
        public UInt16 armor { get; set; }
        public ulong cash { get; set; }
        public Enums.Genders gender { get; set; }
        public Position pos { get; set; }
        public BodyAppearence bodyappearence { get; set; }
        public PlayerCharacter()
        {

        }
    }
}
