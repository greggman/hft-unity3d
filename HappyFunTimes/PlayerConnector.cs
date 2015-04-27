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
    public string gameId = "";
    public bool showMessages = false;
    public bool allowMultipleGames;
    public int timeoutForDisconnectedPlayersToReconnect = 0;

    public GameServer server
    {
        get
        {
            return m_server;
        }
    }

    [CmdName("setName")]
    private class MessageSetName : MessageCmdData {
        public MessageSetName() {  // needed for deserialization
        }
        public MessageSetName(string _name) {
            name = _name;
        }
        public string name = "";
    };

    void StartConnection() {
        GameServer.Options options = new GameServer.Options();
        options.gameId = gameId;
        options.allowMultipleGames = allowMultipleGames;
        options.showMessages = showMessages;

        m_server = new GameServer(options, gameObject);

        m_server.OnPlayerConnect += StartNewPlayer;
        m_server.OnConnect += Connected;
        m_server.OnDisconnect += Disconnected;

        m_server.Init();
    }

    // The player has disconnected
    void RemoveNetPlayer(object sender, EventArgs e) {
        NetPlayer netPlayer = (NetPlayer)sender;
        NetPlayerState netPlayerState = GetActiveNetPlayerState(netPlayer);
        if (netPlayerState != null) {
            m_activePlayers.Remove(netPlayerState);
        }

        netPlayerState = GetWaitingNetPlayerState(netPlayer);
        if (netPlayerState != null) {
            m_waitingPlayers.Remove(netPlayerState);
        }

        PlayerState playerState = GetPlayerState(netPlayer);
        if (playerState != null) {
            playerState.netPlayer = null;
            playerState.disconnectTime = Time.time;
            StartWaitingPlayers();
        }
    }

    PlayerState GetPlayerState(NetPlayer netPlayer) {
        for (int pndx = 0; pndx < m_playerState.Length; ++pndx) {
            PlayerState playerState = m_playerState[pndx];
            if (playerState.netPlayer == netPlayer) {
                return playerState;
            }
        }
        return null;
    }

    NetPlayerState GetActiveNetPlayerState(NetPlayer netPlayer) {
        return m_activePlayers.Find(delegate(NetPlayerState otherNetPlayerState) {
            return otherNetPlayerState.netPlayer == netPlayer;
        });
    }

    NetPlayerState GetWaitingNetPlayerState(NetPlayer netPlayer) {
        return m_waitingPlayers.Find(delegate(NetPlayerState otherNetPlayerState) {
            return otherNetPlayerState.netPlayer == netPlayer;
        });
    }

    void SetNetPlayerName(NetPlayer netPlayer, MessageSetName data) {
        // Find netplayer
        NetPlayerState netPlayerState = GetWaitingNetPlayerState(netPlayer);
        if (netPlayerState != null) {
            netPlayerState.name = data.name;
        }
    }

    void SendSpawnInfoToGameObject(string msg, GameObject gameObject, NetPlayerState netPlayerState) {
        string name = netPlayerState.name;

        SpawnInfo spawnInfo = new SpawnInfo();
        spawnInfo.netPlayer = netPlayerState.netPlayer;
        spawnInfo.name = !String.IsNullOrEmpty(name) ? name : ("Player" + (++m_count));
        spawnInfo.data = netPlayerState.data;
        gameObject.SendMessage(msg, spawnInfo);
    }

    void StartActivePlayer(GameObject gameObject, PlayerState playerState, NetPlayerState netPlayerState) {
        m_activePlayers.Add(netPlayerState);

        NetPlayer netPlayer = netPlayerState.netPlayer;
        netPlayer.RemoveAllHandlers();
        netPlayer.OnDisconnect += RemoveNetPlayer;

        playerState.netPlayer = netPlayer;
        playerState.id = netPlayer.GetSessionId();

        SendSpawnInfoToGameObject("InitializeNetPlayer", gameObject, netPlayerState);
    }

    NetPlayerState DequeFirstWaitingPlayer() {
        IEnumerator<NetPlayerState> iter = m_waitingPlayers.GetEnumerator();
        iter.MoveNext();
        NetPlayerState netPlayerState = iter.Current;
        m_waitingPlayers.Remove(netPlayerState);
        return netPlayerState;
    }

    bool SlotCanAcceptNewPlayer(PlayerState playerState) {
        return playerState.netPlayer == null &&
               Time.time - playerState.disconnectTime > timeoutForDisconnectedPlayersToReconnect;
    }

    public void StartWaitingPlayers() {
        if (m_waitingPlayers.Count > 0) {
            for (int pndx = 0; pndx < m_playerState.Length; ++pndx) {
                PlayerState playerState = m_playerState[pndx];
                if (SlotCanAcceptNewPlayer(playerState)) {
                    NetPlayerState netPlayerState = DequeFirstWaitingPlayer();
                    if (netPlayerState == null) {
                        return;
                    }
                    StartActivePlayer(players[pndx], playerState, netPlayerState);
                }
            }
        }
    }

    void AddWaitingPlayer(NetPlayerState netPlayerState) {
        NetPlayer netPlayer = netPlayerState.netPlayer;
        netPlayer.RemoveAllHandlers();
        netPlayer.RegisterCmdHandler<MessageSetName>(delegate(MessageSetName msgdata) {
            SetNetPlayerName(netPlayer, msgdata);
        });
        netPlayer.OnDisconnect += RemoveNetPlayer;
        m_waitingPlayers.Add(netPlayerState);

        SendSpawnInfoToGameObject("WaitingNetPlayer", gameObject, netPlayerState);
    }

    void StartNewPlayer(object sender, PlayerConnectMessageArgs e)
    {
        NetPlayerState netPlayerState = new NetPlayerState(e.netPlayer, "", e.data);
        string id = e.netPlayer.GetSessionId();
        if (id.Length > 0) {
            // Check if there is a slot with this id
            for (int pndx = 0; pndx < m_playerState.Length; ++pndx) {
                PlayerState playerState = m_playerState[pndx];
                if (playerState.id == id) {
                    if (playerState.netPlayer == null) {
                        StartActivePlayer(players[pndx], playerState, netPlayerState);
                        return;
                    } else {
                        Debug.LogError("Player it same ID join but they're already playing???");
                    }
                }
            }
        }

        // Add player to list of all people connected
        AddWaitingPlayer(netPlayerState);
        StartWaitingPlayers();
    }

    public void StartLocalPlayer(NetPlayer netPlayer, string name = "", Dictionary<string, object> data = null)
    {
        AddWaitingPlayer(new NetPlayerState(netPlayer, name, data));
        StartWaitingPlayers();
    }

    /// <summary>
    /// Call this to rotate an active player out and start the next waiting player.
    /// </summary>
    /// <param name="netPlayer">The NetPlayer of the player to return</param>
    /// <returns></returns>
    public void ReturnPlayer(NetPlayer netPlayer) {
        NetPlayerState netPlayerState = GetActiveNetPlayerState(netPlayer);
        if (netPlayerState != null) {
            m_activePlayers.Remove(netPlayerState);
        } else {
            netPlayerState = GetWaitingNetPlayerState(netPlayer);
            if (netPlayerState != null) {
                m_waitingPlayers.Remove(netPlayerState);
            }
        }

        AddWaitingPlayer(netPlayerState);

        PlayerState playerState = GetPlayerState(netPlayer);
        if (playerState != null) {
            playerState.netPlayer = null;
            // Make the slot available immediately.
            playerState.disconnectTime = Time.time - timeoutForDisconnectedPlayersToReconnect;

            StartWaitingPlayers();
        }
    }

    /// <summary>
    /// Returns all the current players to the waiting list
    /// and gets new ones if any are waiting
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public void FlushCurrentPlayers() {
        for (int pndx = 0; pndx < m_playerState.Length; ++pndx) {
            PlayerState playerState = m_playerState[pndx];
            if (playerState.netPlayer != null) {
                ReturnPlayer(playerState.netPlayer);
            }
        }
    }

    void ResetState() {
        m_playerState = new PlayerState[players.Length];
        for (int ii = 0; ii < m_playerState.Length; ++ii) {
            PlayerState playerState = new PlayerState();
            playerState.disconnectTime = Time.time - timeoutForDisconnectedPlayersToReconnect;
            m_playerState[ii] = playerState;
        }
    }

    void Start ()
    {
        ResetState();
        StartConnection();
    }

    void Update() {
        if (m_waitingPlayers.Count > 0) {
            for (int pndx = 0; pndx < m_playerState.Length; ++pndx) {
                PlayerState playerState = m_playerState[pndx];
                if (SlotCanAcceptNewPlayer(playerState)) {
                    StartWaitingPlayers();
                    return;
                }
            }
        }
    }

    void Connected(object sender, EventArgs e)
    {
    }

    void Disconnected(object sender, EventArgs e)
    {
        Debug.Log("Quitting");
        Application.Quit();
    }

    void Cleanup()
    {
        if (m_server != null) {
            m_server.Close();
        }
    }

    void OnDestroy()
    {
        Cleanup();
    }

    void OnApplicationExit()
    {
        Cleanup();
    }

    // The state of Unity GameObject players
    class PlayerState {
        public string id = "";
        public NetPlayer netPlayer;
        public float disconnectTime;
    };

    // The state of NetPlayers (people with phones).
    class NetPlayerState {
        public NetPlayerState(NetPlayer _netPlayer, string _name, Dictionary<string, object> _data) {
            netPlayer = _netPlayer;
            name = _name;
            data = _data;
        }
        public NetPlayer netPlayer;
        public string name;
        public Dictionary<string, object> data;
    };

    private List<NetPlayerState> m_activePlayers = new List<NetPlayerState>();
    private List<NetPlayerState> m_waitingPlayers = new List<NetPlayerState>();
    private PlayerState[] m_playerState;
    private GameServer m_server;
    private int m_count;
};

}   // namespace HappyFunTimes
