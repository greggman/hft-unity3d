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
using System.Collections;

public class SoundTest : MonoBehaviour {

    void Awake ()
    {
        m_soundPlayer = GetComponent<HFTSoundPlayer>();
        m_gamepad = GetComponent<HFTGamepad>();

        // Delete ourselves if disconnected
        m_gamepad.OnDisconnect += Remove;
    }

    void Remove()
    {
        Destroy(gameObject);
    }

    void Start()
    {
        StartCoroutine("Play");
    }

    IEnumerator Play()
    {
        HFTSounds.Sounds sounds = HFTGlobalSoundHelper.GetSounds();
        string[] soundNames = new string[sounds.Keys.Count];
        sounds.Keys.CopyTo(soundNames, 0);

        int index = 0;
        while (true)
        {
            yield return new WaitForSeconds(1.0f);
            m_soundPlayer.PlaySound(soundNames[index]);
            index = (index + 1) % soundNames.Length;
        }
    }

    private HFTGamepad m_gamepad;
    private HFTSoundPlayer m_soundPlayer;
}
