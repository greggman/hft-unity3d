using System;
using System.Collections.Generic;
using WebSocketSharp.Net;


namespace HappyFunTimes {


    public class HFTCaptivePortalHandler {

        public HFTCaptivePortalHandler(HFTWebServerUtils utils)
        {
            m_webServerUtils = utils;
        }

        public bool HandleRequest(string path, HttpListenerRequest req, HttpListenerResponse res)
        {
            if (!Check(path, req, res)) {
                return false;
            }
            return false;
        }

        bool Check(string path, HttpListenerRequest req, HttpListenerResponse res)
        {
            string sessionId = path;
            bool isCheckingForApple = req.UserAgent.StartsWith("CaptiveNetworkSupport");
            bool isLoginURL = path.Equals("/game-login.html", StringComparison.Ordinal);
            bool isIndexURL = path.Equals("/index.html", StringComparison.Ordinal) ||
                              path.Equals("/", StringComparison.Ordinal)  ||
                              path.Equals(m_firstPath, StringComparison.Ordinal);

            if (isIndexURL)
            {
                sessionId = req.QueryString.Get("sessionId");
                if (sessionId != null)
                {
                    m_sessions.Remove(sessionId);
                }
                return false;
            }

            if (isLoginURL && req.UrlReferrer != null)
            {
                sessionId = req.UrlReferrer.AbsolutePath;
            }

            Session session = null;
            if (m_sessions.TryGetValue(sessionId, out session))
            {
                if (isLoginURL)
                {
                    session.loggedIn = true;
                    SendCaptivePortalHTML(req, res, sessionId, "game-login.html");
                    return true;
                }

                // We've seen this device before. Either it's checking that it can connect or it's asking for a normal webpage.
                if (isCheckingForApple)
                {
                    if (session.loggedIn)
                    {
                        //                  res.status(200).send("<HTML><HEAD><TITLE>Success</TITLE></HEAD><BODY>Success</BODY></HTML>");
                        return true;
                    }
                }
                SendCaptivePortalHTML(req, res, sessionId);
                return true;
            }

            if (!isCheckingForApple)
            {
                return false;
            }

            // We are checking for apple for the first time so remember the path
            m_sessions[sessionId] = new Session();
            SendCaptivePortalHTML(req, res, sessionId);
            return true;
        }

        void SendCaptivePortalHTML(HttpListenerRequest req, HttpListenerResponse res, string sessionId, string path = "captive-portal.html")
        {
            //var fullPath = path.normalize(path.join(this.options.baseDir, opt_path));
            byte[] content = null;
            if (!m_webServerUtils.GetGameFile(path, out content))
            {
                res.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            string str = System.Text.Encoding.UTF8.GetString(content);
            Dictionary<string, string> subs = new Dictionary<string, string>();
            subs["sessionId"] = sessionId;
            subs["locahost"] = m_url; // was this.options.address + ":" + this.options.port
            subs["firstPath"] = m_firstPath;
            str = HFTUtil.ReplaceParamsFlat(str, subs);
            m_webServerUtils.SendContent(res, path, str);
        }

        class Session
        {
            public bool loggedIn = false;
        }

        private HFTWebServerUtils m_webServerUtils;
        private string m_url = "http://192.168.2.9:18679";   // FIX           // localaddress:port ?
        private string m_firstPath = "/enter-name.html";     // where to go first after captive portal
        private Dictionary<string, Session> m_sessions = new Dictionary<string, Session>();
    }
}
