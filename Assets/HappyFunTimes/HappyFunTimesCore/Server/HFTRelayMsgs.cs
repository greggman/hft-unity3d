using System.Collections.Generic;
using System;
using System.IO;

namespace HappyFunTimes
{
    public class HFTRelayToGameMessage
    {
        public HFTRelayToGameMessage(string _cmd, string _id, object _data)
        {
            cmd = _cmd;
            id = _id;
            data = _data;
        }
        public string cmd;
        public string id;  // player id
        public object data;
    }

    public class HFTRelayFromPlayerMessage
    {
        public string cmd;
        public string id;  // player id
        public object data;
    }

    public class HFTRelayToPlayerMessage
    {
        public HFTRelayToPlayerMessage(string _cmd, object _data)
        {
            cmd = _cmd;
            data = _data;
        }
        public string cmd;
        public object data;
    }

    public class HFTCmdMessage
    {
        public HFTCmdMessage(string _cmd, object _data)
        {
            cmd = _cmd;
            data = _data;
        }

        public string cmd;
        public object data;
    }

    public class HFTRedirectCmd
    {
        public HFTRedirectCmd(string _url)
        {
            url = _url;
        }
        public string url;
    }

} // namespace HappyFunTimes


