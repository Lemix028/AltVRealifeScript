using AltV.Net;
using AltVReallifeScript.Entitys;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AltVReallifeScript.Classes
{
    public class ServerFunctions : IScript
    {
        public static async void CheckDiscordAccessTokensAsync(CancellationToken cancellationToken)
        {
            await Task.Run(async () =>
            {
                while (true)
                {
                    foreach (MyPlayer entry in Alt.GetAllPlayers())
                    {
                        if (entry.DiscordId == null || entry.DiscordId == "")
                            return;
                        if (entry.DiscordAuth.datetime.AddSeconds(entry.DiscordAuth.expires_in) <= DateTime.Now.AddHours(48))
                        {
                            Alt.Emit("ars:RefreshAuthenfication", entry, entry.DiscordAuth.refresh_token);
                        }
                    }
                    await Task.Delay(60000*60, cancellationToken); //1 HOUR
                    if (cancellationToken.IsCancellationRequested)
                        break;
                }
            });     
        }
    }
}
