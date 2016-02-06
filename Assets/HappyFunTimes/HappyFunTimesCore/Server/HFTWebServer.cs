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
        public HFTWebServer(HFTGameOptions options)
        {
            m_log = new HFTLog("WebServer");
            m_options = options;
            string gameId = m_options.getGameId();
            m_gamePath = "/games/" + gameId + "/";

            HFTWebFileDB.GetInstance();

            // FIX: sysname and gamename
            string sysName = Environment.MachineName;
            if (sysName.EndsWith(".local"))
            {
                sysName = sysName.Substring(0, sysName.Length - 6);
            }
            string gameName = String.IsNullOrEmpty(m_options.name) ? gameId : m_options.name;
            string ping = Serializer.Serialize(new HFTPing(sysName + ": " + gameName, "HappyFunTimes"));
            m_ping = System.Text.Encoding.UTF8.GetBytes(ping);
            m_log.Info("Ping: " + ping);

            m_liveSettingsStr = "define([], function() { return " + Serializer.Serialize(new LiveSettings()) + "; })\n";
            m_liveSettings = System.Text.Encoding.UTF8.GetBytes(m_liveSettingsStr);

            m_hftSettingsStr = "window.hftSettings = " + Serializer.Serialize(new HFTSettings(options.showMenu));
        }

        public void Start()
        {
            int port = 18679;
            if (!String.IsNullOrEmpty(m_options.serverPort) && !Int32.TryParse(m_options.serverPort.Trim(), out port))
            {
                throw new System.ArgumentException("Bad server port, NaN: " + m_options.serverPort);
            }

            //m_httpsv = new HttpServer (5963, true);
            //m_httpsv = new HttpServer (System.Net.IPAddress.Parse ("127.0.0.1"), 4649);
            //m_httpsv = new HttpServer (System.Net.IPAddress.Parse ("127.0.0.1"), 5963, true);
            //m_httpsv = new HttpServer ("http://localhost:4649");
            //m_httpsv = new HttpServer ("https://localhost:5963");
            //m_httpsv = new HttpServer(System.Net.IPAddress.Parse("127.0.0.1"), 18679);
            m_httpsv = new HttpServer(System.Net.IPAddress.Parse("0.0.0.0"), port);
            m_httpsv.Log.Level = LogLevel.Trace;
            m_httpsv.Log.File = "/Users/gregg/temp/foo.txt";
            #if FALSE
            // To change the logging level.
            m_httpsv.Log.Level = LogLevel.Trace;

            // To change the wait time for the response to the WebSocket Ping or Close.
            m_httpsv.WaitTime = TimeSpan.FromSeconds (2);
            #endif
            /* To provide the secure connection.
            var cert = ConfigurationManager.AppSettings["ServerCertFile"];
            var passwd = ConfigurationManager.AppSettings["CertFilePassword"];
            m_httpsv.SslConfiguration.ServerCertificate = new X509Certificate2 (cert, passwd);
             */

            /* To provide the HTTP Authentication (Basic/Digest).
            m_httpsv.AuthenticationSchemes = AuthenticationSchemes.Basic;
            m_httpsv.Realm = "WebSocket Test";
            m_httpsv.UserCredentialsFinder = id => {
              var name = id.Name;
              // Return user name, password, and roles.
              return name == "nobita"
                     ? new NetworkCredential (name, "password", "gunfighter")
                     : null; // If the user credentials aren't found.
            };
             */

            // Set the document root path.
            //    m_httpsv.RootPath = ConfigurationManager.AppSettings["RootPath"];

            // Set the HTTP GET request event.
            m_httpsv.OnGet += (sender, e) =>
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

                // TODO: Create a router
                if (path.Equals("/index.html") ||
                    (path.Equals("/enter-name.html") && !m_options.askUserForName))
                {
                    string url = uri.GetLeftPart(UriPartial.Authority) + m_gamePath + "controller.html" + uri.Query + uri.Fragment;
                    res.StatusCode = (int)HttpStatusCode.Redirect;
                    res.AddHeader("Location", url);
                    res.ContentType = "text/html";
                    res.WriteContent(System.Text.Encoding.UTF8.GetBytes("<script>window.location.href = decodeURIComponent(\"" + Uri.EscapeDataString(url) + "\");</script>"));
                    m_log.Info("redirect: " + url);
                    return;
                }

                if (path.Equals("/hft/0.x.x/scripts/runtime/live-settings.js"))
                {
                    SendJsonBytes(res, m_liveSettings);
                    return;
                }

                if (path.EndsWith("/controller.html"))
                {
                    byte[] templateData = null;
                    string templatePath = "/hft/0.x.x/templates/controller.index.html";
                    if (!GetGameFile(templatePath, out templateData))
                    {
                        m_log.Error("missing template file: " + templatePath);
                        res.StatusCode = (int)HttpStatusCode.NotFound;
                        return;
                    }
                    string template = System.Text.Encoding.UTF8.GetString(templateData);
                    byte[] controllerData;
                    if (!GetGameFile(path, out controllerData))
                    {
                        m_log.Error("missing file: " + path);
                        res.StatusCode = (int)HttpStatusCode.NotFound;
                        return;
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
    hft: '/hft/0.x.x/scripts',
  },
  shim: {
        '/3rdparty/handjs/hand-1.3.7': {
            //These script dependencies should be loaded before loading
            //hand.js
            deps: [],
            //Once loaded, use the global 'HANDJS' as the
            //module value.
            exports: 'HANDJ',
        },
    },
});
</script>
";
                    subs["content"] = contents;
                    subs["hftSettings"] = m_hftSettingsStr;

                    string s = HFTUtil.ReplaceParamsFlat(template, subs);

                    SendContent(res, path, s);
                    return;
                }


                byte[] content = null;
                if (!GetGameFile(path, out content))
                {
                    res.StatusCode = (int)HttpStatusCode.NotFound;
                    return;
                }

                SendContent(res, path, content);
            };

            m_httpsv.OnOptions += (sender, e) =>
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

            m_httpsv.OnPost += (sender, e) =>
            {
                var req = e.Request;
                var res = e.Response;

                Stream dataStream = req.InputStream;
                StreamReader reader = new StreamReader(dataStream);
                string result = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();

                PostCmd cmd = deserializer_.Deserialize<PostCmd>(result);
                if (cmd.cmd == "happyFunTimesPing")
                {
                    SendJsonBytes(res, m_ping);
                }
                // TODO: use router
            };

            // Not to remove the inactive WebSocket sessions periodically.
            //m_httpsv.KeepClean = false;

            // To resolve to wait for socket in TIME_WAIT state.
            //m_httpsv.ReuseAddress = true;

            // Add the WebSocket services.
            //    m_httpsv.AddWebSocketService<Echo> ("/Echo");
            //    m_httpsv.AddWebSocketService<Chat> ("/Chat");
            m_httpsv.AddWebSocketService<HFTSocket>("/");

            /* Add the WebSocket service with initializing.
            m_httpsv.AddWebSocketService<Chat> (
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
            m_httpsv.Start();
            if (m_httpsv.IsListening)
            {
                m_log.Info(String.Format("Listening on port {0}, and providing WebSocket services:", m_httpsv.Port));
                //foreach (var path in m_httpsv.WebSocketServices.Paths) Debug.Log(String.Format("- {0}", path));
            }
        }

        bool GetGameFile(string path, out byte[] content)
        {
            bool found = HFTWebFileDB.GetInstance().GetFile(path, out content);
            if (!found)
            {
                if (path.StartsWith(m_gamePath))
                {
                    path = path.Substring(m_gamePath.Length - 1);
                    found = HFTWebFileDB.GetInstance().GetFile(path, out content);
                }
            }
            return found;
        }

        void SendJsonBytes(HttpListenerResponse res, byte[] data)
        {
            res.StatusCode = (int)HttpStatusCode.OK;
            res.ContentType = "application/json";
            res.ContentEncoding = Encoding.UTF8;
            res.AddHeader("Access-Control-Allow-Origin", "*");
            res.WriteContent(data);
        }

        void SendContent(HttpListenerResponse res, string path, string content)
        {
            SendContent(res, path, System.Text.Encoding.UTF8.GetBytes(content));
        }

        void SendContent(HttpListenerResponse res, string path, byte[] content)
        {
            string mimeType = HFTMimeType.GetMimeType(path);
            res.ContentType = mimeType;
            if (mimeType.StartsWith("text/") ||
                mimeType == "application/javascript")
            {
                res.ContentEncoding = Encoding.UTF8;
            }
            res.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate"); // HTTP 1.1.
            res.AddHeader("Pragma",        "no-cache");                            // HTTP 1.0.
            res.AddHeader("Expires",       "0");                                   // Proxies.
            res.WriteContent(content);
        }

        public void Stop()
        {
            m_httpsv.Stop();
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
        HFTGameOptions m_options;
        HttpServer m_httpsv;
        HFTLog m_log;
        string m_gamePath;
        byte[] m_ping;
        string m_liveSettingsStr;
        byte[] m_liveSettings;
        string m_hftSettingsStr;
    }

}  // namespace HappyFunTimes

