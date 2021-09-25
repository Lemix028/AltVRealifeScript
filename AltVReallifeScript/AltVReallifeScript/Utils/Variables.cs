using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AltVReallifeScript.Utils
{
    public class Variables : IScript
    {
        //General
        public static string ServerName = "LemixTest";
        public static string Prefix = "[SERVER]";

        //Database
        public static string DBServer = "127.0.0.1";
        public static int DBPort = 3306;
        public static string DBDatabaseName = "altv";
        public static string DBUsername= "root";
        private static string DBPassword = "xxx";
        public static string MySqlConnectionString = $"SERVER={DBServer}; PORT={DBPort}; DATABASE={DBDatabaseName}; UID={DBUsername}; PASSWORD={DBPassword}";
        
        //Discord Auth
        public static string client_id = "xxx"; // Discord Client ID
        public static string client_secret = "xxx"; // Discord Client Secret
        public static string redirect_url = "https://dev.xxx.de:5050"; // Discord Redirect

        //General 2
        public static Position SpawnLocation = new Position(-418.94f, 1147.32f, 325.86f);
        public static Rotation SpawnRotation = new Rotation(0 ,0 , 165f);
        public static ulong StartCash = 5000;

        public static IVoiceChannel channel = Alt.CreateVoiceChannel(true, 20f);

    }
}
