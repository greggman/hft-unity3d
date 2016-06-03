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
using System.Collections.Generic;
using WebSocketSharp;

namespace HappyFunTimes
{
    public class GameServer
    {
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
                server.QueueEvent(delegate()
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
                server.QueueEvent(delegate()
                {
                    // dict is the MessageCmd. We want dict for the MessageCmdData inside the MessageCmd
                    // It might not exist
                    m_handler(data, id);
                });
            }

            UntypedCmdEventHandler m_handler;
        }

        /// <summary>
        /// Options for the GameServer
        /// </summary>
        public class Options : HFTGameOptions
        {
            public Options()
            {
                cwd = Application.dataPath;
                disconnectPlayersIfGameDisconnects = true;
                url = System.Environment.GetEnvironmentVariable("HFT_URL");
                if (url == null)
                {
                    url = "ws://localhost:18679";
                    startServer = true;
                }

                // Prefix all HFT arguments with "hft-" so user can filter them out
                ArgParser p = new ArgParser();
                p.TryGet<string>("hft-id", ref id);
                p.TryGet<string>("hft-url", ref url);
                master = p.Contains("hft-master");
            }

        };

        /// <summary>
        /// Constructor for GameServer
        /// </summary>
        /// <param name="options">The objects</param>
        /// <param name="gameObject">gameObject that will process messages from HappyFunTimes</param>
        public GameServer(Options options, GameObject gameObject)
        {
            m_options = options;
            m_gameObject = gameObject;
            m_players = new Dictionary<string, NetPlayer>();
            m_sendQueue = new List<String>();
            m_deserializer = new Deserializer();
            m_handlers = new Dictionary<string, CmdEventHandler>();

//            if (String.IsNullOrEmpty(m_options.gameId))
//            {
//                m_options.gameId = "HFTUnity";
//            }

            m_gameSystem = new GameSystem(this);
            HFTInstructions instructions = m_gameObject.AddComponent<HFTInstructions>();
            instructions.Init(m_gameSystem);

            m_eventProcessor = m_gameObject.AddComponent<EventProcessor>();
            m_eventProcessor.Init(this);

            m_msgHandlers.Add("update", UpdatePlayer);
            m_msgHandlers.Add("upgame", UpdateGame);
            m_msgHandlers.Add("start", StartPlayer);
            m_msgHandlers.Add("gamestart", StartGame);
            m_msgHandlers.Add("remove", RemovePlayer);
            m_msgHandlers.Add("system", DoSysCommand);
            m_msgHandlers.Add("log", LogMessage);

//            SendMessageToRunner("HFTInitializeRunner");
        }

        private void SendMessageToRunner(string msg)
        {
            string[] names = new string[]
            {
                "HappyFunTimes.HFTRunner",
                "HappyFunTimes.HFTRunner, Assembly-CSharp-firstpass, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null",            "HFTRunner",
                "HFTRunner",
            };
            foreach (string name in names)
            {
                System.Type t = System.Type.GetType(name);
                if (t != null)
                {
                    object o = m_gameObject.GetComponent(t);
                    if (o == null)
                    {
                        o = m_gameObject.AddComponent(t);
                    }
                    if (o != null)
                    {
                        m_gameObject.SendMessage(msg, this);
                    }
                    break;
                }
            }
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
                m_socket.OnOpen += SocketOpened;
                m_socket.OnMessage += SocketMessage;
                m_socket.OnClose += SocketClosed;
                m_socket.OnError += SocketError;
                m_socket.Connect();
            }
        }

        public void Close()
        {
            if (m_socket != null)
            {
                Cleanup();
                m_socket.Close();
                m_socket.OnOpen -= SocketOpened;
                m_socket.OnMessage -= SocketMessage;
                m_socket.OnClose -= SocketClosed;
                m_socket.OnError -= SocketError;
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
            string name = MessageCmdDataNameDB.GetCmdName(typeof(T));
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
        /// private class MessageColor : MessageCmdData {
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
        /// private class MessageColor : MessageCmdData {
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

        public event EventHandler<PlayerConnectMessageArgs> OnPlayerConnect;
        public event EventHandler<EventArgs> OnConnect;
        public event EventHandler<EventArgs> OnDisconnect;
        public event EventHandler<EventArgs> OnConnectFailure;

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
                    Debug.LogError("you can NOT read id before the game has connected");
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

        private Options m_options;
        private bool m_connected = false;
        private int m_totalPlayerCount = 0;
        private int m_recvCount = 0;
        private int m_sendCount = 0;
        private int m_queueCount = 0;
        private bool m_gotMessages = false;
        private WebSocket m_socket;
        private Dictionary<string, NetPlayer> m_players;
        private List<String> m_sendQueue;
        private Deserializer m_deserializer;
        private GameObject m_gameObject;
        private EventProcessor m_eventProcessor;
        private GameSystem m_gameSystem;
        private Dictionary<string, CmdEventHandler> m_handlers;  // handlers by command name
        private string m_id = null;
        private string m_url;
        private Dictionary<string, MessageHandler> m_msgHandlers = new Dictionary<string, MessageHandler>();

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
            Debug.Log("Connnected to HappyFunTimes");
            m_connected = true;

            List<String>.Enumerator i = m_sendQueue.GetEnumerator();
            while (i.MoveNext())
            {
                m_socket.Send(i.Current);
            }
            m_sendQueue.Clear();

            // Inform the relayserver we're a server
            try
            {
                SendSysCmd("server", "-1", m_options);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private void SocketClosed(object sender, CloseEventArgs e)
        {
            //invoke when socket closed
            if (m_connected)
            {
                Debug.Log("Disconnected from HappyFunTimes");
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
                        Debug.Log("r[" + (m_recvCount++) + "] " + e.Data);
                    }
                    MessageToClient m = m_deserializer.Deserialize<MessageToClient>(e.Data);
                    MessageHandler handler = null;
                    if (m_msgHandlers.TryGetValue(m.cmd, out handler))
                    {
                        handler(m);
                    }
                    else
                    {
                        Debug.LogError("unknown client message: " + m.cmd);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);  // TODO: Add object if possible
                    return;
                }
            }
        }

        private void SocketError(object sender, ErrorEventArgs e)
        {
            if (!m_gotMessages)
            {
                Debug.Log("Could not connect to HappyFunTimes. Is it running?");
                Close();
                QueueEvent(delegate()
                {
                    if (OnConnectFailure != null)
                    {
                        OnConnectFailure.Emit(this, new EventArgs());
                    }
                });
            }
            else
            {
                //invoke when socket error
                Debug.Log("socket error: " + e.Message);
                Cleanup();
            }
        }

        private void Cleanup()
        {
            bool wasConnected = m_connected;
            m_connected = false;

            if (wasConnected)
            {
                QueueEvent(delegate()
                {
                    if (OnDisconnect != null)
                    {
                        OnDisconnect.Emit(this, new EventArgs());
                    }
                });
            }

            while (m_players.Count > 0)
            {
                Dictionary<string, NetPlayer>.Enumerator i = m_players.GetEnumerator();
                i.MoveNext();
                RemovePlayer(i.Current.Key);
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
            QueueEvent(delegate()
            {
                // UGH! This is not thread safe because someone might add handler to OnPlayerConnect
                // Odds are low though?
                if (OnPlayerConnect != null)
                {
                    OnPlayerConnect.Emit(this, new PlayerConnectMessageArgs(player, name, msg.data));
                }
            });
        }

        private void DoSysCommand(MessageToClient msg)
        {
            m_gameSystem.HandleUnparsedCommand(msg.data);
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
            MessageGameStart data = m_deserializer.Deserialize<MessageGameStart>(msg.data);
            m_id = data.id;
            if (data.needNewHFT)
            {
                QueueEvent(delegate()
                {
                    Debug.LogError("This game needs a newer version of HappyFunTimes");
                    SendMessageToRunner("HFTNeedNewHFT");
                    Application.Quit();
                });
            }
            else
            {
                QueueEvent(delegate()
                {
                    if (m_options.uploadControllerFiles)
                    {
                        HFTUploader uploader = new HFTUploader(false);
                        string baseFolder = System.IO.Path.Combine(System.IO.Path.Combine(Application.dataPath, "WebPlayerTemplates"), "HappyFunTimes");
                        string url = GetBaseHttpUrl() + "api/v0/uploadFile/";
                        uploader.UploadTree(url, data.gameId, baseFolder);

                    }
                    if (OnConnect != null)
                    {
                        OnConnect.Emit(this, new EventArgs());
                    }
                });
            }
        }

        private void LogMessage(MessageToClient msg)
        {
            MessageLogMessage data = m_deserializer.Deserialize<MessageLogMessage>(msg.data);
            if (data.type == "error")
            {
                Debug.LogError(data.msg);
            }
            else
            {
                Debug.Log(data.msg);
            }
        }

        private void UpdateGame(MessageToClient msg)
        {
            try
            {
                MessageCmd cmd = m_deserializer.Deserialize<MessageCmd>(msg.data);
                CmdEventHandler handler;
                if (!m_handlers.TryGetValue(cmd.cmd, out handler))
                {
                    Debug.LogError("unhandled GameServer cmd: " + cmd.cmd);
                    return;
                }
                handler(m_deserializer, this, cmd.data, msg.id);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
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
            QueueEvent(delegate()
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
                    Debug.Log("q[" + (m_queueCount++) + "] " + msg);
                }
                m_socket.Send(msg);
            }
            else
            {
                if (m_options.showMessages)
                {
                    Debug.Log("s[" + (m_sendCount++) + "] " + msg);
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
            MessageCmd msgCmd = new MessageCmd(name, data);
            SendSysCmd(cmd, id, msgCmd);
        }

        // Only NetPlayer should call this.
        public void SendCmd(string cmd, MessageCmdData data, string id = "-1")
        {
            string name = MessageCmdDataNameDB.GetCmdName(data.GetType());
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
        class HFTPlayerStartData : MessageCmdData // don't think this needs to inherit from MessageCmdData
        {
            public string __hft_session_id__ = "";
            public string __hft_name__ = "";
        }

    };

}  // namaspace HappyFunTimes
