using UnityEngine;
using System.Collections;

namespace HappyFunTimes
{
    public class HFTStartStopTest : MonoBehaviour
    {
        PlayerSpawner m_spawner;
        bool started = false;

        // Use this for initialization
        void Start ()
        {
            m_spawner = GetComponent<PlayerSpawner>();
        }

        void OnGUI()
        {
            if (GUI.Button(new Rect(10, 10, 150, 100), started ? "Stop HFT" : "Start HFT"))
            {
                if (started)
                {
                    m_spawner.StopHappyFunTimes();
                }
                else
                {
                    m_spawner.StartHappyFunTimes();
                }
                started = !started;
            }

        }
    }
}

