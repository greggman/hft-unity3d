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
/// This object represent a Local player, one not on the net.
/// </summary>
public class LocalNetPlayer : NetPlayer {

    public LocalNetPlayer(GameServer server) : base(server) {
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
        // No-op because this is a local player.
    }

    public override void SendCmd(string cmd, MessageCmdData data) {
        // No-op because this is a local player.
    }

    public override void SwitchGame(string gameId, MessageCmdData data)
    {
        if (Connected)
        {
            Disconnect();
        }
    }

    public void SendEvent(MessageCmdData data)
    {
        string cmd = MessageCmdDataNameDB.GetCmdName(data.GetType());
        SendEvent(cmd, data);
    }

    public void SendEvent(string cmd, MessageCmdData data)
    {
        MessageCmd msgCmd = new MessageCmd(cmd, data);
        string json = Serializer.Serialize(msgCmd);
        Dictionary<string, object>dict = base.Deserializer.Deserialize<Dictionary<string, object> >(json);
        SendUnparsedEvent(dict);
    }
};


}

