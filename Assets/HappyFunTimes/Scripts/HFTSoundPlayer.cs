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
using HFTSounds;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (HFTGamepad))]
public class HFTSoundPlayer : MonoBehaviour
{

    private class MessageLoadSounds
    {
        public MessageLoadSounds(Sounds _sounds)
        {
            sounds = _sounds;
        }
        public Sounds sounds = null;
    }

    private class MessagePlaySound
    {
        public MessagePlaySound(string _name, bool _loop = false)
        {
            name = _name;
            loop = _loop;
        }
        public string name;
        public bool loop;
    }

    void Awake()
    {
        try {
            HFTGlobalSoundHelper.GetSounds();
        } catch (System.Exception) {
            Debug.LogError("No HFTGlobalSoundHelper in scene. Please add one");
        }
        m_gamepad = GetComponent<HFTGamepad>();
    }

    void Start()
    {
        NetPlayer netPlayer = m_gamepad.NetPlayer;
        if (netPlayer != null)
        {
            netPlayer.SendCmd("loadSounds", new MessageLoadSounds(HFTGlobalSoundHelper.GetSounds()));
        }
    }

    public void PlaySound(string name, bool loop = false)
    {
        NetPlayer netPlayer = m_gamepad.NetPlayer;
        if (netPlayer != null)
        {
            m_gamepad.NetPlayer.SendCmd("playSound", new MessagePlaySound(name, loop));
        }
    }

    private HFTGamepad m_gamepad;

};

