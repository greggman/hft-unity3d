
using System.Collections.Generic;
using WebSocketSharp.Net;

namespace HappyFunTimes
{
    public delegate bool RouteHandler(string path, HttpListenerRequest req, HttpListenerResponse res);

    public class HFTRouter
    {
        public bool Route(string path, HttpListenerRequest req, HttpListenerResponse res) {
            for (int i = 0; i < handlers_.Count; ++i)
            {
                if (handlers_[i](path, req, res))
                {
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

