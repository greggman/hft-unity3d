using HappyFunTimes;
using System.Collections.Generic;
using UnityEngine;

// There is supposed to be only 1 of these.
// Other objects can use MultiMachineLevelSettings.settings to
// access all the global settings
public class MultiMachineLevelSettings : MonoBehaviour
{
    public Transform bottomOfLevel;
    public Transform leftEdgeOfLevel;
    public Transform rightEdgeOfLevel;
    public Transform[] spawnPoints;
    public int numGames;

    public PlayerSpawner playerSpawner
    {
        get
        {
            return m_playerSpawner;
        }
    }

    public static MultiMachineLevelSettings settings
    {
        get
        {
            return s_settings;
        }
    }

    private PlayerSpawner m_playerSpawner;
    static private MultiMachineLevelSettings s_settings;

    void Awake()
    {
        if (s_settings != null)
        {
            throw new System.InvalidProgramException("there is more than one level settings object!");
        }
        s_settings = this;

        HFTArgParser p = new HFTArgParser();
        p.TryGet<int>("num-games", ref numGames);

        m_playerSpawner = GetComponent<PlayerSpawner>();
    }

    void Cleanup()
    {
        s_settings = null;
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
