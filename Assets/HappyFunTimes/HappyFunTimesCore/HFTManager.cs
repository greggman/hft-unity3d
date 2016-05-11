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
using System.Collections.Generic;

namespace HappyFunTimes
{
    public class HFTManager {

        public event EventHandler<EventArgs> OnReady;
        public event EventHandler<EventArgs> OnFail;

        public HFTManager()
        {
        }

        public void Start(HFTRuntimeOptions options, GameObject gameObject)
        {
            m_options = options;
            m_gameObject = gameObject;

            string hftUrl = System.Environment.GetEnvironmentVariable("HFT_URL");
            if (hftUrl == null && options.startServer)
            {
                StartServer();
            }

            StartCheck();
        }

        void StartCheck()
        {
            m_check = m_gameObject.AddComponent<HFTCheck>();
            m_check.Init(m_options.url, Ready, Failed);
        }

        void CleanupCheck()
        {
            if (m_check != null)
            {
                Component.Destroy(m_check);
                m_check = null;
            }
        }

        public void Stop()
        {
            StopServer();
        }

        void Ready()
        {
            CleanupCheck();

            EventHandler<EventArgs> handler = OnReady;
            if (handler != null) {
                handler(this, new EventArgs());
            }
        }

        void Failed()
        {
            CleanupCheck();
            EventHandler<EventArgs> handler = OnFail;
            if (handler != null) {
                handler(this, new EventArgs());
            }
        }

        public void StartServer()
        {
            var hftOptions = new HFTSite.Options();
            //hftOptions.port = ??
            HFTUtil.SetIfNotNullOrEmpty(m_options.rendezvousUrl, ref hftOptions.rendezvousUrl);
            HFTUtil.SetIfNotNullOrEmpty(m_options.serverPort, ref hftOptions.port);
            m_hftSite = m_gameObject.AddComponent<HFTSite>();
            m_hftSite.Init(hftOptions);

            string[] addresses = {
                "0.0.0.0:18679",
//                "0.0.0.0:80",
            };

            m_webServer = new HFTWebServer(m_options, addresses);
            m_webServer.Start();

            if (m_options.dns || m_options.installationMode)
            {
                m_dnsRunner = new HFTDnsRunner();
                m_dnsRunner.Start();
            }
        }

        public void StopServer()
        {
            CleanupCheck();
            if (m_hftSite != null)
            {
                m_hftSite.Stop();
                Component.Destroy(m_hftSite);
                m_hftSite = null;
            }
            if (m_webServer != null)
            {
                m_webServer.Stop();
                m_webServer = null;
            }
            if (m_dnsRunner != null)
            {
                m_dnsRunner.Stop();
                m_dnsRunner = null;
            }
        }

        HFTRuntimeOptions m_options;
        GameObject m_gameObject;
        HFTSite m_hftSite;
        HFTCheck m_check;
        HFTWebServer m_webServer;
        HFTDnsRunner m_dnsRunner;
    }
}  // namaspace HappyFunTimes
