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

namespace HappyFunTimes {

[AddComponentMenu("HappyFunTimes/PlayerConnector")]
public class PlayerConnector : MonoBehaviour
{
    public GameObject[] players;
    public int timeoutForDisconnectedPlayersToReconnect = 0;

    [Space(10)]
    [Header("advanced")]
    public HFTGameOptions happyfuntimesOptions = new HFTGameOptions();

    // two accessors for the same thing
    public GameServer server
    {
        get
        {
            return m_connectionManager.gameServer;
        }
    }
    public GameServer GameServer
    {
        get
        {
            return m_connectionManager.gameServer;
        }
    }

    /// <summary>
    /// Call this to rotate an active player out and start the next waiting player.
    /// </summary>
    /// <param name="netPlayer">The NetPlayer of the player to return</param>
    public void ReturnPlayer(NetPlayer netPlayer) {
        m_playerManager.ReturnPlayer(netPlayer);
    }

    /// <summary>
    /// Returns all the current players to the waiting list
    /// and gets new ones if any are waiting
    /// </summary>
    public void FlushCurrentPlayers()
    {
        m_playerManager.FlushCurrentPlayers();
    }

    public void StartHappyFunTimes()
    {
        if (!m_started)
        {
            m_log.Info("Starting HappyFunTimes");
            m_started = true;
            enabled = true;
            m_connectionManager.StartHappyFunTimes();
        }
    }

    public void StopHappyFunTimes()
    {
        if (m_started)
        {
            m_log.Info("Stopping HappyFunTimes");
            m_started = false;
            enabled = false;
            m_connectionManager.StopHappyFunTimes();
        }
    }

    void Awake()
    {
        m_connectToServerOnStart = enabled;
        m_connectionManager = new HFTConnectionManager(gameObject, happyfuntimesOptions);

        m_playerManager = new HFTPlayerManager(m_connectionManager.gameServer, gameObject, players.Length, timeoutForDisconnectedPlayersToReconnect, GetPlayer);
    }

    void Start()
    {
        if (m_connectToServerOnStart)
        {
            StartHappyFunTimes();
        }
    }

    void Update()
    {
        if (m_playerManager != null)
        {
            m_playerManager.Update();
        }
    }

    GameObject GetPlayer(int ndx)
    {
        return players[ndx];
    }

    void OnDestroy()
    {
        StopHappyFunTimes();
    }

    void OnApplicationExit()
    {
        StopHappyFunTimes();
    }

    private bool m_started;
    private bool m_connectToServerOnStart;
    private HFTPlayerManager m_playerManager;
    private HFTConnectionManager m_connectionManager;
    private HFTLog m_log = new HFTLog("PlayerConnector");
    private HFTRuntimeOptions m_options;
};

}   // namespace HappyFunTimes
