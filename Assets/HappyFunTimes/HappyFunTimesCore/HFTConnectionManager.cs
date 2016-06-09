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
using System;
using System.Collections;
using System.Collections.Generic;
using HappyFunTimes;

namespace HappyFunTimes
{
    public class HFTConnectionManager
    {
        public GameServer gameServer
        {
            get
            {
                return m_server;
            }
        }

        public void StartHappyFunTimes()
        {
            if (!m_started)
            {
                m_started = true;
                m_hftManager.Start(m_options, m_gameObject);
            }
        }

        public void StopHappyFunTimes()
        {
            if (m_started)
            {
                m_started = false;

                // This has to be done in 2 stages
                // The issue is because things are async let's stay
                // the GameServer disconnects. The server will then see
                // the disconnection and report it (player disconnected, game disconected, etc...)
                // so first we tell both "stop listening for messages/status changes" then
                // we can actually disconnect them,
                // Note this is mostly a matter of stopping superfulous messages in the console
                if (m_server != null)
                {
                    m_server.StopListening();
                }

                if (m_hftManager != null)
                {
                    m_hftManager.StopListening();
                }

                if (m_server != null)
                {
                    m_server.Close();
                }

                if (m_hftManager != null)
                {
                    m_hftManager.Stop();
                }
            }
        }

        void FailedToStart()
        {
            m_log.Error("could not connect to server:");
        }

        void StartGameServer()
        {
            m_server.Init();
        }

        public HFTConnectionManager(GameObject gameObject, HFTGameOptions options)
        {
            m_gameObject = gameObject;
            m_options = new HFTRuntimeOptions(options);

            m_server = new GameServer(m_options, gameObject);
            m_server.OnConnect += Connected;
            m_server.OnDisconnect += Disconnected;

            m_hftManager = new HFTManager();
            m_hftManager.OnReady += StartGameServer;
            m_hftManager.OnFail += FailedToStart;
        }

        void Connected()
        {
        }

        void Disconnected()
        {
        }

        private bool m_started;
        private GameObject m_gameObject;
        private GameServer m_server;
        private HFTManager m_hftManager;
        private HFTLog m_log = new HFTLog("HFTConnectionManager");
        private HFTRuntimeOptions m_options;
    };

}   // namespace HappyFunTimes
