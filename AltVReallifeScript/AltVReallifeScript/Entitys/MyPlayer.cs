using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using AltVReallifeScript.Classes;
using AltVReallifeScript.Database;
using AltVReallifeScript.Structs;
using AltVReallifeScript.Utils;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using static AltVReallifeScript.Utils.Enums;

namespace AltVReallifeScript.Entitys
{
    public class MyPlayer : Player
    {
        public uint DBId { get; set; }
        public string Username { get; set; }
        public int AdminLevel { get; set; }
        public string DiscordId { get; set; }
        public string DiscordAvatar { get; set; }
        public string DiscordDiscriminator { get; set; }
        public bool LoggedIn { get; set; } = false;
        public PlayerCharacter Character { get; set; }
        public DiscordAuthObject DiscordAuth { get; set; }

        public MyPlayer(IntPtr nativePointer, ushort id) : base(nativePointer, id)
        {
            Username = "Noname";
            AdminLevel = 0;
            LoggedIn = false;
        }

        public MyPlayer(IntPtr nativePointer, ushort id, string username) : base(nativePointer, id)
        {
            Username = username;
            AdminLevel = 0;
        }

        public void Register(string username, string password)
        {
            Username = username;
            PlayerData.RegisterPlayer(this, password);
            Login(true);
        }

        public void RegisterDiscord(string id, string username, string avatar, string discriminator, DiscordAuthObject dcauth)
        {
            Username = username;
            DiscordId = id;
            DiscordAvatar = avatar;
            DiscordDiscriminator = discriminator;
            DiscordAuth = dcauth;

            PlayerData.RegisterDiscordPlayer(this);
            LoginDiscord(true);
        }
        public void Login(bool isRegister = false)
        {
            if (!isRegister)
                PlayerData.LoadPlayer(this);
            LoggedIn = true;
            this.Emit("ars:log", $"Logged in as {Username}");

            // this.Model = (uint)PedModel.Xmech02SMY;
            //this.Spawn(new Position(0, 0, 75), 10);
            //Character = new PlayerCharacter();
            //Character.ownerId = DBId;
            //Character.firstname = "Lemix";
            //Character.lastname = "dsffdsfsd";
            //Character.birthday = "12-12-2000";
            //Character.cash = 5000;
            //PlayerCharacterData.RegisterNewCharacter(Character);
        }
        public void LoginDiscord(bool isRegister = false)
        {
            PlayerData.UpdateDiscordData(this);
            if (!isRegister)
                PlayerData.LoadPlayer(this);
            LoggedIn = true;
            this.Emit("ars:log", $"Logged in as {Username}#{DiscordDiscriminator}");
            // this.Model = (uint)PedModel.Xmech02SMY;
           // this.Spawn(new Position(0, 0, 75), 10);
            //Character = new PlayerCharacter();
            //Character.ownerId = DBId;
            //Character.firstname = "Lemix";
            //Character.lastname = "2sadsads";
            //Character.birthday = "12-12-2000";
            //Character.cash = 5000;
            //PlayerCharacterData.RegisterNewCharacter(Character);
        }

        public void Disconnect()
        {
            Save();
        }

        public void Save()
        {
            
            if(IsLoggedInAsDiscordPlayer())
            {
                PlayerData.UpdateDiscordData(this);
                PlayerData.UpdatePlayer(this);
                if(this.Character != null)
                {
                    Character.pos = this.Position;
                    Character.health = this.Health;
                    Character.armor = this.Armor;
                    PlayerCharacterData.UpdateCharacter(Character);
                }
                    

            }
            else
            {
                PlayerData.UpdatePlayer(this);
                if (this.Character != null)
                {
                    Character.pos = this.Position;
                    Character.health = this.Health;
                    Character.armor = this.Armor;
                    PlayerCharacterData.UpdateCharacter(Character);
                }
            }
           
               

            Alt.Log($"{Variables.Prefix} PlayerData from {Username} was saved!");
        }

        public static MyPlayer GetPlayerByName(string username)
        {
            foreach(MyPlayer entry in Alt.GetAllPlayers())
            {
                if(entry.Username == username)
                    return entry;
            }
            return null;
        }

        public bool IsPlayerLoggedIn()
        {
            return LoggedIn;
        }

        public bool HasAdminLevel(AdminRanks ar)
        {
            return AdminLevel >= (int)ar;
        }

        public bool IsLoggedInAsDiscordPlayer()
        {
            if (LoggedIn == false)
                return false;
            if(DiscordId == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public string GetHash()
        {
            return PasswordDerivation.GetSha256Hash(SHA256.Create(), this.HardwareIdHash + ":" + this.HardwareIdExHash.ToString());
        }

    }
}
