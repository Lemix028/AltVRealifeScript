using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Web;
using AltVReallife_DiscordAuthentificationSystem;
using Newtonsoft.Json;
using AltV.Net;
using System.Security.Cryptography;
using AltV.Net.Elements.Entities;

public class MyHTTPServer : IScript
{
    private readonly string[] _indexFiles = {
        "index.html",
        "index.htm",
        "default.html",
        "default.htm"
    };

    private static IDictionary<string, string> _mimeTypeMappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase) {
        #region extension to MIME type list
        {".asf", "video/x-ms-asf"},
        {".asx", "video/x-ms-asf"},
        {".avi", "video/x-msvideo"},
        {".bin", "application/octet-stream"},
        {".cco", "application/x-cocoa"},
        {".crt", "application/x-x509-ca-cert"},
        {".css", "text/css"},
        {".deb", "application/octet-stream"},
        {".der", "application/x-x509-ca-cert"},
        {".dll", "application/octet-stream"},
        {".dmg", "application/octet-stream"},
        {".ear", "application/java-archive"},
        {".eot", "application/octet-stream"},
        {".exe", "application/octet-stream"},
        {".flv", "video/x-flv"},
        {".gif", "image/gif"},
        {".hqx", "application/mac-binhex40"},
        {".htc", "text/x-component"},
        {".htm", "text/html"},
        {".html", "text/html"},
        {".ico", "image/x-icon"},
        {".img", "application/octet-stream"},
        {".iso", "application/octet-stream"},
        {".jar", "application/java-archive"},
        {".jardiff", "application/x-java-archive-diff"},
        {".jng", "image/x-jng"},
        {".jnlp", "application/x-java-jnlp-file"},
        {".jpeg", "image/jpeg"},
        {".jpg", "image/jpeg"},
        {".js", "application/x-javascript"},
        {".mml", "text/mathml"},
        {".mng", "video/x-mng"},
        {".mov", "video/quicktime"},
        {".mp3", "audio/mpeg"},
        {".mpeg", "video/mpeg"},
        {".mpg", "video/mpeg"},
        {".msi", "application/octet-stream"},
        {".msm", "application/octet-stream"},
        {".msp", "application/octet-stream"},
        {".pdb", "application/x-pilot"},
        {".pdf", "application/pdf"},
        {".pem", "application/x-x509-ca-cert"},
        {".pl", "application/x-perl"},
        {".pm", "application/x-perl"},
        {".png", "image/png"},
        {".prc", "application/x-pilot"},
        {".ra", "audio/x-realaudio"},
        {".rar", "application/x-rar-compressed"},
        {".rpm", "application/x-redhat-package-manager"},
        {".rss", "text/xml"},
        {".run", "application/x-makeself"},
        {".sea", "application/x-sea"},
        {".shtml", "text/html"},
        {".sit", "application/x-stuffit"},
        {".swf", "application/x-shockwave-flash"},
        {".tcl", "application/x-tcl"},
        {".tk", "application/x-tcl"},
        {".txt", "text/plain"},
        {".war", "application/java-archive"},
        {".wbmp", "image/vnd.wap.wbmp"},
        {".wmv", "video/x-ms-wmv"},
        {".xml", "text/xml"},
        {".xpi", "application/x-xpinstall"},
        {".zip", "application/zip"},
        #endregion
    };
    private Thread _serverThread;
    private string _rootDirectory;
    private HttpListener _listener;
    private int _port;

    public int Port
    {
        get { return _port; }
        private set { }
    }

    /// <summary>
    /// Construct server with given port.
    /// </summary>
    /// <param name="path">Directory path to serve.</param>
    /// <param name="port">Port of the server.</param>
    public MyHTTPServer(string path, int port)
    {
        
    }

    public MyHTTPServer()
    {

    }

    /// <summary>
    /// Construct server with suitable port.
    /// </summary>
    /// <param name="path">Directory path to serve.</param>
    public MyHTTPServer(string path)
    {
        //get an empty port
        TcpListener l = new TcpListener(IPAddress.Loopback, 0);
        l.Start();
        int port = ((IPEndPoint)l.LocalEndpoint).Port;
        l.Stop();
        this.Initialize(path, port);
    }

    /// <summary>
    /// Stop server and dispose all functions.
    /// </summary>
    public void Stop()
    {
        _serverThread.Abort();
        _listener.Stop();
    }

    private void Listen()
    {
        _listener = new HttpListener();
        _listener.Prefixes.Add("https://*:" + _port.ToString() + "/");
        try
        {
            _listener.Start();
        }
        catch(HttpListenerException hle)
        {
            Alt.Log(hle.ToString());
            return;
        }
        
        while (true)
        {
            try
            {
                HttpListenerContext context = _listener.GetContext();
                Process(context);

                DiscordAuthorization(context);
            }
            catch (Exception ex)
            {
                Alt.Log(ex.ToString());
            }
        }
    }
    private void AuthErrored(string state, string msg)
    {
        foreach (var entry in Alt.GetAllPlayers())
        {
            if (GetSha256Hash(SHA256.Create(), entry.HardwareIdHash + ":" + entry.HardwareIdExHash.ToString()) == state)
            {
                Alt.Emit("ars:DiscordAuthError", entry, msg);
                return;
            }
        }
        Alt.Emit("ars:DiscordAuthError", null, msg);
        return;
    }
    private void DiscordAuthorization(HttpListenerContext context)
    {
        var request = context.Request;
        string code = "";
        string state = "";
        try
        {
            code = request.QueryString["code"];
            state = request.QueryString["state"];
        }
        catch (Exception)
        {
            return;
        }
        if(code == null || state == null)
            return;
        if (code.Length <= 10 || state.Length <= 10)
            return;

        ResponseObject responseobj;
        try
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("https://discordapp.com/api/oauth2/token");
            webRequest.Method = "POST";
            string parameters = "client_id=" + ServerHandler.config.client_id + "&client_secret=" + ServerHandler.config.client_secret + "&grant_type=authorization_code&code=" + code + "&redirect_uri=" + ServerHandler.config.redirect_url + "";
            Alt.Log(parameters);
            byte[] byteArray = Encoding.UTF8.GetBytes(parameters);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ContentLength = byteArray.Length;
            Stream postStream = webRequest.GetRequestStream();

            postStream.Write(byteArray, 0, byteArray.Length);
            postStream.Close();
            WebResponse response = webRequest.GetResponse();
            postStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(postStream);
            string responseFromServer = reader.ReadToEnd();
            responseobj = JsonConvert.DeserializeObject<ResponseObject>(responseFromServer);
            
        } catch(WebException we)
        {
            if(we.Status == WebExceptionStatus.ProtocolError)
                AuthErrored(state, "Bad Request");

            Alt.Log(we.Message);
            return;
        }
        catch (Exception e) { Alt.Log(ServerHandler.prefix + " " + e.Message); return; }
        if(responseobj.access_token.Length == 0)
        {
            AuthErrored(state, "Access token NULL");
            return;
        }
            

        if(responseobj.access_token != "" && responseobj.scope == "identify")
        {
            try
            {
                HttpWebRequest webRequest1 = (HttpWebRequest)WebRequest.Create("https://discordapp.com/api/users/@me");
                webRequest1.Method = "Get";
                webRequest1.ContentLength = 0;
                webRequest1.Headers.Add("Authorization", "Bearer " + responseobj.access_token);
                webRequest1.ContentType = "application/x-www-form-urlencoded";

                string apiResponse1 = "";
                using (HttpWebResponse response1 = webRequest1.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader1 = new StreamReader(response1.GetResponseStream());
                    apiResponse1 = reader1.ReadToEnd();
                    DiscordUserData userdata = JsonConvert.DeserializeObject<DiscordUserData>(apiResponse1);
                    responseobj.datetime = DateTime.Now;
                    foreach (var entry in Alt.GetAllPlayers())
                    {
                        if(GetSha256Hash(SHA256.Create(), entry.HardwareIdHash + ":" + entry.HardwareIdExHash.ToString()) == state)
                        {
                            Alt.Emit("ars:DiscordAuthSuccess", entry, userdata.id, userdata.username, userdata.avatar, userdata.discriminator, JsonConvert.SerializeObject(responseobj)); 
                            return;
                        }
                    }
                    AuthErrored(state, "Player not found");
                }
            }
            catch
            {
                AuthErrored(state, "Access token invalid");
                return;
            }
           
        }
        else
        {
            AuthErrored(state, "Access token or scope invalid");
            return;
        }
        
    }

    private void Process(HttpListenerContext context)
    {
        try
        {
            string filename = context.Request.Url.AbsolutePath;
            filename = filename.Substring(1);
            if (string.IsNullOrEmpty(filename))
            {
                foreach (string indexFile in _indexFiles)
                {
                    if (File.Exists(Path.Combine(_rootDirectory, indexFile)))
                    {
                        filename = indexFile;
                        break;
                    }
                }
            }

            filename = Path.Combine(_rootDirectory, filename);

            if (File.Exists(filename))
            {
                try
                {
                    Stream input = new FileStream(filename, FileMode.Open);

                    //Adding permanent http response headers
                    string mime;
                    context.Response.ContentType = _mimeTypeMappings.TryGetValue(Path.GetExtension(filename), out mime) ? mime : "application/octet-stream";
                    context.Response.ContentLength64 = input.Length;
                    context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
                    context.Response.AddHeader("Last-Modified", System.IO.File.GetLastWriteTime(filename).ToString("r"));

                    byte[] buffer = new byte[1024 * 16];
                    int nbytes;
                    while ((nbytes = input.Read(buffer, 0, buffer.Length)) > 0)
                        context.Response.OutputStream.Write(buffer, 0, nbytes);
                    input.Close();

                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    context.Response.OutputStream.Flush();
                }
                catch (Exception)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }

            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }

            context.Response.OutputStream.Close();
        }catch(Exception e)
        {
            Alt.Log(e.Message);
        }
      
    }

    public void Initialize(string path, int port)
    {
        this._rootDirectory = path;
        this._port = port;
        _serverThread = new Thread(this.Listen);
        _serverThread.Start();
    }

    public static string GetSha256Hash(SHA256 shaHash, string input)
    {
        // Convert the input string to a byte array and compute the hash.
        byte[] data = shaHash.ComputeHash(Encoding.UTF8.GetBytes(input));

        // Create a new Stringbuilder to collect the bytes
        // and create a string.
        StringBuilder sBuilder = new StringBuilder();

        // Loop through each byte of the hashed data 
        // and format each one as a hexadecimal string.
        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }

        // Return the hexadecimal string.
        return sBuilder.ToString();
    }



    [ServerEvent("ars:RefreshAuthenfication")]
    public void RefreshAuthenfication(Player player, string refresh_token)
    {
        ResponseObject responseobj = new ResponseObject();
        try
        {
            var Hash = MyHTTPServer.GetSha256Hash(SHA256.Create(), player.HardwareIdHash + ":" + player.HardwareIdExHash.ToString());
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("https://discordapp.com/api/oauth2/token");
            webRequest.Method = "POST";
            string parameters = "client_id=" + ServerHandler.config.client_id + "&client_secret=" + ServerHandler.config.client_secret + "&grant_type=refresh_token&refresh_token=" + refresh_token + "&redirect_uri=" + ServerHandler.config.redirect_url + "&scope=identify" + $"&state={Hash}";
            byte[] byteArray = Encoding.UTF8.GetBytes(parameters);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ContentLength = byteArray.Length;
            Stream postStream = webRequest.GetRequestStream();
            postStream.Write(byteArray, 0, byteArray.Length);
            postStream.Close();
            WebResponse response = webRequest.GetResponse();
            postStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(postStream);
            string responseFromServer = reader.ReadToEnd();
            responseobj = JsonConvert.DeserializeObject<ResponseObject>(responseFromServer);
        }
        catch (WebException we)
        {
            if (we.Status == WebExceptionStatus.ProtocolError)
                AuthErrored(player, "Bad Request");

            Alt.Log(we.Message);
            return;
        }
        catch (Exception e) { Alt.Log(ServerHandler.prefix + " " + e.Message); return; }
        if (responseobj.access_token.Length == 0)
        {
            AuthErrored(player, "Access token NULL");
            return;
        }


        if (responseobj.access_token != "" && responseobj.scope == "identify")
        {
            try
            {
                HttpWebRequest webRequest1 = (HttpWebRequest)WebRequest.Create("https://discordapp.com/api/users/@me");
                webRequest1.Method = "Get";
                webRequest1.ContentLength = 0;
                webRequest1.Headers.Add("Authorization", "Bearer " + responseobj.access_token);
                webRequest1.ContentType = "application/x-www-form-urlencoded";

                string apiResponse1 = "";
                using (HttpWebResponse response1 = webRequest1.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader1 = new StreamReader(response1.GetResponseStream());
                    apiResponse1 = reader1.ReadToEnd();
                    DiscordUserData userdata = JsonConvert.DeserializeObject<DiscordUserData>(apiResponse1);
                    responseobj.datetime = DateTime.Now;
                    Alt.Emit("ars:UpdateDiscordAuth", player, userdata.id, userdata.username, userdata.avatar, userdata.discriminator, JsonConvert.SerializeObject(responseobj));
                    return;
                }
            }
            catch
            {
                AuthErrored(player, "Access token invalid");
                return;
            }


        }
    }

    [ServerEvent("ars:AccessTokenLogin")]
    public void OnAccessTokenLogin(IPlayer player, string responseobjs)
    {
        ResponseObject responseobj = JsonConvert.DeserializeObject<ResponseObject>(responseobjs);
        if (responseobj.access_token.Length == 0)
        {
            AuthErrored(player, "Access token NULL");
            return;
        }


        if (responseobj.access_token != "" && responseobj.scope == "identify")
        {
            try
            {
                HttpWebRequest webRequest1 = (HttpWebRequest)WebRequest.Create("https://discordapp.com/api/users/@me");
                webRequest1.Method = "Get";
                webRequest1.ContentLength = 0;
                webRequest1.Headers.Add("Authorization", "Bearer " + responseobj.access_token);
                webRequest1.ContentType = "application/x-www-form-urlencoded";

                string apiResponse1 = "";
                using (HttpWebResponse response1 = webRequest1.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader1 = new StreamReader(response1.GetResponseStream());
                    apiResponse1 = reader1.ReadToEnd();
                    DiscordUserData userdata = JsonConvert.DeserializeObject<DiscordUserData>(apiResponse1);
                    responseobj.datetime = DateTime.Now;
                    Alt.Emit("ars:UpdateDiscordAuth", player, userdata.id, userdata.username, userdata.avatar, userdata.discriminator, JsonConvert.SerializeObject(responseobj));
                    Alt.Emit("ars:DiscordAuthSuccess", player, userdata.id, userdata.username, userdata.avatar, userdata.discriminator, JsonConvert.SerializeObject(responseobj));
                    return;
                }
            }
            catch
            {
                AuthErrored(player, "Access token invalid");
                var hash = MyHTTPServer.GetSha256Hash(SHA256.Create(), player.HardwareIdHash + ":" + player.HardwareIdExHash.ToString());
                string url = $@"https://discord.com/oauth2/authorize?client_id={ServerHandler.config.client_id}&redirect_uri={ServerHandler.config.redirect_url}&response_type=code&scope=identify&state={hash}";
                Alt.Emit("ars:ClearDiscordAuth", player);
                player.Emit("ars:SetNewPage", url);
                return;
            }


        }
    }

    private void AuthErrored(IPlayer player, string msg)
    {
        Alt.Emit("ars:DiscordAuthError", player, msg);
        return;
    }


}