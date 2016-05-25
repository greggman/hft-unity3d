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

/// <summary>
/// This object represent a Local player, one not on the net.x
/// </summary>
public class LocalNetPlayer : NetPlayer {

    public class Options {
        public string sessionId = "";
        public string name = "LocalPlayer";
    }

    public LocalNetPlayer(GameServer server, Options options = null) : base(server, options != null ? options.name : "LocalPlayer") {
        m_gameHandlers = new Dictionary<string, GameCmdEventHandler>();
        m_sessionId = options != null ? options.sessionId : "";
        m_log = new HFTLog("LocalNetPlayer");  // add name
    }

    public delegate void UntypedGameCmdEventHandler(Dictionary<string, object> data);
    public delegate void TypedGameCmdEventHandler<T>(T eventArgs);

    private class GameCmdConverter<T>
    {
        public GameCmdConverter(TypedGameCmdEventHandler<T> handler) {
            m_handler = handler;
        }

        public void Callback(Deserializer deserializer, object dict) {
            T data = deserializer.Deserialize<T>(dict);
            m_handler(data);
        }

        TypedGameCmdEventHandler<T> m_handler;
    }

    private class UntypedGameCmdConverter {
        public UntypedGameCmdConverter(UntypedGameCmdEventHandler handler) {
            m_handler = handler;
        }

        public void Callback(Deserializer deserializer, object data) {
            string json = Serializer.Serialize(data);
            Dictionary<string, object> dict = (new Deserializer()).Deserialize<Dictionary<string, object> >(json);
            m_handler(dict);
        }

        UntypedGameCmdEventHandler m_handler;
    }

    /// <param name="server">This needs the server because messages need to be queued as they need to be delivered on anther thread</param>.
    private delegate void GameCmdEventHandler(Deserializer deserializer, object data);


    /// <summary>
    /// Lets you register a command to be called when message comes in from this player.
    ///
    /// The callback you register must have a CmdName attribute. That attibute will determine
    /// which event the callback gets dispatched for.
    /// </summary>
    /// <example>
    /// <code>
    /// [CmdName("color")]
    /// <![CDATA[
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
    public void RegisterGameCmdHandler<T>(TypedGameCmdEventHandler<T> callback) where T : MessageCmdData {
        string name = HFTMessageCmdDataNameDB.GetCmdName(typeof(T));
        if (name == null) {
            throw new System.InvalidOperationException("no CmdNameAttribute on " + typeof(T).Name);
        }
        RegisterGameCmdHandler<T>(name, callback);
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
    public void RegisterGameCmdHandler<T>(string name, TypedGameCmdEventHandler<T> callback) {
        GameCmdConverter<T> converter = new GameCmdConverter<T>(callback);
        m_gameHandlers[name] = converter.Callback;
    }

    /// <summary>
    /// <![CDATA[
    /// Lets you register a command to be called when a message is sent from this player.
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
    public void RegisterGameCmdHandler(string name, UntypedGameCmdEventHandler callback) {
        UntypedGameCmdConverter converter = new UntypedGameCmdConverter(callback);
        m_gameHandlers[name] = converter.Callback;
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
    public override void SendCmd(MessageCmdData data) {
        string cmd = HFTMessageCmdDataNameDB.GetCmdName(data.GetType());
        SendCmd(cmd, data);
    }

    public override void SendCmd(string cmd, object data) {
        try {
            GameCmdEventHandler handler;
            if (!m_gameHandlers.TryGetValue(cmd, out handler)) {
                if (m_debug) {
                    m_log.Error("unhandled LocalNetPlayer cmd: " + cmd);
                }
                return;
            }
            handler(m_deserializer, data);
        } catch (Exception ex) {
            m_log.Error(ex);
        }

    }

    public override void SwitchGame(string gameId, object data)
    {
        if (Connected)
        {
            Disconnect();
        }
    }

    public void SendEvent(MessageCmdData data)
    {
        string cmd = HFTMessageCmdDataNameDB.GetCmdName(data.GetType());
        SendEvent(cmd, data);
    }

    public void SendEvent(string cmd, object data)
    {
        HFTMessageCmd msgCmd = new HFTMessageCmd(cmd, data);
        string json = Serializer.Serialize(msgCmd);
        Dictionary<string, object>dict = base.Deserializer.Deserialize<Dictionary<string, object> >(json);
        SendUnparsedEvent(dict);
    }

    public override string GetSessionId() {
        return m_sessionId;
    }

    private Deserializer m_deserializer = new Deserializer();
    private bool m_debug = false;
    private string m_sessionId = "";
    private Dictionary<string, GameCmdEventHandler> m_gameHandlers;  // handlers by command name
    private HFTLog m_log;
};


}

