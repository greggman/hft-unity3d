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

namespace HappyFunTimes {

public abstract class NetPlayer
{
    public delegate void UntypedCmdEventHandler(Dictionary<string, object> data);
    public delegate void TypedCmdEventHandler<T>(T eventArgs);

    private class CmdConverter<T>
    {
        public CmdConverter(TypedCmdEventHandler<T> handler) {
            m_handler = handler;
        }

        public void Callback(Deserializer deserializer, GameServer server, object dict) {
            T data = deserializer.Deserialize<T>(dict);
            server.QueueEvent(() => {
                m_handler((T)data);
            });
        }

        TypedCmdEventHandler<T> m_handler;
    }

    private class UntypedCmdConverter {
        public UntypedCmdConverter(UntypedCmdEventHandler handler) {
            m_handler = handler;
        }

        public void Callback(Deserializer deserializer, GameServer server, object dict) {
            Dictionary<string, object> data = (Dictionary<string, object>)dict;
            server.QueueEvent(() => {
                m_handler(data);
            });
        }

        UntypedCmdEventHandler m_handler;
    }

    /// <param name="server">This needs the server because messages need to be queued as they need to be delivered on anther thread</param>.
    private delegate void CmdEventHandler(Deserializer deserializer, GameServer server, object dict);

    public NetPlayer(GameServer server, string name)
    {
        m_server = server;
        m_connected = true;
        m_handlers = new Dictionary<string, CmdEventHandler>();
        m_internalHandlers = new Dictionary<string, CmdEventHandler>();
        m_deserializer = new Deserializer();
        m_log = new HFTLog("NetPlayer[" + name + "]");

        AddHandlers();
    }

    /// <summary>
    /// Lets you register a command to be called when message comes in from this player.
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
    /// netPlayer.RegisterCmdHandler<MessageColor>(OnColor);
    /// ...
    /// void OnColor(MessageColor) {
    ///   Debug.Log("received msg with color: " + color);
    /// }
    /// ]]>
    /// </code>
    /// </example>
    /// <param name="callback">Typed callback</param>
    public void RegisterCmdHandler<T>(TypedCmdEventHandler<T> callback) where T : MessageCmdData {
        string name = HFTMessageCmdDataNameDB.GetCmdName(typeof(T));
        if (name == null) {
            throw new System.InvalidOperationException("no CmdNameAttribute on " + typeof(T).Name);
        }
        RegisterCmdHandler<T>(name, callback);
    }

    /// <summary>
    /// Lets you register a command to be called when a message comes in from this player.
    /// </summary>
    /// <example>
    /// <code>
    /// private class MessageColor {
    ///     public string color = "";    // in CSS format rgb(r,g,b)
    /// };
    ///
    /// ...
    /// newPlayer.RegisterCmdHandler("color", OnColor);
    /// ...
    /// void OnColor(MessageColor) {
    ///   Debug.Log("received msg with color: " + color);
    /// }
    /// </code>
    /// </example>
    /// <param name="name">command to call callback for</param>
    /// <param name="callback">Typed callback</param>
    public void RegisterCmdHandler<T>(string name, TypedCmdEventHandler<T> callback) {
        CmdConverter<T> converter = new CmdConverter<T>(callback);
        m_handlers[name] = converter.Callback;
    }

    private void RegisterInternalCmdHandler<T>(string name, TypedCmdEventHandler<T> callback) {
        CmdConverter<T> converter = new CmdConverter<T>(callback);
        m_internalHandlers[name] = converter.Callback;
    }

    /// <summary>
    /// Lets you register a command to be called when a message is sent from this player.
    ///
    /// This the most low-level basic version of RegisterCmdHandler. The function registered
    /// will be called with a Dictionary&lt;string, object&gt; with whatever data is passed in.
    /// You can either pull the data out directly OR you can call Deserializer.Deserilize
    /// with a type and have the data extracted for you.
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// ...
    /// netPlayer.RegisterCmdHandler("color", OnColor);
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
    public void RegisterCmdHandler(string name, UntypedCmdEventHandler callback) {
        UntypedCmdConverter converter = new UntypedCmdConverter(callback);
        m_handlers[name] = converter.Callback;
    }

    /// <summary>
    /// Unregisters the command handler for a given MessageCmdData. Gets the Command name from the MessgeCmdData's
    /// CmdData attribute.
    /// </summary>
    /// <returns>true if there was a command handler, false if not</returns>
    public bool UnregisterCmdHandler<T>() where T : MessageCmdData {
        string name = HFTMessageCmdDataNameDB.GetCmdName(typeof(T));
        return UnregisterCmdHandler(name);
    }

    /// <summary>
    /// Unregisters the command handler for the specified command
    /// </summary>
    /// <param name="name"></param>
    /// <returns>true if there was a command handler, false if not</returns>
    public bool UnregisterCmdHandler(string name) {
        return m_handlers.Remove(name);
    }

    /// <summary>
    /// Removes all handlers
    /// </summary>
    public void RemoveAllHandlers() {
        OnDisconnect = null;
        m_handlers.Clear();
    }

    void AddHandlers() {
        RegisterInternalCmdHandler<HFTMessageLog>("_hft_log_", HandleLogMsg);
    }

    /// <summary>
    /// Sends a message to this player's phone
    /// </summary>
    /// <param name="data">The message. It must be derived from MessageCmdData and must have a
    /// CmdName attribute</param>
    /// <example>
    /// <code>
    /// </code>
    /// </example>
    public abstract void SendCmd(MessageCmdData data);

    public abstract void SendCmd(string cmd, object data);

    public void SendCmd(string cmd) {
        SendCmd(cmd, null);
    }

    public virtual void Disconnect()
    {
        m_connected = false;

        EventHandler<EventArgs> handler = OnDisconnect;
        if (handler != null) {
            handler(this, new EventArgs());
        }
        RemoveAllHandlers();
    }

    public abstract void SwitchGame(string gameId, object data);

    public void SendUnparsedEvent(object data)
    {
        if (!m_connected)
        {
            return;
        }

        // This is kind of round about. The issue is we queue message
        // if there are no handlers as that means no one has had time
        // to register any and those message will be lost.
        //
        // That's great but we can also call RemoveAllHanders. PlayerConnector
        // does this. Players that are waiting have all messages disconnected.
        // That means if they are waiting for 2-3 mins, with a poorly designed
        // controller there could be tons of messages queued up.
        //
        // So, only allow queuing messages once. After that they're never
        // queued.
        if (m_handlers.Count > 0) {
            m_haveHandlers = true;
        }

        // If there are no handlers registered then the object using this NetPlayer
        // has not been instantiated yet. The issue is the GameSever makes a NetPlayer.
        // It then has to queue an event to start that player so that it can be started
        // on another thread. But, before that event has triggered other messages might
        // come through. So, if there are no handlers then we add an event to run the
        // command later. It's the same queue that will birth the object that needs the
        // message.
        if (!m_haveHandlers) {
            m_server.QueueEvent(() => {
                SendUnparsedEvent(data);
            });
            return;
        }

        try {
            HFTMessageCmd cmd = m_deserializer.Deserialize<HFTMessageCmd>(data);
            CmdEventHandler handler;
            if (!m_handlers.TryGetValue(cmd.cmd, out handler)) {
                if (!m_internalHandlers.TryGetValue(cmd.cmd, out handler)) {
                    m_log.Error("unhandled NetPlayer cmd: " + cmd.cmd);
                    return;
                }
            }
            handler(m_deserializer, m_server, cmd.data);
        } catch (Exception ex) {
            m_log.Error(ex);
        }
    }

    private string errorStr = @"error";
    void HandleLogMsg(HFTMessageLog data) {
        if (errorStr.Equals(data.type, StringComparison.Ordinal)) {
            m_log.Error(data.msg);
        } else {
            m_log.Tell(data.msg);
        }
    }

    public abstract string GetSessionId();
    public string Name {
        set {
            m_log.prefix = "NetPlayer[" + value + "]";
        }
    }

    public event EventHandler<EventArgs> OnDisconnect;

    // We need to separate m_internalHandlers because we check if
    // m_handlers.count is 0. If it is we assume no one has had
    // a chance to add any handlers and we queue the messages
    // instead of processing them. If we registered internal handlers
    // to m_handlers we'd couldn't check that.
    private Dictionary<string, CmdEventHandler> m_handlers;  // handlers by command name
    private Dictionary<string, CmdEventHandler> m_internalHandlers;  // handlers by command name
    private Deserializer m_deserializer;
    private bool m_connected;
    private bool m_haveHandlers = false;
    private GameServer m_server;
    private HFTLog m_log;

    protected Deserializer Deserializer {
        get {
            return m_deserializer;
        }
    }

    protected bool Connected {
        get {
            return m_connected;
        }
    }

    protected GameServer Server {
        get {
            return m_server;
        }
    }
}

}

