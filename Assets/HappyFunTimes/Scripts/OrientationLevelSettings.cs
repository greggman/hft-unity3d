using UnityEngine;

// There is supposed to be only 1 of these.
// Other objects can use OrientationLevelSettings.settings to
// access all the global settings
public class OrientationLevelSettings : MonoBehaviour
{
    static private OrientationLevelSettings s_settings;

    public static OrientationLevelSettings settings
    {
        get {
            return s_settings;
        }
    }

    public OrientationLevelSettings() {
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
