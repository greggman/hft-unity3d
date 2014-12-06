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

namespace HappyFunTimes {

public class GameServer {

    public delegate void UntypedCmdEventHandler(Dictionary<string, object> data, string id);
    public delegate void TypedCmdEventHandler<T>(T eventArgs, string id) where T : MessageCmdData;

    private class CmdConverter<T> where T : MessageCmdData
    {
        public CmdConverter(TypedCmdEventHandler<T> handler) {
            m_handler = handler;
        }

        public void Callback(GameServer server, MessageCmdData data, Dictionary<string, object> dict, string id) {
            server.QueueEvent(delegate() {
                m_handler((T)data, id);
            });
        }

        TypedCmdEventHandler<T> m_handler;
    }

    private class UntypedCmdConverter {
        public UntypedCmdConverter(UntypedCmdEventHandler handler) {
            m_handler = handler;
        }

        public void Callback(GameServer server, MessageCmdData data, Dictionary<string, object> dict, string id) {
            server.QueueEvent(delegate() {
                object mcd = null;
                // dict is the MessageCmd. We want dict for the MessageCmdData inside the MessageCmd
                // It might not exist
                dict.TryGetValue("data", out mcd);
                m_handler((Dictionary<string, object>)mcd, id);
            });
        }

        UntypedCmdEventHandler m_handler;
    }

    /// <summary>
    /// Options for the GameServer
    /// </summary>
    public class Options
    {
        public Options()
        {
            cwd = Application.dataPath;
            disconnectPlayersIfGameDisconnects = true;
            url = "ws://localhost:18679";

            // Prefix all HFT arguments with "hft-" so user can filter them out
            ArgParser p = new ArgParser();
            p.TryGet<string>("hft-id", ref id);
            p.TryGet<string>("hft-url", ref url);
            master = p.Contains("hft-master");
        }

        ///<summary>
        /// there's generally no need to set this.
        ///</summary>
        public string gameId;

        ///<summary>
        /// id used for multi-player games. Can be set from command line with --hft=id=someid
        ///</summary>
        public string id;

        ///<summary>
        ///Deprecated and not used.
        ///</summary>
        public string controllerUrl;

        ///<summary>
        ///true allows multiple games to run as the same id. Default: false
        ///
        ///normally when a second game connects the first game will be disconnected
        ///as it's assumed the first game probably crashed or for whatever reason did
        ///not disconnect and this game is taking over. Setting this to true doesn't
        ///disconnect the old game. This is needed for multi-machine games.
        ///</summary>
        public bool allowMultipleGames;   // allow multiple games

        ///<summary>
        ///For a multiple machine game designates this game as the game where players start.
        ///Default: false
        ///Can be set from command line with --hft-master
        ///</summary>
        public bool master;

        ///<summary>
        ///The URL of HappyFunTimes
        ///
        ///Normally you don't need to set this as HappyFunTimes is running on the same machine
        ///as the game. But, for multi-machine games you'd need to tell each machine the address
        ///of the machine running HappyFunTimes. Example: "ws://192.168.2.9:18679".
        ///
        ///Can be set from the command line with --hft-url=someurl
        ///</summary>
        public string url;

        ///<summary>
        ///Normally if a game disconnets all players are also disconnected. This means
        ///they'll auto join the next game running.
        ///Default: true
        ///</summary>
        public bool disconnectPlayersIfGameDisconnects;

        ///<summary>
        ///Prints all the messages in and out to the console.
        ///</summary>
        public bool showMessages;

        ///<summary>
        /// don't set this. it will be set automatically
        ///</summary>
        public string cwd;
    };

    /// <summary>
    /// Constructor for GameServer
    /// </summary>
    /// <param name="options">The objects</param>
    /// <param name="gameObject">gameObject that will process messages from HappyFunTimes</param>
    public GameServer(Options options, GameObject gameObject) {
        m_options = options;
        m_gameObject = gameObject;
        m_players = new Dictionary<string, NetPlayer>();
        m_sendQueue = new List<String>();
        m_deserializer = new Deserializer();
        m_mcdc = new MessageCmdDataCreator();
        m_deserializer.RegisterCreator(m_mcdc);
        m_handlers = new Dictionary<string, CmdEventHandler>();

        m_gameSystem = new GameSystem(this);

        m_eventProcessor = m_gameObject.AddComponent<EventProcessor>();
        m_eventProcessor.Init(this);

        m_gameObject.AddComponent("HFTRunner");
        m_gameObject.SendMessage("HFTInitializeRunner", this);
    }

    /// <summary>
    /// Starts the connection to HappyFunTimes.
    /// </summary>
    public void Init() {
        Init(m_options.url);
    }

    /// <summary>
    /// Deperacated
    /// </summary>
    /// <param name="url"></param>
    public void Init(string url/* = "ws://localhost:18679" */) {
        m_url = url;
        Connect();
    }

    public void Connect()
    {
        if (m_socket == null) {
Debug.Log("GS:Connect");
            m_gotMessages = false;
            m_socket = new WebSocket(m_url);
            m_socket.OnOpen += SocketOpened;
            m_socket.OnMessage += SocketMessage;
            m_socket.OnClose += SocketClosed;
            m_socket.OnError += SocketError;
            m_socket.Connect();
        }
    }

    public void Close() {
        if (m_socket != null) {
            Cleanup();
            m_socket.Close();
            m_socket.OnOpen -= SocketOpened;
            m_socket.OnMessage -= SocketMessage;
            m_socket.OnClose -= SocketClosed;
            m_socket.OnError -= SocketError;
            m_socket = null;
Debug.Log("GS:Close");
        }
    }

    public void QueueEvent(Action action) {
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
    /// </code>
    /// </example>
    /// <param name="callback">Typed callback</param>
    public void RegisterCmdHandler<T>(TypedCmdEventHandler<T> callback) where T : MessageCmdData {
        string name = MessageCmdDataNameDB.GetCmdName(typeof(T));
        if (name == null) {
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
    public void RegisterCmdHandler<T>(string name, TypedCmdEventHandler<T> callback) where T : MessageCmdData {
        CmdConverter<T> converter = new CmdConverter<T>(callback);
        m_handlers[name] = converter.Callback;
        m_mcdc.RegisterCreator(typeof(T));
    }


    /// <summary>
    /// Lets you register a command to be called when the game is directly sent a message.
    ///
    /// This the most low-level basic version of RegisterCmdHandler. The function registered
    /// will be called with a Dictionary<string, object> with whatever data is passed in.
    /// You can either pull the data out directly OR you can call Deserializer.Deserilize
    /// with a type and have the data extracted for you.
    /// </summary>
    /// <example>
    /// <code>
    /// ...
    /// gameServer.RegisterCmdHandler("color", OnColor);
    /// ...
    /// void OnColor(Dictionary<string, object> data) {
    ///   Debug.Log("received msg with color: " + data["color"]);
    /// }
    /// </code>
    /// or
    /// <code>
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
    /// </code>
    /// </example>
    /// <param name="name">command to call callback for</param>
    /// <param name="callback">Typed callback</param>
    public void RegisterCmdHandler(string name, UntypedCmdEventHandler callback) {
        UntypedCmdConverter converter = new UntypedCmdConverter(callback);
        m_handlers[name] = converter.Callback;
        m_mcdc.RegisterCreator(name, typeof(Dictionary<string, object>));
    }

    /// <param name="server">This needs the server because messages need to be queued as they need to be delivered on anther thread</param>.
    private delegate void CmdEventHandler(GameServer server, MessageCmdData cmdData, Dictionary<string, object> dict, string id);

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
    public string Id {
        get {
            if (m_id == null) {
                Debug.LogError("you can NOT read id before the game has connected");
            }
            return m_id;
        }
        private set {
        }
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
    private MessageCmdDataCreator m_mcdc;
    private string m_id = null;
    private string m_url;

    public class MessageToClient {
        public string cmd;  // command 'server', 'update'
        public string id;      // id of client
        public Dictionary<string, object> data;
    };

    private class MessageGameStart {
        public string id = "";
    };

    private class RelayServerCmd {
        public RelayServerCmd(string _cmd, string _id, object _data) {
            cmd = _cmd;
            id = _id;
            data = _data;
        }

        public string cmd;
        public string id;
        public object data;
    }

    private void SocketOpened(object sender, System.EventArgs e) {
        //invoke when socket opened
        Debug.Log("Connnected to HappyFunTimes");
        m_connected = true;

        List<String>.Enumerator i = m_sendQueue.GetEnumerator();
        while (i.MoveNext()) {
            m_socket.Send(i.Current);
        }
        m_sendQueue.Clear();

        // Inform the relayserver we're a server
        try {
            SendSysCmd("server", "-1", m_options);
        } catch (Exception ex) {
            Debug.LogException(ex);
        }
    }

    private void SocketClosed(object sender, CloseEventArgs e) {
        //invoke when socket closed
        Debug.Log("Disconnected from HappyFunTimes");
        Cleanup();
    }

    private void SocketMessage(object sender, MessageEventArgs e) {
        m_gotMessages = true;
        //invoke when socket message
        if ( e!= null && e.Type == Opcode.Text) {
            try {
                // Handle ping.
                if (e.Data == "P") {
                    Send("P");
                    return;
                }
                if (m_options.showMessages) {
                    Debug.Log("r[" + (m_recvCount++) + "] " + e.Data);
                }
                MessageToClient m = m_deserializer.Deserialize<MessageToClient>(e.Data);
                // TODO: make this a dict to callback
                if (m.cmd.Equals("update")) {
                    UpdatePlayer(m.id, m.data);
                } else if (m.cmd.Equals("upgame")) {
                    UpdateGame(m.id, m.data);
                } else if (m.cmd.Equals("start")) {
                    StartPlayer(m.id, "", m.data);
                } else if (m.cmd.Equals("gamestart")) {
                    StartGame(m.id, m.data);
                } else if (m.cmd.Equals("remove")) {
                    RemovePlayer(m.id);
                } else if (m.cmd.Equals("system")) {
                    DoSysCommand(m.data);
                } else {
                    Debug.LogError("unknown client message: " + m.cmd);
                }
            } catch (Exception ex) {
                Debug.LogException(ex);  // TODO: Add object if possible
                return;
            }
        }
    }

    private void SocketError(object sender, ErrorEventArgs e) {
        if (!m_gotMessages) {
            Debug.Log("Could not connect to HappyFunTimes. Is it running?");
            Close();
            QueueEvent(delegate() {
                OnConnectFailure.Emit(this, new EventArgs());
            });
        } else {
            //invoke when socket error
            Debug.Log("socket error: " + e.Message);
            Cleanup();
        }
    }

    private void Cleanup()
    {
        bool wasConnected = m_connected;
        m_connected = false;

        if (wasConnected) {
            QueueEvent(delegate() {
                OnDisconnect.Emit(this, new EventArgs());
            });
        }

        while (m_players.Count > 0) {
            Dictionary<string, NetPlayer>.Enumerator i = m_players.GetEnumerator();
            i.MoveNext();
            RemovePlayer(i.Current.Key);
        }
    }

    private void StartPlayer(string id, string name, Dictionary<string, object> data) {
        if (m_players.ContainsKey(id)) {
            return;
        }

        if (string.IsNullOrEmpty(name)) {
            name = "Player" + (++m_totalPlayerCount);
        }

        NetPlayer player = new RealNetPlayer(this, id);
        m_players[id] = player;
        QueueEvent(delegate() {
            // UGH! This is not thread safe because someone might add handler to OnPlayerConnect
            // Odds are low though?
            OnPlayerConnect.Emit(this, new PlayerConnectMessageArgs(player, name, data));
        });
    }

    private void DoSysCommand(Dictionary<string, object> cmd) {
        m_gameSystem.HandleUnparsedCommand(cmd);
    }

    private void UpdatePlayer(string id, Dictionary<string, object> cmd) {
        NetPlayer player;
        if (!m_players.TryGetValue(id, out player)) {
            return;
        }
        player.SendUnparsedEvent(cmd);
    }

    private void StartGame(string id, Dictionary<string, object> cmd) {
        MessageGameStart data = m_deserializer.Deserialize<MessageGameStart>(cmd);
        m_id = data.id;

        QueueEvent(delegate() {
            OnConnect.Emit(this, new EventArgs());
        });
    }

    private void UpdateGame(string id, Dictionary<string, object> data) {
        try {
            MessageCmd cmd = m_deserializer.Deserialize<MessageCmd>(data);
            CmdEventHandler handler;
            if (!m_handlers.TryGetValue(cmd.cmd, out handler)) {
                Debug.LogError("unhandled GameServer cmd: " + cmd.cmd);
                return;
            }
            handler(this, cmd.data, data, id);
        } catch (Exception ex) {
            Debug.LogException(ex);
        }
    }

    private void RemovePlayer(string id) {
        NetPlayer player;
        if (!m_players.TryGetValue(id, out player)) {
            return;
        }
        QueueEvent(delegate() {
            player.Disconnect();
        });
        m_players.Remove(id);
    }

    private void Send(string msg) {
        if (m_connected) {
            if (m_options.showMessages) {
                Debug.Log("q[" + (m_queueCount++) + "] " + msg);
            }
            m_socket.Send(msg);
        } else {
            if (m_options.showMessages) {
                Debug.Log("s[" + (m_sendCount++) + "] " + msg);
            }
            m_sendQueue.Add(msg);
        }
    }

    public void SendSysCmd(string cmd, string id, object data) {
        var msg = new RelayServerCmd(cmd, id, data);
        string json = Serializer.Serialize(msg);
        Send(json);
    }

    // Only NetPlayer should call this.
    public void SendCmd(string cmd, string name, MessageCmdData data, string id = "-1") {
        MessageCmd msgCmd = new MessageCmd(name, data);
        SendSysCmd(cmd, id, msgCmd);
    }

    // Only NetPlayer should call this.
    public void SendCmd(string cmd, MessageCmdData data, string id = "-1") {
        string name = MessageCmdDataNameDB.GetCmdName(data.GetType());
        SendCmd(cmd, name, data, id);
    }

    public void BroadcastCmd(string cmd, MessageCmdData data) {
        SendCmd("broadcast", cmd, data);
    }

    public void BroadcastCmd(MessageCmdData data) {
        SendCmd("broadcast", data);
    }

    public void SendCmdToGame(string id, string cmd, MessageCmdData data) {
        SendCmd("peer", cmd, data, id);
    }

    public void SendCmdToGame(string id, MessageCmdData data) {
        SendCmd("peer", data, id);
    }

    public void BroadcastCmdToGames(string cmd, MessageCmdData data) {
        SendCmd("bcastToGames", cmd, data);
    }

    public void BroadcastCmdToGames(MessageCmdData data) {
        SendCmd("bcastToGames", data);
    }
};

}
