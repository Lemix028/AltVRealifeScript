using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltVReallifeScript.Classes;
using AltVReallifeScript.Database;
using AltVReallifeScript.Entitys;
using AltVReallifeScript.Factories;
using AltVReallifeScript.Utils;
using System;
using System.Threading;

namespace AltVReallifeScript
{
    internal class ServerHandler : AsyncResource
    {
        //x -903,96924 y -363,63956 z: 113,073975
        //x -910,16705 y -446,14944 z: 115,39929
        public static CancellationTokenSource cancellationToken = new CancellationTokenSource();
        public override void OnStart()
        {
            Alt.Log($"{Variables.Prefix} ==MySql==");
            Alt.Log($"{Variables.Prefix} IP: {Variables.DBServer}:{Variables.DBPort}");
            Alt.Log($"{Variables.Prefix} DB Name: {Variables.DBDatabaseName}");
            Alt.Log($"{Variables.Prefix} Username: {Variables.DBUsername}");
            if(General.CheckMySqlConnection())
                Alt.Log($"{Variables.Prefix} Database connection established!");
            else
                Alt.Log($"{Variables.Prefix} Database connection could not be established!");
            ServerFunctions.CheckDiscordAccessTokensAsync(cancellationToken.Token);

        }

        public override void OnStop()
        {
            cancellationToken.Cancel();
            Alt.Log($"{Variables.Prefix} Save all players!");
            foreach (MyPlayer entry in Alt.GetAllPlayers())
            {
                entry.Save();
            }
        }

        public override IEntityFactory<IVehicle> GetVehicleFactory()
        {
            return new VehicleFactory();
        }
        public override IEntityFactory<IPlayer> GetPlayerFactory()
        {
            return new PlayerFactory();
        }
    }
}
