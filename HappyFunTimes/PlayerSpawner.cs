using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using HappyFunTimes;

namespace HappyFunTimes {

public class SpawnInfo {
    public NetPlayer netPlayer;
    public string name;
    public Dictionary<string, object> data;
};

[AddComponentMenu("HappyFunTimes/PlayerSpawner")]
public class PlayerSpawner : MonoBehaviour
{
    public GameObject prefabToSpawnForPlayer;
    public string gameId = "";
    public bool showMessages = false;
    public bool allowMultipleGames;

    public GameServer server
    {
        get
        {
            return m_server;
        }
    }

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

    void StartNewPlayer(object sender, PlayerConnectMessageArgs e)
    {
        GameObject gameObject = (GameObject)Instantiate(prefabToSpawnForPlayer);
        SpawnInfo spawnInfo = new SpawnInfo();
        spawnInfo.netPlayer = e.netPlayer;
        spawnInfo.name = "Player" + (++m_count);
        spawnInfo.data = e.data;
        gameObject.SendMessage("InitializeNetPlayer", spawnInfo);
    }

    void Start ()
    {
        StartConnection();
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

    public GameServer GameServer {
        get {
            return m_server;
        }
        private set {
        }
    }

    private GameServer m_server;
    private int m_count;
};

}   // namespace HappyFunTimes
