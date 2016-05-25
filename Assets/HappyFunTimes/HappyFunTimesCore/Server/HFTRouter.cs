
using System.Collections.Generic;
using WebSocketSharp.Net;

namespace HappyFunTimes
{
    public delegate bool RouteHandler(string path, HttpListenerRequest req, HttpListenerResponse res);

    public class HFTRouter
    {
        public bool Route(string path, HttpListenerRequest req, HttpListenerResponse res) {
//UnityEngine.Debug.Log("routing: " + path);
            for (int i = 0; i < handlers_.Count; ++i)
            {
                RouteHandler handler = handlers_[i];
//UnityEngine.Debug.Log("Route Checking: " + handler.Method.Name + " path: " + path);
                if (handler(path, req, res))
                {
//UnityEngine.Debug.Log("handled");
                    return true;
                }
            }
            return false;  // not handled
        }

        public void Add(RouteHandler handler)
        {
            handlers_.Add(handler);
        }

        private List<RouteHandler> handlers_ = new List<RouteHandler>();
    }
}

