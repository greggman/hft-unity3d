using System;
using System.Collections.Generic;
using WebSocketSharp.Net;

namespace HappyFunTimes {


    public class HFTCaptivePortalHandler {

        public HFTCaptivePortalHandler(HFTWebServerUtils utils)
        {
            m_log.Tell("Fix hard coded url");
            m_webServerUtils = utils;
            m_appleResponseContent = System.Text.Encoding.UTF8.GetBytes("<HTML><HEAD><TITLE>Success</TITLE></HEAD><BODY>Success</BODY></HTML>");
        }

        public bool HandleRequest(string path, HttpListenerRequest req, HttpListenerResponse res)
        {
            return Check(path, req, res);
        }

        bool Check(string path, HttpListenerRequest req, HttpListenerResponse res)
        {
            m_log.Info("path = " + path);
            string sessionId = System.Uri.EscapeUriString(req.RemoteEndPoint.Address.ToString()) + "_" + System.IO.Path.GetExtension(path);
            m_log.Info("sessionId: " + sessionId);
            bool isCheckingForApple = req.UserAgent.StartsWith("CaptiveNetworkSupport");
            bool isLoginURL = path.Equals("/game-login.html", StringComparison.Ordinal);
            bool isIndexURL = path.Equals("/index.html", StringComparison.Ordinal) ||
                              path.Equals("/", StringComparison.Ordinal)  ||
                              path.Equals(m_firstPath, StringComparison.Ordinal);

            if (isIndexURL)
            {
                m_log.Info("remove session: " + sessionId);
                m_sessions.Remove(sessionId);
                return false;
            }

            Session session = null;
            if (m_sessions.TryGetValue(sessionId, out session))
            {
                m_log.Info("found prev session:" + sessionId);
                if (isLoginURL)
                {
                    session.loggedIn = true;
                    SendCaptivePortalHTML(req, res, sessionId, "/hft/captive-portal/game-login.html");
                    return true;
                }

                // We've seen this device before. Either it's checking that it can connect or it's asking for a normal webpage.
                if (isCheckingForApple)
                {
                    if (session.loggedIn)
                    {
                        m_log.Info("send apple response");
                        m_webServerUtils.SendContent(res, path, m_appleResponseContent);
                        return true;
                    }
                }
                SendCaptivePortalHTML(req, res, sessionId, "/hft/captive-portal/captive-portal.html");
                return true;
            }

            if (!isCheckingForApple)
            {
                m_log.Info("not checking for apple so just fall through");
                return false;
            }

            m_log.Info("send captive-portal.html with new session: " + sessionId);
            // We are checking for apple for the first time so remember the path
            m_sessions[sessionId] = new Session();
            SendCaptivePortalHTML(req, res, sessionId, "/hft/captive-portal/captive-portal.html");
            return true;
        }

        void SendCaptivePortalHTML(HttpListenerRequest req, HttpListenerResponse res, string sessionId, string path)
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
            subs["startUrl"] = m_url + m_firstPath + "?sessionId=" + sessionId;
            str = HFTUtil.ReplaceParamsFlat(str, subs);
            m_log.Info("SCPH: Sending " + path);
            m_webServerUtils.SendContent(res, path, str);
        }

        class Session
        {
            public bool loggedIn = false;
        }

        private HFTLog m_log = new HFTLog("HFTCaptivePortalHandler");
        private HFTWebServerUtils m_webServerUtils;
        private byte[] m_appleResponseContent;
        private string m_url = "http://192.168.2.9:18679";   // FIX           // localaddress:port ?
        private string m_firstPath = "/enter-name.html";     // where to go first after captive portal
        private Dictionary<string, Session> m_sessions = new Dictionary<string, Session>();
    }
}
