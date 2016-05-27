using System;

namespace HappyFunTimes
{
    public class HFTRuntimeOptions : HFTArgsDirect
    {
        public HFTRuntimeOptions(string prefix) : base(prefix)
        {
        }

        public string dataPath = "";
        public string url = "";
        public string id = "";
        public string name = "";
        public string gameId = "HFTUnity";  // this is kind of left over from when one server supported mutiple games
        public string controllerFilename = "";
        public bool disconnectPlayersIfGameDisconnects = true;
        public bool installationMode = false;
        public bool master = false;
        public bool showInList = true;
        public bool showMessages;
        public bool debug;
        public bool startServer;
        public bool dns;
        public bool captivePortal;
        public string serverPort = "";
        public string rendezvousUrl;
    }
}

