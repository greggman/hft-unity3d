using DeJson;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Net;
using WebSocketSharp.Server;

namespace HappyFunTimes
{
    public class HFTWebServer
    {
        public HFTWebServer(HFTRuntimeOptions options, string[] addresses)
        {
            m_log = new HFTLog("WebServer");
            m_options = options;
            string gameId = m_options.gameId;
            m_gamePath = "/games/" + gameId + "/";  // FIX: don't need this path? Just use root?
            m_webServerUtils = new HFTWebServerUtils(m_gamePath);

            HFTWebFileDB.GetInstance();

            // FIX: sysname and gamename
            string sysName = Environment.MachineName;
            if (sysName.EndsWith(".local"))
            {
                sysName = sysName.Substring(0, sysName.Length - 6);
            }
            string gameName = String.IsNullOrEmpty(m_options.name) ? Application.productName : m_options.name;
            string ping = Serializer.Serialize(new HFTPing(sysName + ": " + gameName, "HappyFunTimes"));
            m_ping = System.Text.Encoding.UTF8.GetBytes(ping);
            m_log.Info("Ping: " + ping);

            m_liveSettingsStr = "define([], function() { return " + Serializer.Serialize(new LiveSettings()) + "; })\n";
            m_liveSettings = System.Text.Encoding.UTF8.GetBytes(m_liveSettingsStr);

            m_hftSettingsStr = "window.hftSettings = " + Serializer.Serialize(new HFTSettings(true /* FIX options.showMenu */));

            if (options.captivePortal || options.installationMode)
            {
                m_captivePortalHandler = new HFTCaptivePortalHandler(m_webServerUtils);
                m_getRouter.Add(m_captivePortalHandler.HandleRequest);
            }

            m_getRouter.Add(HandleRoot);
            m_getRouter.Add(HandleLiveSettings);
            m_getRouter.Add(HandleController);
            m_getRouter.Add(HandleFile);

            m_addresses = addresses;
        }

        public void Start()
        {
            foreach (string address in m_addresses)
            {
                HttpServer server = StartServer(address);
                if (server != null)
                {
                    m_servers.Add(server);
                }
            }
        }

        HttpServer StartServer(string addressAndOptionalPort)
        {
            // FIX: deal with ip6
            int port = 18679;
            string[] ap = addressAndOptionalPort.Split(':');
            string address = ap[0];
            if (ap.Length > 1)
            {
                if (!String.IsNullOrEmpty(ap[1]) && !Int32.TryParse(ap[1].Trim(), out port))
                {
                    throw new System.ArgumentException("Bad server port, NaN: " + ap[1]);
                }
            }

            //server = new HttpServer (5963, true);
            //server = new HttpServer (System.Net.IPAddress.Parse ("127.0.0.1"), 4649);
            //server = new HttpServer (System.Net.IPAddress.Parse ("127.0.0.1"), 5963, true);
            //server = new HttpServer ("http://localhost:4649");
            //server = new HttpServer ("https://localhost:5963");
            //server = new HttpServer(System.Net.IPAddress.Parse("127.0.0.1"), 18679);
            HttpServer server = new HttpServer(System.Net.IPAddress.Parse(address), port);
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

                m_log.Info(path);
                m_getRouter.Route(path, req, res);
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
                PostCmd cmd = deserializer_.Deserialize<PostCmd>(result);
                if (cmd.cmd == "happyFunTimesPingForGame")
                {
                    m_webServerUtils.SendJsonBytes(res, m_ping);
                }
                else if (cmd.cmd == "happyFunTimesPing")
                {
                    // Yes reaching up this far is shit :(
                    if (!HFTGameManager.GetInstance().HaveGame())
                    {
                        res.StatusCode = (int)HttpStatusCode.NotFound;
                        return;
                    }

                    m_webServerUtils.SendJsonBytes(res, m_ping);
                }
                // TODO: use router
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

        public void Stop()
        {
            while (m_servers.Count > 0)
            {
                HttpServer server = m_servers[0];
                m_servers.RemoveAt(0);
                server.Stop();
            }
        }

        bool HandleFile(string path, HttpListenerRequest req, HttpListenerResponse res)
        {
            m_webServerUtils.SendFile(path, req, res);
            return true;
        }

        bool HandleRoot(string path, HttpListenerRequest req, HttpListenerResponse res)
        {
            if (path.Equals("/index.html") ||
                (path.Equals("/enter-name.html") && !(true /* FIX:m_options.askUserForName*/)))
            {
                var uri = req.Url;
                string url = uri.GetLeftPart(UriPartial.Authority) + m_gamePath + "controller.html" + uri.Query + uri.Fragment;
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

        bool HandleController(string path, HttpListenerRequest req, HttpListenerResponse res)
        {
            if (!path.EndsWith("/controller.html"))
            {
                return false;
            }
            byte[] templateData = null;
            string templatePath = "/hft/0.x.x/templates/controller.index.html";
            if (!m_webServerUtils.GetGameFile(templatePath, out templateData))
            {
                m_log.Error("missing template file: " + templatePath);
                res.StatusCode = (int)HttpStatusCode.NotFound;
                return true;
            }
            string template = System.Text.Encoding.UTF8.GetString(templateData);
            byte[] controllerData;
            if (!m_webServerUtils.GetGameFile(path, out controllerData))
            {
                m_log.Error("missing file: " + path);
                res.StatusCode = (int)HttpStatusCode.NotFound;
                return true;
            }
            string contents = System.Text.Encoding.UTF8.GetString(controllerData);
            // info.name
            // hftSettings
            // pages.controller.beforeScripts
            // pages.controller.afterScripts
            // contents
            Dictionary<string, string> subs = new Dictionary<string, string>();

            subs["filename"] = Path.GetFileNameWithoutExtension(path);
            subs["urls.favicon"] = "icon.png";
            subs["info.name"] = "???";
            subs["pages.controller.beforeScripts"] = "<script  src=\"/hft/0.x.x/extra/apphelper.js\"></script>";
            subs["pages.controller.afterScripts"] = @"
<script src=""3rdparty/require.js"" data-main=""scripts/controller.js"" type=""hft-late""></script>
<script type=""hft-late"">
requirejs.config({
  paths: {
    hft: '../../../../hft/0.x.x/scripts',
    },
    shim: {
      '3rdparty/pep.min': {
          //These script dependencies should be loaded before loading
          //pep.js
          deps: [],
          //Once loaded, use the global 'PEPNOREALLY' as the
          //module value.
          exports: 'PEPNOTREALLY',
      },
  },
});
</script>
";
            subs["content"] = contents;
            subs["hftSettings"] = m_hftSettingsStr;

            string s = HFTUtil.ReplaceParamsFlat(template, subs);

            m_webServerUtils.SendContent(res, path, s);
            return true;
        }

        class SystemSettings
        {
            public bool checkForApp = false; // FIX?
        }

        class LiveSettings
        {
            public SystemSettings system = new SystemSettings();
        }

        class HFTSettings
        {
            public HFTSettings(bool showMenu)
            {
                menu = showMenu;
            }
            public bool menu = true;
            public string apiVersion = "1.14.0";
        }

        Deserializer deserializer_ = new Deserializer();
        HFTRuntimeOptions m_options;
        string[] m_addresses;  // Addresses to listen in ip:port format?
        List<HttpServer> m_servers = new List<HttpServer>();
        HFTWebServerUtils m_webServerUtils;
        HFTRouter m_getRouter = new HFTRouter();
        HFTCaptivePortalHandler m_captivePortalHandler;
        HFTLog m_log;
        string m_gamePath;
        byte[] m_ping;
        string m_liveSettingsStr;
        byte[] m_liveSettings;
        string m_hftSettingsStr;
    }

}  // namespace HappyFunTimes

