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

using HappyFunTimes;
using System;
using System.Collections;
using UnityEngine;

// NOTE: The HFTPlayerNameManger is NOT part of "HappyFunTimes". It's part of
// the sample-ui used in many of the samples. If you make your own controllers
// based on the sample-ui you're free to use this class to help manage names.
// Otherwise you're free to make your own messages and UI for dealing with names.
public class HFTPlayerNameManager {
    public HFTPlayerNameManager(NetPlayer netPlayer)
    {
        m_netPlayer = netPlayer;
        AddHandlers();
    }

    public void Close()
    {
        RemoveAllHandlers();
    }

    public event Action<string> OnNameChange;
    public event Action<bool> OnBusy;

    public string Name
    {
       get
       {
           return m_name;
       }
    }

    public bool Busy
    {
        get
        {
            return m_busy;
        }
    }

    private NetPlayer m_netPlayer;
    private string m_name;
    private bool m_busy = false;

    // Message when player changes their name.
    private class MessageSetName
    {
        public MessageSetName() {  // needed for deserialization
        }
        public MessageSetName(string _name) {
            name = _name;
        }
        public string name = "";
    }

    // Message then player is busy (on system menu)
    private class MessageBusy {
        public bool busy = false;
    }

    void RemoveAllHandlers() {
        m_netPlayer.UnregisterCmdHandler("_hft_setname_");
        m_netPlayer.UnregisterCmdHandler("_hft_busy_");
        OnNameChange = null;
        OnBusy = null;
    }

    void AddHandlers() {
        m_netPlayer.RegisterCmdHandler<MessageSetName>("_hft_setname_", HandleSetNameMsg);
        m_netPlayer.RegisterCmdHandler<MessageBusy>("_hft_busy_", HandleBusyMsg);
    }

    void HandleSetNameMsg(MessageSetName data) {
        if (data.name.Length > 0 && data.name != m_name) {
            m_name = data.name;

            Action<string> handler = OnNameChange;
            if (handler != null) {
                handler(m_name);
            }
        }
    }

    void HandleBusyMsg(MessageBusy data) {
        if (data.busy != m_busy) {
            m_busy = data.busy;
            Action<bool> handler = OnBusy;
            if (handler != null) {
                handler(m_busy);
            }
        }
    }

}

