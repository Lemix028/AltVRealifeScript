using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using AltV.Net.Resources.Chat.Api;
using AltVReallifeScript.Entitys;
using AltVReallifeScript.Utils;
using System;
using static AltVReallifeScript.Utils.Enums;

namespace AltVReallifeScript
{
    class ChatHandler : IScript
    {

        [ClientEvent("chat:message")]
        public void OnChatMessage(IPlayer player, string msg)
        {
            if (msg.Length == 0 || msg[0] == '/')
                return;
            foreach (IPlayer target in Alt.GetAllPlayers())
            {
                if (target.Position.Distance(player.Position) <= 10)
                {
                    player.SendChatMessage($"{player.Name} sagt: {msg}");
                }
            }
        }

        [CommandEvent(CommandEventType.CommandNotFound)]
        public void OnCommandNotFound(IPlayer player, string cmd)
        {
            player.SendChatMessage("{ff0000}[Server] {ffffff}Befehl konnte nicht gefunden werden!");
        }

        [Command("veh")]
        public static void CMD_CreateVehicle(IPlayer player, string VehicleName, int r = 0, int g = 0, int b = 0)
        {
            uint vehHash = Alt.Hash(VehicleName);
            if (!Enum.IsDefined(typeof(VehicleModel), vehHash))
            {
                player.SendChatMessage("{ff0000}[Server] {ffffff}Ungültiger Fahrzeugname!");
                return;
            }

            //IVehicle veh = Alt.CreateVehicle(vehHash, GetRandomPositionAround(player.Position, 5.0f), player.Rotation);
            MyVehicle veh = new MyVehicle(vehHash, GetRandomPositionAround(player.Position, 5.0f), player.Rotation);
            veh.PrimaryColorRgb = new Rgba(Convert.ToByte(r), Convert.ToByte(g), Convert.ToByte(b), 255);
            veh.SecondaryColorRgb = new Rgba(Convert.ToByte(r), Convert.ToByte(g), Convert.ToByte(b), 255);
            player.SendChatMessage("{ff0000}[Server] {ffffff}Fahrzeug gespawnt!");
            
        }

        [Command("engine")]
        public static void CMD_Engine(IPlayer player)
        {
            if (!player.IsInVehicle || player.Seat != 1)
                return;
            MyVehicle veh = (MyVehicle)player.Vehicle;
            veh.ToggleEngine();
        }

        [Command("repair")]
        public static void CMD_Repair(IPlayer player)
        {
            if (!player.IsInVehicle || player.Seat != 1)
                return;
            MyVehicle veh = (MyVehicle)player.Vehicle;
            veh.Repair();
        }

        public static Position GetRandomPositionAround(Position pos, float range)
        {
            Random rnd = new Random();
            float x = pos.X + (float)rnd.NextDouble() * (range * 2) - range;
            float y = pos.Y + (float)rnd.NextDouble() * (range * 2) - range;
            return new Position(x, y, pos.Z);
        }

        [Command("getpos", aliases: new string[] { "pos" })]
        public static void CMD_Getpos(MyPlayer player)
        {
            if (!player.LoggedIn)
                return;
            if (!player.HasAdminLevel(Utils.Enums.AdminRanks.Moderator))
            {
                player.SendChatMessage($"{Variables.Prefix} Du hast nicht die nötigen Rechte!");
                return;
            }
            Position position;
            Rotation rotation;
            string text;
            if (player.IsInVehicle)
            {
                position = player.Vehicle.Position;
                rotation = player.Vehicle.Rotation;
                text = $"VehiclePos: X:{position.X} Y:{position.Y} Z:{position.Z} Rotation:{rotation.Pitch}";
            }
            else
            {
                position = player.Position;
                rotation = player.Rotation;
                text = $"Pos: X:{position.X} Y:{position.Y} Z:{position.Z} Rotation:{rotation.Yaw};{rotation.Pitch};{rotation.Roll}";
            }
            Alt.Log($"{position.X} {position.Y} {position.Z} {rotation.Pitch}");
            player.SendChatMessage(text);
        }

        [Command("freecam")]
        public void CMD_freecam(MyPlayer player)
        {
            if (!player.LoggedIn)
                return;
            Alt.Emit("freecam:Toggle", player);
        }
        [Command("var")]
        public void CMD_var(MyPlayer player, string a, string b, string c, string d)
        {
            if (!player.LoggedIn)
                return;
            player.Emit("ars:SetPedVariation", a, b, c, d);
            
        }

        [Command("setadminlevel")]
        public static void CMD_SetAdminLevel(MyPlayer player, string username, int adminlevel)
        {
            if (!player.LoggedIn)
                return;
            if (!player.HasAdminLevel(Utils.Enums.AdminRanks.Admin))
            {
                player.SendChatMessage($"{Variables.Prefix} Du hast nicht die nötigen Rechte!");
                return;
            }
            if (!Enum.IsDefined(typeof(AdminRanks), adminlevel))
            {
                player.SendChatMessage("Das Adminlevel gibt es nicht!");
                return;
            }
            if (player.AdminLevel < adminlevel)
            {
                player.SendChatMessage($"{Variables.Prefix} Du hast nicht die nötigen Rechte!");
                return;
            }
            var target = MyPlayer.GetPlayerByName(username);
            if (target == null)
            {
                player.SendChatMessage("Spieler nicht gefunden!");
                return;
            }
            if (target.AdminLevel > player.AdminLevel)
            {
                player.SendChatMessage("Der Spieler hat mehr Rechte als du!");
                return;
            }
            target.AdminLevel = adminlevel;
            player.SendChatMessage($"Adminlevel von {target.Username} geändert!");
            Alt.Log($"{player.Username} has changed the adminlevel of {target.Username}!");
        }
        [Command("lock")]
        public static void CMD_Lock(MyPlayer player)
        {
            var veh = Util.getNearestVehicle(player, 3); 
            if(veh == null)
            {
                player.SendChatMessage("No vehicle");
            }
            else
            {
                player.SendChatMessage(Util.GetDistance(player.Position, veh.Position).ToString());
                veh.ToggleLock();
                player.SendChatMessage("Vehicle Lockstate:" + veh.LockState.ToString());
            }
        }

        [Command("teleport", aliases: new string[] { "tp" })]
        public static void CMD_Teleport(MyPlayer player, float x = 0, float y = 0, float z = 0)
        {
            if (!player.LoggedIn)
                return;
            if(x == 0 && y == 0 && z == 0)
            {
                if (player.IsInVehicle)
                    player.Vehicle.SetPosition(x, y, z);
                else
                    player.SetPosition(x, y, z);
            }
            else
            {
                if(player.IsInVehicle)
                    player.Vehicle.SetPosition(x, y, z);
                else
                    player.SetPosition(x, y, z);
            }
          
        }

    }
}
