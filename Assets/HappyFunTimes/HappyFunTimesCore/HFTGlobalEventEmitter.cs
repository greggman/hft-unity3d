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
    public enum HFTGlobalEventType
    {
        CheckingForServer,
        GettingDomain,
        GettingCertificate,
        Ready,
        DisconnectedFromServer,
        Error,
    }

    public class HFTGlobalEventEmitter
    {
        static private HFTGlobalEventEmitter s_instance;

        HFTEventProcessor m_eventProcessor;

        public static HFTGlobalEventEmitter GetInstance()
        {
            if (s_instance == null)
            {
                s_instance = new HFTGlobalEventEmitter();
            }
            return s_instance;
        }

        public void Setup(HFTEventProcessor eventProcessor)
        {
            m_eventProcessor = eventProcessor;
        }

        public delegate void HFTGlobalEventHandler(HFTGlobalEventType type, object data);

        public event HFTGlobalEventHandler OnGlobalEvent;

        public bool QueueEvent(HFTGlobalEventType type, object data)
        {
            // At the time we queue this event, is there a handler?
            // Let's us check the event might be handled so we can take other action if it's
            // not.
            if (OnGlobalEvent == null)
            {
                return false;
            }

            m_eventProcessor.QueueEvent(() => {
                HFTGlobalEventHandler handler = OnGlobalEvent;
                if (handler != null)
                {
                    handler(type, data);
                }
            });

            return true;
        }
    }

}  // namaspace HappyFunTimes
