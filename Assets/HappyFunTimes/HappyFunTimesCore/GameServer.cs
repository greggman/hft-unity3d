/*
 * Copyright 2014, Gregg Tavares.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are
 * met:
 *
 *     * Redistributions of source code must retain the above copyright
 * notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above
 * copyright notice, this list of conditions and the following disclaimer
 * in the documentation and/or other materials provided with the
 * distribution.
 *     * Neither the name of Gregg Tavares. nor the names of its
 * contributors may be used to endorse or promote products derived from
 * this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 * A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
 * OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
 * LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 * THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using UnityEngine;
using DeJson;
using System;
using System.Linq;
using System.Collections.Generic;
using WebSocketSharp;

namespace HappyFunTimes
{
    // This manages a happyfuntimes "game" and it's players
    public class GameServer
    {
        public delegate void PlayerConnectFunc(PlayerConnectMessageArgs msg);

        public delegate void UntypedCmdEventHandler(Dictionary<string, object> data, string id);
        public delegate void TypedCmdEventHandler<T>(T eventArgs, string id);

        private class CmdConverter<T>
        {
            public CmdConverter(TypedCmdEventHandler<T> handler)
            {
                m_handler = handler;
            }

            public void Callback(Deserializer deserializer, GameServer server, object dict, string id)
            {
                T data = deserializer.Deserialize<T>(dict);
                server.QueueEvent(() =>
                {
                    m_handler(data, id);
                });
            }

            TypedCmdEventHandler<T> m_handler;
        }

        private class UntypedCmdConverter
        {
            public UntypedCmdConverter(UntypedCmdEventHandler handler)
            {
                m_handler = handler;
            }

            public void Callback(Deserializer deserializer, GameServer server, object dict, string id)
            {
                Dictionary<string, object> data = (Dictionary<string, object>)dict;
                server.QueueEvent(() =>
                {
                    // dict is the HFTMessageCmd. We want dict for the MessageCmdData inside the HFTMessageCmd
                    // It might not exist
                    m_handler(data, id);
                });
            }

            UntypedCmdEventHandler m_handler;
        }

        /// <summary>
        /// Constructor for GameServer
        /// </summary>
        /// <param name="options">The objects</param>
        /// <param name="gameObject">gameObject that will process messages from HappyFunTimes</param>
        public GameServer(HFTRuntimeOptions options, GameObject gameObject)
        {
            m_options = options;
            HFTLog.debug = options.debug;
            m_gameObject = gameObject;
            m_players = new Dictionary<string, NetPlayer>();
            m_sendQueue = new List<String>();
            m_deserializer = new Deserializer();
            m_handlers = new Dictionary<string, CmdEventHandler>();

            m_eventProcessor = m_gameObject.AddComponent<HFTEventProcessor>();
            HFTGlobalEventEmitter.GetInstance().Setup(m_eventProcessor);

            m_msgHandlers.Add("update", UpdatePlayer);
            m_msgHandlers.Add("upgame", UpdateGame);
            m_msgHandlers.Add("start", StartPlayer);
            m_msgHandlers.Add("gamestart", StartGame);
            m_msgHandlers.Add("remove", RemovePlayer);
            m_msgHandlers.Add("system", DoSysCommand);
            m_msgHandlers.Add("log", LogMessage);
        }

        /// <summary>
        /// Starts the connection to HappyFunTimes.
        /// </summary>
        public void Init()
        {
            Init(m_options.url);
        }

        /// <summary>
        /// Deperacated
        /// </summary>
        /// <param name="url"></param>
        public void Init(string url/* = "ws://localhost:18679" */)
        {
            m_url = url;
            Connect();
        }

        public void Connect()
        {
            if (m_socket == null)
            {
                m_gotMessages = false;
                m_socket = new WebSocket(m_url);
                StartListening();
                m_socket.Connect();
            }
        }

        public void StartListening()
        {
            if (m_socket == null)
            {
                throw new System.InvalidOperationException("not connected");
            }

            if (!m_listening)
            {
                m_listening = true;
                m_socket.OnOpen += SocketOpened;
                m_socket.OnMessage += SocketMessage;
                m_socket.OnClose += SocketClosed;
                m_socket.OnError += SocketError;
            }
        }

        public void StopListening()
        {
            if (m_listening)
            {
                m_listening = false;
                m_socket.OnOpen -= SocketOpened;
                m_socket.OnMessage -= SocketMessage;
                m_socket.OnClose -= SocketClosed;
                m_socket.OnError -= SocketError;
            }
        }

        public void Close()
        {
            StopListening();

            if (m_socket != null)
            {
                Cleanup();
                m_socket.Close();
                m_socket = null;
            }
        }

        public void QueueEvent(Action action)
        {
            m_eventProcessor.QueueEvent(action);
        }

        /// <summary>
        /// Lets you register a command to be called when the game is directly sent a message.
        ///
        /// The callback you register must have a CmdName attribute. That attibute will determine
        /// which event the callback gets dispatched for.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// [CmdName("color")]
        /// private class MessageColor : MessageCmdData {
        ///     public string color = "";    // in CSS format rgb(r,g,b)
        /// };
        ///
        /// ...
        /// gameServer.RegisterCmdHandler<MessageColor>(OnColor);
        /// ...
        /// void OnColor(MessageColor) {
        ///   Debug.Log("received msg with color: " + color);
        /// }
        /// ]]>
        /// </code>
        /// </example>
        /// <param name="callback">Typed callback</param>
        public void RegisterCmdHandler<T>(TypedCmdEventHandler<T> callback) where T : MessageCmdData
        {
            string name = HFTMessageCmdDataNameDB.GetCmdName(typeof(T));
            if (name == null)
            {
                throw new System.InvalidOperationException("no CmdNameAttribute on " + typeof(T).Name);
            }
            RegisterCmdHandler<T>(name, callback);
        }

        /// <summary>
        /// Lets you register a command to be called when the game is directly sent a message.
        /// </summary>
        /// <example>
        /// <code>
        /// private class MessageColor {
        ///     public string color = "";    // in CSS format rgb(r,g,b)
        /// };
        ///
        /// ...
        /// gameServer.RegisterCmdHandler("color", OnColor);
        /// ...
        /// void OnColor(MessageColor) {
        ///   Debug.Log("received msg with color: " + color);
        /// }
        /// </code>
        /// </example>
        /// <param name="name">command to call callback for</param>
        /// <param name="callback">Typed callback</param>
        public void RegisterCmdHandler<T>(string name, TypedCmdEventHandler<T> callback)
        {
            CmdConverter<T> converter = new CmdConverter<T>(callback);
            m_handlers[name] = converter.Callback;
        }


        /// <summary>
        /// <![CDATA[
        /// Lets you register a command to be called when the game is directly sent a message.
        ///
        /// This the most low-level basic version of RegisterCmdHandler. The function registered
        /// will be called with a Dictionary<string, object> with whatever data is passed in.
        /// You can either pull the data out directly OR you can call Deserializer.Deserilize
        /// with a type and have the data extracted for you.
        /// ]]>
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// ...
        /// gameServer.RegisterCmdHandler("color", OnColor);
        /// ...
        /// void OnColor(Dictionary<string, object> data) {
        ///   Debug.Log("received msg with color: " + data["color"]);
        /// }
        /// ]]>
        /// </code>
        /// or
        /// <code>
        /// <![CDATA[
        /// private class MessageColor {
        ///     public string color = "";    // in CSS format rgb(r,g,b)
        /// };
        /// ...
        /// gameServer.RegisterCmdHandler("color", OnColor);
        /// ...
        /// void OnColor(Dictionary<string, object> data) {
        ///   Deserializer d = new Deserializer();
        ///   MessageColor msg = d.Deserialize<MessageColor>(data);
        ///   Debug.Log("received msg with color: " + msg.color);
        /// }
        /// ]]>
        /// </code>
        /// </example>
        /// <param name="name">command to call callback for</param>
        /// <param name="callback">Typed callback</param>
        public void RegisterCmdHandler(string name, UntypedCmdEventHandler callback)
        {
            UntypedCmdConverter converter = new UntypedCmdConverter(callback);
            m_handlers[name] = converter.Callback;
        }

        /// <param name="server">This needs the server because messages need to be queued as they need to be delivered on anther thread</param>.
        private delegate void CmdEventHandler(Deserializer deserializer, GameServer server, object dict, string id);

        public event PlayerConnectFunc OnPlayerConnect;
        public event Action OnConnect;
        public event Action OnDisconnect;
        public event Action OnConnectFailure;

        /// <summary>
        /// Id of the machine assigned by HappyFunTimes.
        ///
        /// If you're running a multi-machine you can pass an id in the GameServer construtor options.
        /// If you don't pass an id you'll be assigned one. This is the assigned one.
        ///
        /// Note: It is invalid to read this property before the game has connected.
        /// </summary>
        /// <example>
        /// <code>
        /// ...
        /// gameServer.OnConnect += OnConnect;
        /// ...
        /// void OnConnect(EventArgs e) {
        ///   Debug.Log(gameServer.Id);
        /// }
        /// </code>
        /// </example>
        /// <returns>id of machine assigned by HappyFunTimes</returns>
        public string Id
        {
            get
            {
                if (m_id == null)
                {
                    m_log.Error("you can NOT read id before the game has connected");
                }
                return m_id;
            }
            private set
            {
            }
        }

        /// <summary>
        /// Gets the base HTTP url
        /// </summary>
        /// <returns>The base HTTP url. Example "http://localhost:18679"</returns>
        public string GetBaseHttpUrl()
        {
            System.Uri uri = new System.Uri(m_url);
            return "http://" + uri.Host + ":" + uri.Port + "/";
        }

        public class MessageToClient
        {
            public string cmd;  // command 'server', 'update'
            public string id;      // id of client
            public object data;
        };

        private delegate void MessageHandler(MessageToClient msg);

        private class MessageLogMessage
        {
            public string type = "";
            public string msg = "";
        }

        private void SocketOpened(object sender, System.EventArgs e)
        {
            //invoke when socket opened
            m_log.Info("Connnected to HappyFunTimes");
            m_connected = true;

            List<String>.Enumerator i = m_sendQueue.GetEnumerator();
            while (i.MoveNext())
            {
                m_socket.Send(i.Current);
            }
            m_sendQueue.Clear();

            try
            {
                // Inform the relayserver we're a server
                SendSysCmd("server", "-1", m_options);

                #if UNITY_STANDALONE_OSX
                if (m_options.sendFilesToServer)
                {
                    // Send the files
                    HFTWebFileLoader.LoadFiles((string filename, byte[] bytes) => {
                        m_log.Info("Send File: " + filename + ", size: " + bytes.Length);
                        SendSysCmd("addFile", "", new HFTMessageAddFile(filename, System.Convert.ToBase64String(bytes)));
                    });
                }
                #endif

            }
            catch (Exception ex)
            {
                m_log.Error(ex);
            }
        }

        private void SocketClosed(object sender, CloseEventArgs e)
        {
            //invoke when socket closed
            if (m_connected)
            {
                m_log.Tell("Disconnected from HappyFunTimes");
            }
            Cleanup();
        }

        private void SocketMessage(object sender, MessageEventArgs e)
        {
            m_gotMessages = true;
            //invoke when socket message
            if ( e!= null && e.IsText)
            {
                try
                {
                    // Handle ping.
                    if (e.Data == "P")
                    {
                        Send("P");
                        return;
                    }
                    if (m_options.showMessages)
                    {
                        m_log.Tell("r[" + (m_recvCount++) + "] " + e.Data);
                    }
                    MessageToClient m = m_deserializer.Deserialize<MessageToClient>(e.Data);
                    MessageHandler handler = null;
                    if (m_msgHandlers.TryGetValue(m.cmd, out handler))
                    {
                        handler(m);
                    }
                    else
                    {
                        m_log.Error("unknown client message: " + m.cmd);
                    }
                }
                catch (Exception ex)
                {
                    m_log.Error(ex);  // TODO: Add object if possible
                    return;
                }
            }
        }

        private void SocketError(object sender, ErrorEventArgs e)
        {
            if (!m_gotMessages)
            {
                m_log.Tell("Could not connect to HappyFunTimes. Is it running?");
                Close();
                QueueEvent(() =>
                {
                    var handler = OnConnectFailure;
                    if (handler != null)
                    {
                        handler();
                    }
                });
            }
            else
            {
                //invoke when socket error
                m_log.Error("socket error: " + e.Message);
                Cleanup();
            }
        }

        private void Cleanup()
        {
            bool wasConnected = m_connected;
            m_connected = false;

            if (wasConnected)
            {
                QueueEvent(() =>
                {
                    var handler = OnDisconnect;
                    if (handler != null)
                    {
                        handler();
                    }
                });
            }

            foreach(var id in m_players.Keys.ToArray())
            {
                RemovePlayer(id);
            }
        }

        private void StartPlayer(MessageToClient msg)
        {
            if (m_players.ContainsKey(msg.id))
            {
                return;
            }

            string name = "";

            RealNetPlayer.Options options = new RealNetPlayer.Options();
            if (msg.data != null) {
                DeJson.Deserializer deserializer = new DeJson.Deserializer();
                HFTPlayerStartData startData = deserializer.Deserialize<HFTPlayerStartData>(msg.data);
                if (startData != null) {
                    options.sessionId = startData.__hft_session_id__;
                    name = startData.__hft_name__;
                }
            }

            if (System.String.IsNullOrEmpty(name))
            {
                name = "Player" + (++m_totalPlayerCount);
            }

            NetPlayer player = new RealNetPlayer(this, msg.id, name, options);
            m_players[msg.id] = player;
            QueueEvent(() =>
            {
                var handler = OnPlayerConnect;
                if (handler != null)
                {
                    handler(new PlayerConnectMessageArgs(player, name, msg.data));
                }
            });
        }

        private void DoSysCommand(MessageToClient msg)
        {
            // Sys commands don't currently exist in 2.0+ version of HappyFunTimes for Unity
            // m_gameSystem.HandleUnparsedCommand(msg.data);
        }

        private void UpdatePlayer(MessageToClient msg)
        {
            NetPlayer player;
            if (!m_players.TryGetValue(msg.id, out player))
            {
                return;
            }
            player.SendUnparsedEvent(msg.data);
        }

        private void StartGame(MessageToClient msg)
        {
            HFTMessageGameStart data = m_deserializer.Deserialize<HFTMessageGameStart>(msg.data);
            m_id = data.id;
            QueueEvent(() =>
            {
                var handler = OnConnect;
                if (handler != null)
                {
                    handler();
                }
            });
        }

        private void LogMessage(MessageToClient msg)
        {
            MessageLogMessage data = m_deserializer.Deserialize<MessageLogMessage>(msg.data);
            if (data.type == "error")
            {
                m_log.Error(data.msg);
            }
            else
            {
                m_log.Tell(data.msg);
            }
        }

        private void UpdateGame(MessageToClient msg)
        {
            try
            {
                HFTMessageCmd cmd = m_deserializer.Deserialize<HFTMessageCmd>(msg.data);
                CmdEventHandler handler;
                if (!m_handlers.TryGetValue(cmd.cmd, out handler))
                {
                    m_log.Error("unhandled GameServer cmd: " + cmd.cmd);
                    return;
                }
                handler(m_deserializer, this, cmd.data, msg.id);
            }
            catch (Exception ex)
            {
                m_log.Error(ex);
            }
        }

        private void RemovePlayer(MessageToClient msg)
        {
            RemovePlayer(msg.id);
        }

        private void RemovePlayer(string id)
        {
            NetPlayer player;
            if (!m_players.TryGetValue(id, out player))
            {
                return;
            }
            QueueEvent(() =>
            {
                player.Disconnect();
            });
            m_players.Remove(id);
        }

        private void Send(string msg)
        {
            if (m_connected)
            {
                if (m_options.showMessages)
                {
                    m_log.Tell("s[" + (m_sendCount++) + "] " + msg);
                }
                m_socket.Send(msg);
            }
            else
            {
                if (m_options.showMessages)
                {
                    m_log.Tell("q[" + (m_queueCount++) + "] " + msg);
                }
                m_sendQueue.Add(msg);
            }
        }

        public void SendSysCmd(string cmd, string id, object data)
        {
            var msg = new RelayServerCmd(cmd, id, data);
            string json = Serializer.Serialize(msg);
            Send(json);
        }

        // Only NetPlayer should call this.
        public void SendCmd(string cmd, string name, object data, string id = "-1")
        {
            HFTMessageCmd msgCmd = new HFTMessageCmd(name, data);
            SendSysCmd(cmd, id, msgCmd);
        }

        // Only NetPlayer should call this.
        public void SendCmd(string cmd, MessageCmdData data, string id = "-1")
        {
            string name = HFTMessageCmdDataNameDB.GetCmdName(data.GetType());
            SendCmd(cmd, name, data, id);
        }

        public void BroadcastCmd(string cmd, object data)
        {
            SendCmd("broadcast", cmd, data);
        }

        public void BroadcastCmd(MessageCmdData data)
        {
            SendCmd("broadcast", data);
        }

        public void SendCmdToGame(string id, string cmd, object data)
        {
            SendCmd("peer", cmd, data, id);
        }

        public void SendCmdToGame(string id, MessageCmdData data)
        {
            SendCmd("peer", data, id);
        }

        public void BroadcastCmdToGames(string cmd, object data)
        {
            SendCmd("bcastToGames", cmd, data);
        }

        public void BroadcastCmdToGames(MessageCmdData data)
        {
            SendCmd("bcastToGames", data);
        }

        // HFT system data that comes when the player starts
        class HFTPlayerStartData // don't think this needs to inherit from MessageCmdData
        {
            public string __hft_session_id__ = "";
            public string __hft_name__ = "";
        }

        HFTRuntimeOptions m_options;
        bool m_connected = false;
        bool m_listening = false;
        int m_totalPlayerCount = 0;
        int m_recvCount = 0;
        int m_sendCount = 0;
        int m_queueCount = 0;
        bool m_gotMessages = false;
        WebSocket m_socket;
        Dictionary<string, NetPlayer> m_players;
        List<String> m_sendQueue;
        Deserializer m_deserializer;
        GameObject m_gameObject;
        HFTEventProcessor m_eventProcessor;
        Dictionary<string, CmdEventHandler> m_handlers;  // handlers by command name
        string m_id = null;
        string m_url;
        Dictionary<string, MessageHandler> m_msgHandlers = new Dictionary<string, MessageHandler>();
        HFTLog m_log = new HFTLog("GameServer");
    };

}  // namaspace HappyFunTimes
