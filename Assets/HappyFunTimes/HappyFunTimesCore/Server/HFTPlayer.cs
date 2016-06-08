using DeJson;
using System.Collections.Generic;
using System;
using System.Text;

namespace HappyFunTimes
{
    public class HFTPlayer {

        public string id {
            get {
                return id_;
            }
        }

        public delegate void UntypedCmdEventHandler(object data);
        public delegate void TypedCmdEventHandler<T>(T eventArgs);
        private delegate bool CmdEventHandler(Deserializer deserializer, object data);

        private class AddPlayerToGameMessage
        {
            public string gameId = "";
            public Dictionary<string, object> data = null;
        }

        private class CmdConverter<T>
        {
            public CmdConverter(HFTLog log, TypedCmdEventHandler<T> handler)
            {
                m_log = log;
                m_handler = handler;
            }

            public bool Callback(Deserializer deserializer, object data) {
                try
                {
                    T t = deserializer.Deserialize<T>(data);
                    m_handler(t);
                    return true;
                }
                catch (System.Exception ex)
                {
                    m_log.Error(ex.ToString());
                    return false;
                }
            }

            HFTLog m_log;
            TypedCmdEventHandler<T> m_handler;
        }

        private class UntypedCmdConverter
        {
            public UntypedCmdConverter(UntypedCmdEventHandler handler)
            {
                m_handler = handler;
            }

            public void Callback(Deserializer deserializer, object data)
            {
                m_handler(data);
            }

            UntypedCmdEventHandler m_handler;
        }

        public HFTPlayer(HFTSocket client, HFTGameManager server, string id)
        {
            client_ = client;
            gameManager_ = server;
            id_ = id;

            log_ = new HFTLog("HFTPlayer[" + id + "]");
            log_.Info("start player");

            client.OnMessageEvent += HandleMessage;
            client.OnCloseEvent   += HandleDisconnect;

            RegisterCmdHandler<AddPlayerToGameMessage>("join", AddPlayerToGame);
            RegisterCmdHandler<HFTRuntimeOptions>("server", AssignAsServerForGame);
            RegisterCmdHandler<object>("update", PassMessageFromPlayerToGame);

        }

        public void RegisterCmdHandler<T>(string name, TypedCmdEventHandler<T> callback) {
            CmdConverter<T> converter = new CmdConverter<T>(log_, callback);
            messageHandlers_[name] = converter.Callback;
        }

        void AddPlayerToGame(AddPlayerToGameMessage data)
        {
            var game = gameManager_.AddPlayerToGame(this, data.gameId, data.data);
            if (game == null)
            {
                log_.Error("game does not exist:" + data.gameId);
                SendCmd("_hft_redirect_", new HFTRedirectCmd("/"));
            }
        }

        public void SetGame(HFTGame game)
        {
            game_ = game;
        }

        void AssignAsServerForGame(HFTRuntimeOptions data)  // ???????????????????
        {
            client_.OnMessageEvent -= HandleMessage;
            client_.OnCloseEvent   -= HandleDisconnect;
            gameManager_.AssignAsClientForGame(data, client_);
        }

        void PassMessageFromPlayerToGame(object data) {
            if (game_ == null)
            {
                log_.Warn("player has no game");
                return;
            }
            game_.Send(this, new HFTRelayToGameMessage("update", id_, data));
        }

        void HandleMessage(object sender, WebSocketSharp.MessageEventArgs e)
        {
            HFTRelayFromPlayerMessage message = null;
            try
            {
                message = deserializer_.Deserialize<HFTRelayFromPlayerMessage>(e.Data);

            }
            catch (System.Exception ex)
            {
                log_.Error(ex.ToString());
                return;
            }

            string cmd = message.cmd;
            CmdEventHandler handler = null;
            if (!messageHandlers_.TryGetValue(cmd, out handler))
            {
              log_.Error("unknown player message: " + cmd);
              return;
            }

            handler(deserializer_, message.data);
        }

        void HandleDisconnect(object sender, WebSocketSharp.CloseEventArgs e)
        {
            log_.Info("disconnected");
            if (game_ != null)
            {
                game_.RemovePlayer(this);
            }
            this.Disconnect();
        }

        public void Send(object msg)
        {
            try
            {
                client_.Send(msg);
            }
            catch (System.Exception ex)
            {
                log_.Error("error sending to client");
                log_.Error(ex.ToString());
                log_.Error("disconnecting");
                Disconnect();
            }
        }
        void SendCmd(string cmd, object data)
        {
            Send(new HFTRelayToPlayerMessage(cmd, data));
        }

        void SendToGame(HFTRelayToGameMessage msg)
        {
            if (game_ != null)
            {
                game_.Send(this, msg);
            }
        }

        public void Disconnect()
        {
            client_.OnMessageEvent -= HandleMessage;
            client_.OnCloseEvent -= HandleDisconnect;
            log_.Info("Disconnect");
            client_.Close();
        }

        Deserializer deserializer_ = new Deserializer();
        Dictionary<string, CmdEventHandler> messageHandlers_ = new Dictionary<string, CmdEventHandler>();
        HFTSocket client_ = null;
        HFTGameManager gameManager_ = null;
        HFTGame game_ = null;
        HFTLog log_;
        string id_ = "";
    }

}  // namespace HappyFunTimes

