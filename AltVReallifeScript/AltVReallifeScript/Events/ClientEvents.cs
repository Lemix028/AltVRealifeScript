using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Async;
using AltV.Net.Enums;
using AltVReallifeScript.Classes;
using AltVReallifeScript.Database;
using AltVReallifeScript.Entitys;
using AltVReallifeScript.Structs;
using AltVReallifeScript.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AltVReallifeScript.Events
{
    class ClientEvents : IScript
    {

        [ClientEvent("ars:OnLoginSubmit")]
        public void OnLoginSubmit(MyPlayer player, string username, string password)
        {
            //Prüfung für login richtig und sooo einen kram
            if (player.LoggedIn)
                return;
            if (!PlayerData.CheckPassword(username, password))
            {
                if (player.HasData("LoginCount"))
                {
                    player.GetData<int>("LoginCount", out int logincount);
                    Alt.Log(logincount.ToString());
                    if (logincount >= 3)
                    {
                        player.Kick("You have tried to log in too many times!");
                        return;
                    }
                    player.SetData("LoginCount", logincount + 1);
                }

                player.Emit("ars:LoginErrorMessage", "Benutzername und/oder Passwort falsch!");
                return;
            }
            player.Username = username;
            player.Login();

            player.Emit("ars:LoginSuccess");
            

        }

        //If Home Webview is ready
        [ClientEvent("ars:HomeReady")]
        public void OnHomeReady(MyPlayer player)
        {
            player.Emit("ars:SetCharacterData", JsonConvert.SerializeObject(PlayerCharacterData.GetAllCharactersFromPlayer(player)));
        }

        [ClientEvent("ars:OnRegisterSubmit")]
        public void OnRegisterSubmit(MyPlayer player, string username, string password)
        {
            //Prüfung für register richtig und sooo einen kram
            if (player.LoggedIn)
                return;
            if (PlayerData.DoesSocialClubUserExists(player))
            {
                player.Emit("ars:RegisterErrorMessage", "Du bist bereits registriert!");
                if (player.HasData("RegisterCount"))
                {
                    player.GetData<int>("RegisterCount", out int registercount);
                    if (registercount >= 5)
                    {
                        player.Kick("You have tried to register too many times!");
                        return;
                    }
                    player.SetData("RegisterCount", registercount + 1);
                }
                return;
            }
            if (PlayerData.DoesPlayerUsernameExists(username))
            {
                player.Emit("ars:RegisterErrorMessage", "Dieser Benutzername existiert bereits!");
                if (player.HasData("RegisterCount"))
                {
                    player.GetData<int>("RegisterCount", out int registercount);
                    if (registercount >= 5)
                    {
                        player.Kick("You have tried to register too many times!");
                        return;
                    }
                    player.SetData("RegisterCount", registercount + 1);
                }
                return;
            }
            player.Register(username, password);
            
            player.Emit("ars:LoginSuccess");
        }
        [ClientEvent("ars:OnDiscordSubmit")]
        public void OnGenerateDiscordLink(MyPlayer player)
        {
            PlayerData.LoadPlayerSocialClub(player);
            if (player.DiscordId == null || player.DiscordId == "")
            {
                // new register
                string hash = player.GetHash();
                string url = $@"https://discord.com/oauth2/authorize?client_id={Variables.client_id}&redirect_uri={Variables.redirect_url}&response_type=code&scope=identify&state={hash}";
                player.Emit("ars:SetNewPage", url);
                return;
            }
            else
            {
                Alt.Emit("ars:AccessTokenLogin", player, JsonConvert.SerializeObject(player.DiscordAuth));
            }

        }

        [ClientEvent("ars:PlayCharacter")]
        public void OnPlayCharacter(MyPlayer player, int charid)
        {
            PlayerCharacter character = new PlayerCharacter();
            PlayerCharacterData.LoadCharacter(character, charid);
            player.Character = character;
            player.Spawn(player.Character.pos, 10);
            Alt.Emit("character:Sync", player, JsonConvert.SerializeObject(character.bodyappearence));
            player.Emit("ars:RemoveWebview");
            player.Emit("ars:notify", $"Welcome {character.firstname}");
            
        }
        [ClientEvent("ars:DeleteCharacter")]
        public void OnDeleteCharacter(MyPlayer player, int charid)
        {
            PlayerCharacterData.DeleteCharacter(charid);
            player.Emit("ars:SetCharacterData", JsonConvert.SerializeObject(PlayerCharacterData.GetAllCharactersFromPlayer(player)));
        }
        [ClientEvent("ars:RegisterNewChar")]
        public void OnRegisterNewChar(MyPlayer player, string firstname, string lastname, string birthday, string gender)
        {

            PlayerCharacter character = new PlayerCharacter();
            character.firstname = firstname;
            character.lastname = lastname;
            character.cash = Variables.StartCash;
            character.gender = (Enums.Genders)int.Parse(gender);
            Alt.Log(((int)character.gender).ToString());
            character.birthday = DateTime.ParseExact(birthday, "MMMM d, yyyy", System.Globalization.CultureInfo.InvariantCulture);
            character.owner = player;
            character.ownerId = player.DBId;
            character.pos = Variables.SpawnLocation;
            player.SetData("ars:TempCharacterData", character);
            player.Emit("ars:OpenNewCharEditor");
            player.Emit("character:Edit");
            
            //PlayerCharacterData.RegisterNewCharacter(character);
            //player.Emit("ars:OpenHomeMenu");
        }

        [ClientEvent("character:Done")]
        public void OnCharDone(MyPlayer player, object data)
        {
            if(data == null)
            {
                if (player.HasData("ars:TempCharacterData"))
                {
                    player.SetData("ars:TempCharacterData", null);
                    player.Emit("ars:OpenHomeMenu");
                }
            }
            else
            {
                if (player.HasData("ars:TempCharacterData"))
                {
                    Alt.Emit("character:Sync", player, data);
                    player.GetData<PlayerCharacter>("ars:TempCharacterData", out PlayerCharacter character);
                    BodyAppearence ba = JsonConvert.DeserializeObject<BodyAppearence>(JsonConvert.SerializeObject(data));
                    character.bodyappearence = ba;
                    PlayerCharacterData.RegisterNewCharacter(character);
                    player.Emit("ars:OpenHomeMenu");
                    player.SetData("ars:TempCharacterData", null);
                }
            }
           
            

        }

        [ClientEvent("character:LCancel")]
        public void OnCharCancel(MyPlayer player, string oldData)
        {
            if (player.HasData("ars:TempCharacterData"))
            {
                player.SetData("ars:TempCharacterData", null);
                player.Emit("ars:OpenHomeMenu");
            }
            // player.Emit("character:Sync", player, data);
            // player.Position = player.Position;
        }

        [ClientEvent("ars:ToggleVehicleEngine")]
        public void OnToggleVehicleEngine(MyPlayer player)
        {
            MyVehicle veh = (MyVehicle)player.Vehicle;
            veh.ToggleEngine();
        }

        [ClientEvent("ars:SetPos")]
        public async void OnSetPos(MyPlayer player, string x, string y, string z)
        {

            if (await player.IsInVehicleAsync())
            {
                await player.Vehicle.SetPositionAsync(new Position(float.Parse(x), float.Parse(y), float.Parse(z)));
            }
            else
            {
                await player.SetPositionAsync(new Position(float.Parse(x), float.Parse(y), float.Parse(z)));
            }

        }
        [ClientEvent("ars:SetRot")]
        public async void OnSetRot(MyPlayer player, string x, string y, string z)
        {
            if (await player.IsInVehicleAsync())
            {

                await player.Vehicle.SetRotationAsync(new Rotation(float.Parse(x), float.Parse(y), float.Parse(z)));
            }
            else
            {
                await player.SetRotationAsync(new Rotation(float.Parse(x), float.Parse(y), float.Parse(z)));
            }
        }

        [ClientEvent("ars:OpenBarberMenu")]
        public async void OnOpenBarberMenu(MyPlayer player)
        {
            await player.EmitAsync("ars:CreateBarberMenuWithData", JsonConvert.SerializeObject(player.Character.bodyappearence));          
        }

    }
}
