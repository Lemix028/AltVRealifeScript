using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using AltVReallifeScript.Classes;
using AltVReallifeScript.Entitys;
using AltVReallifeScript.Utils;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace AltVReallifeScript
{
    class PlayerEvents : IScript
    {

        [ScriptEvent(ScriptEventType.PlayerConnect)]
        public void OnPlayerConnect(MyPlayer player, string reason)
        {
            player.SetData("LoginCount", 0);
            player.SetData("RegisterCount", 0);
            player.Emit("ars:ConfigFlags");
            player.Emit("ars:createlogin");
            player.Spawn(new AltV.Net.Data.Position(0, 0, 0), 5);
            Variables.channel.AddPlayer(player);
            player.Model = (uint)PedModel.FreemodeMale01;
            //player.Emit("ars:log", "Version: 1");
            Colshapes.LoadColshapes();
            Colshapes.DrawMarkes(player);
        }

        [ScriptEvent(ScriptEventType.PlayerDisconnect)]
        public void OnPlayerDisconnect(MyPlayer player, string reason)
        {
            player.Disconnect();
            Variables.channel.RemovePlayer(player);
        }

        [ScriptEvent(ScriptEventType.PlayerEnterVehicle)]
        public void OnPlayerEnterVehicle(MyVehicle vehicle, IPlayer player, byte seat)
        {
         //   vehicle.SecondaryColorRgb = new AltV.Net.Data.Rgba(255, 0, 0, 255);
          //  SendNotification(player, "Fahrzeug betreten");
        }


        [ScriptEvent(ScriptEventType.PlayerLeaveVehicle)]
        public void OnPlayerLeaveVehicle(MyVehicle vehicle, IPlayer player, byte seat)
        {
          //  vehicle.SecondaryColorRgb = new AltV.Net.Data.Rgba(255, 255, 255, 255);
           // SendNotification(player, "Fahrzeug verlassen");
        }

        internal static void SendNotification(IPlayer networkOwner, string msg)
        {
            networkOwner.Emit("ars:notify", msg);
        }

        public static void SendNotification(MyPlayer player, string msg)
        {
            player.Emit("ars:notify", msg);
        }
    }
}
