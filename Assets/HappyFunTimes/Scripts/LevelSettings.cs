using UnityEngine;

// There is supposed to be only 1 of these.
// Other objects can use LevelSettings.settings to
// access all the global settings
public class LevelSettings : MonoBehaviour
{
    public Transform bottomOfLevel;
    public Transform[] spawnPoints;

    static private LevelSettings s_settings;

    public static LevelSettings settings
    {
        get {
            return s_settings;
        }
    }

    void Awake()
    {
        if (s_settings != null)
        {
            throw new System.InvalidProgramException("there is more than one level settings object!");
        }
        s_settings = this;
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
