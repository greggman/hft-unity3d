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
using HappyFunTimes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace HappyFunTimes
{
    [AddComponentMenu("")]
    public class HFTCheck : MonoBehaviour {

        HFTLog m_log = new HFTLog("HFTCheck");
        Action m_connectFunc;       // func to call if connected
        Action m_failFunc;          // func to call if failed to connect
        string m_url;
        int m_tries;
        bool m_found;
        WWW m_www;
        const int s_maxTries = 500;

        public void Init(string url, Action onConnect, Action onFail)
        {
            System.Uri uri = new System.Uri(url);
            m_url = "http://" + uri.Host + ":" + uri.Port;
            m_connectFunc = onConnect;
            m_failFunc = onFail;
            m_tries = 0;
            m_found = false;
            StartCoroutine(CheckForHFT());
        }

        IEnumerator CheckForHFT()
        {
            while (!m_found && m_tries < s_maxTries)
            {
                ++m_tries;
                if (m_tries > 1)
                {
                    m_log.Tell("Checking for HFT(" + m_url + ") try" + m_tries + " of " + s_maxTries);
                }
                yield return PingHFT();
                yield return new WaitForSeconds(1.0f);
            }

            if (!m_found)
            {
                m_log.Tell("could not connect to HappyFunTimes server");
                m_failFunc();
            }
            else
            {
                m_connectFunc();
            }
        }

        IEnumerator PingHFT()
        {
            string json = JsonUtility.ToJson(new PostCmd("happyFunTimesPingForGame"));
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
            var headers = new Dictionary<string, string>();
            headers["Content-Type"] = "application/json";
            m_www = new WWW(m_url, bytes, headers);

            yield return m_www;

            string err = m_www.error;
            string result = m_www.text;
            m_www = null;

            if (String.IsNullOrEmpty(err))
            {
                HFTPing ping = JsonUtility.FromJson<HFTPing>(result);
                if (ping != null && ping.id.Equals("HappyFunTimes")) {
                    m_found = true;
                }
            }

            if (!m_found)
            {
                m_log.Tell("error: " + err + ", result:" + result);
            }
        }
    }
}

