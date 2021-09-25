using AltV.Net;
using AltV.Net.Elements.Entities;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AltVReallife_DiscordAuthentificationSystem
{

    public class ServerHandler : Resource
    {

        public static string prefix = "[AltVReallifeDiscordAuth]";
        public static Config config;

        public override void OnStart()
        {
            if (!File.Exists("AltVReallifeDiscordAuthConfig.json"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                try
                {

                    TextWriter writer = null;
                    try
                    {
                        Config NewConfig = new Config();
                        //Discord Config
                        NewConfig.client_id = "1112222";
                        NewConfig.client_secret = "dffdssfd";
                        NewConfig.redirect_url = "https://domain.de:5050";
                        NewConfig.port = 5050;

                        writer = new StreamWriter("AltVReallifeDiscordAuthConfig.json", false);
                        writer.Write(JsonConvert.SerializeObject(NewConfig, Formatting.Indented));
                    }
                    finally
                    {
                        if (writer != null)
                            writer.Close();
                    }
                    Alt.Log($"{prefix} New config file created!");
                    Alt.Log($"{prefix} Please change the default values in config.json and restart.");
                }
                catch (Exception e)
                {
                    Alt.Log($"{prefix} Config Error! " + e);
                }
                Alt.Log($"{prefix} HTTP Server not started!");
                return;
            }
            else
            {
                try
                {
                    var fs = File.OpenRead("AltVReallifeDiscordAuthConfig.json");
                    StreamReader sr = new StreamReader(fs, new UTF8Encoding(false));

                    string json = sr.ReadToEnd();

                    config = JsonConvert.DeserializeObject<Config>(json);
                }
                catch (Exception e)
                {
                    Alt.Log($"{prefix} Config Read Error! " + e);
                }
            }
            try
            {
                MyHTTPServer httpserver = new MyHTTPServer($@"{Environment.CurrentDirectory}/index.html", config.port);
                httpserver.Initialize($@"{Environment.CurrentDirectory}/index.html", config.port);
            }
            catch (Exception e)
            {
                Alt.Log($"{prefix} HTTP SERVER ERRORED: {e}");
            }

            Alt.Log($"{prefix} HTTP auth server is running on this port: " + config.port);
            Alt.Log($"{prefix} Config successfully loaded!");
        }

        public override void OnStop()
        {

        }
   
    }
}