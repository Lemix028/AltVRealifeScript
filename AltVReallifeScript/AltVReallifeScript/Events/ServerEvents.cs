using AltV.Net;
using AltV.Net.Resources.Chat.Api;
using AltVReallifeScript.Database;
using AltVReallifeScript.Entitys;
using AltVReallifeScript.Structs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AltVReallifeScript.Events
{
    public class ServerEvents : IScript
    {
        [ServerEvent("ars:DiscordAuthSuccess")]
        public void OnDiscordAuthSuccess(MyPlayer player, string id, string username, string avatar, string discriminator, string discordauth)
        {
            try
            {
                if (PlayerData.DoesDiscordPlayerExists(id) || PlayerData.DoesSocialClubUserExists(player))
                {
                    player.DiscordId = id;
                    player.Username = username;
                    player.DiscordAvatar = avatar;
                    player.DiscordDiscriminator = discriminator;
                    player.DiscordAuth = JsonConvert.DeserializeObject<DiscordAuthObject>(discordauth);
                    player.LoginDiscord(false);
                    player.Emit("ars:LoginSuccess");
                    return;
                }
                //if (PlayerData.DoesSocialClubUserExists(player))
                //{
                //    player.Emit("ars:log", "Du bist bereits registriert!");
                //    return;
                //}
                player.RegisterDiscord(id, username, avatar, discriminator, JsonConvert.DeserializeObject<DiscordAuthObject>(discordauth));
                player.Emit("ars:LoginSuccess");
            }catch(Exception e)
            {
                Alt.Log(e.ToString());
            }
          
        }
        [ServerEvent("ars:DiscordAuthError")]
        public void OnDiscordAuthError(MyPlayer player, string msg)
        {
            if (player != null)
            {
                player.Emit("ars:log", msg);
                return;
            }
            else
            {
                Alt.Log(msg);
            }
        }
        [ServerEvent("ars:UpdateDiscordAuth")]
        public void OnUpdateDiscordAuth(MyPlayer player, string id, string username, string avatar, string discriminator, string discordauth)
        {
            player.DiscordId = id;
            player.Username = username;
            player.DiscordAvatar = avatar;
            player.DiscordDiscriminator = discriminator;
            player.DiscordAuth = JsonConvert.DeserializeObject<DiscordAuthObject>(discordauth);
            PlayerData.UpdateDiscordData(player);
        }
        [ServerEvent("ars:ClearDiscordAuth")]
        public void OnClearDiscordAuth(MyPlayer player)
        {
            PlayerData.ClearDiscordData(player);
        }

     
    }
}
