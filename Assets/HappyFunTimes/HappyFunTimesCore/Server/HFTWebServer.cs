using DeJson;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using WebSocketSharp;
using WebSocketSharp.Net;
using WebSocketSharp.Server;

namespace HappyFunTimes
{
    public class HFTWebServer
    {
        public HFTWebServer(HFTRuntimeOptions options, string[] addresses)
        {
            m_log = new HFTLog("HFTWebServer");
            m_options = options;
            m_gamePath = "/";
            m_webServerUtils = new HFTWebServerUtils(m_gamePath);

            // Touch the HFTWebFileDB
            // We do this be because we want it to get the list
            // of files BEFORE run the server. The server will
            // run in a different thread and HFTWebFileDB will
            // not be able to populate its database from that thread.
            HFTWebFileDB.GetInstance();

            // FIX: sysname and gamename
            string sysName = Environment.MachineName;
            if (sysName.EndsWith(".local"))
            {
                sysName = sysName.Substring(0, sysName.Length - 6);
            }
            string gameName = m_options.name;
            string ping = Serializer.Serialize(new HFTPing(sysName + ": " + gameName, "HappyFunTimes"));
            m_ping = System.Text.Encoding.UTF8.GetBytes(ping);
            m_log.Info("Ping: " + ping);

            m_liveSettingsStr = "define([], function() { return " + Serializer.Serialize(new LiveSettings()) + "; })\n";
            m_liveSettings = System.Text.Encoding.UTF8.GetBytes(m_liveSettingsStr);

            if (options.captivePortal || options.installationMode)
            {
                m_captivePortalHandler = new HFTCaptivePortalHandler(m_webServerUtils);
                m_getRouter.Add(m_captivePortalHandler.HandleRequest);
            }

            m_getRouter.Add(HandleRoot);
            m_getRouter.Add(HandleLiveSettings);
            m_getRouter.Add(HandleFile);
            m_getRouter.Add(HandleMissingRoute);
            m_getRouter.Add(HandleNotFound);

            m_postCmdHandlers["happyFunTimesPingForGame"] = HandleCmdPingForGame;
            m_postCmdHandlers["happyFunTimesPing"]        = HandleCmdPing;
            m_postCmdHandlers["happyFunTimesRedir"]       = HandleCmdRedir;
            m_postCmdHandlers["time"]                     = HandleCmdTime;
            m_postCmdHandlers["quit"]                     = HandleCmdQuit;

            m_addresses = addresses;
        }

        public void Start()
        {
            foreach (string address in m_addresses)
            {
                m_log.Info("Trying address: " + address);
                HttpServer server = StartServer(address);
                if (server != null)
                {
                    m_log.Info("Added address: " + address);
                    m_servers.Add(server);
                }
            }
        }

        string Unwrap(string s)
        {
            return (s.StartsWith("[") && s.EndsWith("]")) ? s.Substring(1, s.Length - 2) : s;
        }

        HttpServer StartServer(string addressAndOptionalPort)
        {
//            string host = "http://" + addressAndOptionalPort;
//            var addressUri = new System.Uri(addressAndOptionalPort);
//            int port = addressUri.Port;
//            string address = Unwrap(addressUri.Host);
            //server = new HttpServer (5963, true);
            //server = new HttpServer (System.Net.IPAddress.Parse ("127.0.0.1"), 4649);
            //server = new HttpServer (System.Net.IPAddress.Parse ("127.0.0.1"), 5963, true);
            //server = new HttpServer ("http://localhost:4649");
            //server = new HttpServer ("https://localhost:5963");
            //server = new HttpServer(System.Net.IPAddress.Parse("127.0.0.1"), 18679);
//            HttpServer server = new HttpServer(System.Net.IPAddress.Parse(address), port);
            HttpServer server = new HttpServer(addressAndOptionalPort);
            //server.Log.Level = LogLevel.Trace;
            //server.Log.File = "/Users/gregg/temp/foo.txt";
            #if FALSE
            // To change the logging level.
            server.Log.Level = LogLevel.Trace;

            // To change the wait time for the response to the WebSocket Ping or Close.
            server.WaitTime = TimeSpan.FromSeconds (2);
            #endif
            /* To provide the secure connection.
            var cert = ConfigurationManager.AppSettings["ServerCertFile"];
            var passwd = ConfigurationManager.AppSettings["CertFilePassword"];
            server.SslConfiguration.ServerCertificate = new X509Certificate2 (cert, passwd);
             */

            /* To provide the HTTP Authentication (Basic/Digest).
            server.AuthenticationSchemes = AuthenticationSchemes.Basic;
            server.Realm = "WebSocket Test";
            server.UserCredentialsFinder = id => {
              var name = id.Name;
              // Return user name, password, and roles.
              return name == "nobita"
                     ? new NetworkCredential (name, "password", "gunfighter")
                     : null; // If the user credentials aren't found.
            };
             */

            // Set the document root path.
            //    server.RootPath = ConfigurationManager.AppSettings["RootPath"];

            // Set the HTTP GET request event.
            server.OnGet += (sender, e) =>
            {
                var req = e.Request;
                var res = e.Response;
                var uri = req.Url;
                var path = uri.AbsolutePath;
                if (String.IsNullOrEmpty(path))
                {
                    path = "/";
                }

                if (path == "/")
                {
                    path += "index.html";
                }
                //Debug.Log("HFTWebServer: " + uri.ToString() + " ========================");
                //Debug.Log("remote ip: " + req.RemoteEndPoint.Address.ToString());
                //Debug.Log("Num Headers: " + req.Headers.Count);
                //foreach(string key in req.Headers.AllKeys)
                //{
                //    string value = req.Headers.Get(key);
                //    Debug.Log("Header: " + key + " = " + value);
                //}

                m_log.Info(path);
                if (!m_getRouter.Route(path, req, res))
                {
                    throw new System.InvalidOperationException("no route for request: " + path);
                }
            };

            server.OnOptions += (sender, e) =>
            {
                var res = e.Response;

                res.AddHeader("Access-Control-Allow-Origin", "*");
                res.AddHeader("Access-Control-Allow-Methods", "GET,POST,OPTIONS");
                res.AddHeader("Access-Control-Allow-Headers", "X-Requested-With, X-HTTP-Method-Override, Content-Type, Accept");
                res.AddHeader("Access-Control-Allow-Credentials", "false");
                res.AddHeader("Access-Control-Max-Age", "86400");
                res.StatusCode = (int)HttpStatusCode.OK;
                res.WriteContent(new byte[0]);
            };

            server.OnPost += (sender, e) =>
            {
                var req = e.Request;
                var res = e.Response;

                Stream dataStream = req.InputStream;
                StreamReader reader = new StreamReader(dataStream);
                string result = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();

                // This is hacky. Basically websever repsonds long before
                // the game is ready to accept players. So players connect
                // and get rejected (no such game). There's probably a better
                // solution but this lets the game check if the server is
                // running immediately but controllers won't be told
                // until the game has registered with the server.
                //
                // The better solution is probably try to figure out
                // why the game doesn't connect first.
                //
                // Note, nornally you'd start a game, then ask players
                // to join. The case above happens while developing. You
                // run the game once. Open a browser window. Quit the game.
                // The browser is now trying to reconnect. The moment you
                // run the game the browser reconnects but the system
                // isn't fully ready yet.
                PostCmd cmd = null;
                try
                {
                    cmd = deserializer_.Deserialize<PostCmd>(result);
                }
                catch (System.Exception)
                {
                    m_log.Warn("could't deseralize PostCmd:" + result);
                }
                if (cmd == null || cmd.cmd == null)
                {
                    res.ContentType = "application/json";
                    res.StatusCode = (int)HttpStatusCode.BadRequest;
                    res.WriteContent(System.Text.Encoding.UTF8.GetBytes("{\"error\":\"bad command data:\"}"));
                }

                m_log.Info(cmd.cmd);
                PostCmdHandler handler = null;
                if (m_postCmdHandlers.TryGetValue(cmd.cmd, out handler))
                {
                    handler(req, res);
                    return;
                }

                m_log.Error("Unknown cmd: " + cmd);
                res.ContentType = "application/json";
                res.StatusCode = (int)HttpStatusCode.BadRequest;
                res.WriteContent(System.Text.Encoding.UTF8.GetBytes("{\"error\":\"unknown cmd: " + cmd + "\"}"));
            };

            // Not to remove the inactive WebSocket sessions periodically.
            //server.KeepClean = false;

            // To resolve to wait for socket in TIME_WAIT state.
            //server.ReuseAddress = true;

            // Add the WebSocket services.
            //    server.AddWebSocketService<Echo> ("/Echo");
            //    server.AddWebSocketService<Chat> ("/Chat");
            server.AddWebSocketService<HFTSocket>("/");

            /* Add the WebSocket service with initializing.
            server.AddWebSocketService<Chat> (
              "/Chat",
              () => new Chat ("Anon#") {
                // To send the Sec-WebSocket-Protocol header that has a subprotocol name.
                Protocol = "chat",
                // To emit a WebSocket.OnMessage event when receives a ping.
                EmitOnPing = true,
                // To ignore the Sec-WebSocket-Extensions header.
                IgnoreExtensions = true,
                // To validate the Origin header.
                OriginValidator = val => {
                  // Check the value of the Origin header, and return true if valid.
                  Uri origin;
                  return !val.IsNullOrEmpty () &&
                         Uri.TryCreate (val, UriKind.Absolute, out origin) &&
                         origin.Host == "localhost";
                },
                // To validate the Cookies.
                CookiesValidator = (req, res) => {
                  // Check the Cookies in 'req', and set the Cookies to send to the client with 'res'
                  // if necessary.
                  foreach (Cookie cookie in req) {
                    cookie.Expired = true;
                    res.Add (cookie);
                  }
                  return true; // If valid.
                }
              });
             */
            server.Start();
            if (server.IsListening)
            {
                m_log.Info(String.Format("Listening on {0} port {1}, and providing WebSocket services:", server.Address, server.Port));
                //foreach (var path in server.WebSocketServices.Paths) Debug.Log(String.Format("- {0}", path));
            }
            return server;
        }

        public void StopListening()
        {
            // It's not clear to me where this belongs. Ideally I'd like to pass in the HFTGameManager
            // but how to pass it to new connections I have no idea.
            HFTGameManager.GetInstance().Close();
        }

        public void Stop()
        {
            StopListening();

            while (m_servers.Count > 0)
            {
                HttpServer server = m_servers[0];
                m_servers.RemoveAt(0);
                server.Stop();
            }
        }

        bool HandleMissingRoute(string path, HttpListenerRequest req, HttpListenerResponse res)
        {
            if (path.EndsWith(".html")) {
                if (m_webServerUtils.SendFile("/hft/missing.html", req, res)) {
                    return true;
                }
            }
            return false;
        }

        bool HandleFile(string path, HttpListenerRequest req, HttpListenerResponse res)
        {
            return m_webServerUtils.SendFile(path, req, res);
        }

        bool HandleRoot(string path, HttpListenerRequest req, HttpListenerResponse res)
        {
            if (path.Equals("/index.html") ||
                path.Equals("/enter-name.html"))
            {
                var uri = req.Url;
                string url = uri.GetLeftPart(UriPartial.Authority) + m_gamePath + m_options.controllerFilename + uri.Query + uri.Fragment;
                res.StatusCode = (int)HttpStatusCode.Redirect;
                res.AddHeader("Location", url);
                res.ContentType = "text/html";
                res.WriteContent(System.Text.Encoding.UTF8.GetBytes("<script>window.location.href = decodeURIComponent(\"" + Uri.EscapeDataString(url) + "\");</script>"));
                m_log.Info("redirect: " + url);
                return true;
            }
            return false;
        }

        bool HandleLiveSettings(string path, HttpListenerRequest req, HttpListenerResponse res)
        {
            if (!path.Equals("/hft/0.x.x/scripts/runtime/live-settings.js"))
            {
                return false;
            }

            m_webServerUtils.SendJsonBytes(res, m_liveSettings);
            return true;
        }

        bool HandleNotFound(string path, HttpListenerRequest req, HttpListenerResponse res)
        {
            res.ContentType = "text/text";
            res.StatusCode = (int)HttpStatusCode.NotFound;
            string error = "unknown path: " + req.RawUrl;
            res.WriteContent(System.Text.Encoding.UTF8.GetBytes(error));
            m_log.Info(error);
            return true;
        }

        void HandleCmdPingForGame(HttpListenerRequest req, HttpListenerResponse res)
        {
            m_webServerUtils.SendJsonBytes(res, m_ping);
        }

        void HandleCmdPing(HttpListenerRequest req, HttpListenerResponse res)
        {
            // Yes reaching up this far is shit :(
            if (!HFTGameManager.GetInstance().HaveGame())
            {
                res.StatusCode = (int)HttpStatusCode.NotFound;
            }

            m_webServerUtils.SendJsonBytes(res, m_ping);
        }

        void HandleCmdRedir(HttpListenerRequest req, HttpListenerResponse res)
        {
            string controllerPath = m_gamePath + m_options.controllerFilename;
            string redirStr = Serializer.Serialize(new Redir(controllerPath));
            byte[] redir = System.Text.Encoding.UTF8.GetBytes(redirStr);
            m_webServerUtils.SendJsonBytes(res, redir);
        }

        void HandleCmdTime(HttpListenerRequest req, HttpListenerResponse res)
        {
            double seconds = HFTUtil.GetJSTimeInSeconds();
            string timeStr = Serializer.Serialize(new HFTTimePing(seconds));
            byte[] time = System.Text.Encoding.UTF8.GetBytes(timeStr);
            m_webServerUtils.SendJsonBytes(res, time);
        }

        void HandleCmdQuit(HttpListenerRequest req, HttpListenerResponse res)
        {
            m_webServerUtils.SendContent(res, "quit.txt", "quit");
            Stop();
            System.Environment.Exit(0);
        }

        class SystemSettings
        {
            public bool checkForApp = false; // FIX?
        }

        class LiveSettings
        {
            public SystemSettings system = new SystemSettings();
        }

        class Redir {
            public Redir(string pathname) {
                this.pathname = pathname;
            }
            public string pathname;
        }

        delegate void PostCmdHandler(HttpListenerRequest req, HttpListenerResponse res);

        Deserializer deserializer_ = new Deserializer();
        HFTRuntimeOptions m_options;
        string[] m_addresses;  // Addresses to listen in ip:port format?
        List<HttpServer> m_servers = new List<HttpServer>();
        HFTWebServerUtils m_webServerUtils;
        HFTRouter m_getRouter = new HFTRouter();
        Dictionary<string, PostCmdHandler> m_postCmdHandlers = new Dictionary<string, PostCmdHandler>();
        HFTCaptivePortalHandler m_captivePortalHandler;
        HFTLog m_log;
        string m_gamePath;
        byte[] m_ping;
        string m_liveSettingsStr;
        byte[] m_liveSettings;
    }

}  // namespace HappyFunTimes

