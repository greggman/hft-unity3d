using UnityEngine;
using System.Collections;
using HappyFunTimes;

public class SyncedClockScript : MonoBehaviour
{
    // Use this for initialization
    void Start ()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update ()
    {
        double time = HFTSyncedClock.Now;
        int eighthSecondTicks = (int)(time * 8.0 % 8.0);
        bool newState = eighthSecondTicks == 0;
        m_seconds = (int)(time % 60.0);

        m_spriteRenderer.color = newState ? Color.white : Color.red;

        transform.eulerAngles = new Vector3(0.0f, 0.0f, (float)(time % 1.0) * -360.0f);

        if (newState != m_lastState)
        {
            m_lastState = newState;
            if (newState)
            {
                m_audioSource.Play();
            }
        }
    }

    void OnGUI()
    {
        if (m_style == null)
        {
            m_style = GUI.skin.GetStyle("Label");
            m_style.alignment = TextAnchor.MiddleCenter;
            m_style.fontSize = 40;
            m_style.fontStyle = FontStyle.Bold;
        }

        if (m_seconds != m_oldSeconds)
        {
            m_oldSeconds = m_seconds;
            m_secondsStr = m_seconds.ToString().PadLeft(2, '0');
        }
        int width = 100;
        int height = 50;
        Rect r = new Rect(Screen.width / 2 - width / 2, Screen.height / 4 * 3, width, height);

        GUI.Label(r, m_secondsStr, m_style);
    }

    GUIStyle m_style;
    string m_secondsStr;
    int m_seconds;
    int m_oldSeconds = -1;
    bool m_lastState;
    AudioSource m_audioSource;
    SpriteRenderer m_spriteRenderer;
}
