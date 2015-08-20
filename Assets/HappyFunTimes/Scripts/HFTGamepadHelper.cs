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
using HappyFunTimes;

public class HFTGamepadHelper : MonoBehaviour {

    [System.NonSerialized]
    public PlayerSpawner playerSpawner;

    // This gives you a chance to send that player's phone
    // a command to tell it to display "The game is full" or
    // whatever you want.
    //
    // Note: You can call PlayerSpawner.ReturnPlayer to eject
    // a player from their slot and get a new player for that slot
    // If you do that this function will be called for the returned
    // player.
    //
    // Simiarly you can call PlayerSpawner.FlushCurrentPlayers to
    // eject all current players in which case this will be called
    // for all players that were player.
    void WaitingNetPlayer(SpawnInfo spawnInfo) {
        // Tell the controller to display full message
        spawnInfo.netPlayer.SendCmd("full");
    }

    static private HFTGamepadHelper s_helper;

    public static HFTGamepadHelper helper
    {
        get {
            return s_helper;
        }
    }

    void Awake()
    {
        if (s_helper != null)
        {
            throw new System.InvalidProgramException("there is more than one HFTGamepadHelper component!");
        }
        s_helper = this;
        playerSpawner = GetComponent<PlayerSpawner>();
    }

    void Cleanup()
    {
        s_helper = null;
    }

    void OnDestroy()
    {
        Cleanup();
    }

    void OnApplicationExit()
    {
        Cleanup();
    }
}


