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

using System;
using System.Collections;
using System.Collections.Generic;
using DeJson;
using UnityEngine;
using UnityEngine.Networking;

namespace HappyFunTimes
{
    [AddComponentMenu("")]
    public class HFTSyncedClock : MonoBehaviour {

        HFTSyncedClock()
        {
            if (s_haveSyncedClock)
            {
                throw new System.InvalidOperationException("There should be only 1 HFTSyncedClock");
            }
            s_haveSyncedClock = true;
        }

        public void StartClock(string url)
        {
            if (!m_running)
            {
                m_running = true;
                System.Uri uri = new System.Uri(url);
                m_url = "http://" + uri.Host + ":" + uri.Port;
                StartCoroutine("SyncClock");
            }
        }

        public void StopClock()
        {
            if (m_running)
            {
                m_running = false;
                StopCoroutine("SyncClock");
            }
        }

        void OnDestroy()
        {
            StopClock();
            s_haveSyncedClock = false;
        }

        // Current time in seconds since epoch?
        static public double Now
        {
            get
            {
                double now = HFTUtil.GetJSTimeInSeconds();
                return now + s_timeOffsetSeconds;
            }
        }

        IEnumerator SyncClock()
        {
            for (;;)
            {
                yield return PingClock();
                yield return new WaitForSeconds(10.0f);
            }
        }

        IEnumerator PingClock()
        {
            using (var www = new UnityWebRequest(m_url, UnityWebRequest.kHttpVerbPOST)) {
                www.uploadHandler = new UploadHandlerRaw(m_timeCmdBytes);
                www.uploadHandler.contentType = "application/json";
                www.downloadHandler = new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json");
                double sendTime = HFTUtil.GetJSTimeInSeconds();
                yield return www.SendWebRequest();
                double receiveTime = HFTUtil.GetJSTimeInSeconds();

                string err = www.error;
                string result = www.downloadHandler.text;

                if (www.isHttpError || www.isNetworkError && !String.IsNullOrEmpty(err)) {
                    m_log.Tell("error: " + err + ", result:" + result);
                }
                else
                {
                    HFTTimePing ping = JsonUtility.FromJson<HFTTimePing>(result);
                    if (ping != null && ping.time > 0.0)
                    {
                        double duration = receiveTime - sendTime;
                        double serverTime = ping.time + duration / 2;
                        s_timeOffsetSeconds = serverTime - receiveTime;
                    }

                }
            }
        }

        bool m_running = false;
        HFTLog m_log = new HFTLog("HFTSyncedClock");
        string m_url;
        byte[] m_timeCmdBytes = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(new PostCmd("time")));

        static bool s_haveSyncedClock = false;
        static double s_timeOffsetSeconds = 0.0;
    }

}  // namespace HappyFunTimes
