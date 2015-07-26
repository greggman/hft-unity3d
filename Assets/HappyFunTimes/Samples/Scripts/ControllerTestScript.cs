using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControllerTestScript : MonoBehaviour {

    private int m_id;
    private HFTGamepad m_gamepad;
    private static Dictionary<int, bool> s_ids= new Dictionary<int, bool>();
//    private HFTInput m_hftInput;

    void Start ()
    {
        // Find an empty id;
        bool foo = false;
        for (int ii = 0; ii < 1000; ++ii) {
            if (!s_ids.TryGetValue(ii, out foo)) {
                m_id = ii;
                s_ids[ii] = true;
                break;
            }
        }

        m_gamepad = GetComponent<HFTGamepad>();
//        m_hftInput = GetComponent<HFTInput>();
        m_gamepad.NetPlayer.OnDisconnect += OnDisconnect;
    }

    void OnDisconnect(object sender, System.EventArgs args)
    {
        s_ids.Remove(m_id);
    }

    void OnGUI()
    {
        int unitWidth = 20;
        int unitHeight = 20;
        int xx = 10 + 110 * m_id;
        int yy = 10;
        GUI.Box(new Rect(xx, 10, 100, unitHeight), m_gamepad.Name);
        yy += unitHeight;
        GUI.Box(new Rect(xx, yy, 100, unitHeight), "buttons");
        yy += unitHeight;
        for (int ii = 0; ii < m_gamepad.buttons.Length; ++ii) {
            int x = ii % 4;
            int y = ii / 4;
            GUI.Box(new Rect(xx + x * unitWidth, yy + y * unitHeight, unitWidth, unitHeight), m_gamepad.buttons[ii].pressed ? "*" : "");
        }
        yy += unitHeight * ((m_gamepad.buttons.Length + 3) / 4);

        GUI.Box(new Rect(xx, yy, 100, unitHeight), "axes");
        yy += unitHeight;
        for (int ii = 0; ii < m_gamepad.axes.Length; ++ii) {
            int x = ii % 4;
            int y = ii / 4;
            GUI.Box(new Rect(xx + x * unitWidth, yy + y * unitHeight, unitWidth, unitHeight), m_gamepad.axes[ii].ToString());
        }

    }
}
