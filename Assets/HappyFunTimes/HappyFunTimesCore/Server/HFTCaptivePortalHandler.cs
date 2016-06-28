using System;
using System.Collections.Generic;
using WebSocketSharp.Net;

namespace HappyFunTimes {


    public class HFTCaptivePortalHandler {

        public HFTCaptivePortalHandler(HFTWebServerUtils utils)
        {
            m_webServerUtils = utils;
            m_appleResponseContent = System.Text.Encoding.UTF8.GetBytes("<HTML><HEAD><TITLE>Success</TITLE></HEAD><BODY>Success</BODY></HTML>");
        }

        public bool HandleRequest(string path, HttpListenerRequest req, HttpListenerResponse res)
        {
            if (path.Equals("/generate_204"))
            {
                return m_webServerUtils.SendContent(res, path, "");
            }

            if (path.Equals("/xtra2.bin"))
            {
                return m_webServerUtils.SendFile("/hft/captive-portal/xtra2.bin", req, res);
            }

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
                    return SendCaptivePortalHTML(req, res, sessionId, "/hft/captive-portal/game-login.html");
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
                return SendCaptivePortalHTML(req, res, sessionId, "/hft/captive-portal/captive-portal.html");
            }

            if (!isCheckingForApple)
            {
                m_log.Info("not checking for apple so just fall through");
                return false;
            }

            m_log.Info("send captive-portal.html with new session: " + sessionId);
            // We are checking for apple for the first time so remember the path
            m_sessions[sessionId] = new Session();
            return SendCaptivePortalHTML(req, res, sessionId, "/hft/captive-portal/captive-portal.html");
        }

        string GetBaseUrl(HttpListenerRequest req)
        {
            System.Net.IPEndPoint localEndPoint = req.LocalEndPoint;
            return (req.IsSecureConnection ? "https://" : "http://")
                + (localEndPoint.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6
                    ? ("[" + localEndPoint.Address.ToString() + "]")
                    : localEndPoint.Address.ToString())
                + ":" + localEndPoint.Port.ToString();
        }

        bool SendCaptivePortalHTML(HttpListenerRequest req, HttpListenerResponse res, string sessionId, string path)
        {
            //var fullPath = path.normalize(path.join(this.options.baseDir, opt_path));
            byte[] content = null;
            if (!m_webServerUtils.GetGameFile(path, out content))
            {
                res.StatusCode = (int)HttpStatusCode.NotFound;
                return true;
            }

            string str = System.Text.Encoding.UTF8.GetString(content);
            Dictionary<string, string> subs = new Dictionary<string, string>();
            subs["startUrl"] = GetBaseUrl(req) + m_firstPath + "?sessionId=" + sessionId;
            subs["sessionId"] = sessionId;
            str = HFTUtil.ReplaceParamsFlat(str, subs);
            m_log.Info("SCPH: Sending " + path);
            return m_webServerUtils.SendContent(res, path, str);
        }

        class Session
        {
            public bool loggedIn = false;
        }

        private HFTLog m_log = new HFTLog("HFTCaptivePortalHandler");
        private HFTWebServerUtils m_webServerUtils;
        private byte[] m_appleResponseContent;
        private string m_firstPath = "/enter-name.html";     // where to go first after captive portal
        private Dictionary<string, Session> m_sessions = new Dictionary<string, Session>();
    }
}
