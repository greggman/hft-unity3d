using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControllerTestScript : MonoBehaviour {

    private int m_id;
    private HFTGamepad m_gamepad;
    private HFTInput m_hftInput;
    private static Dictionary<int, bool> s_ids= new Dictionary<int, bool>();

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
        m_hftInput = GetComponent<HFTInput>();
        m_gamepad.NetPlayer.OnDisconnect += OnDisconnect;
    }

    void OnDisconnect(object sender, System.EventArgs args)
    {
        s_ids.Remove(m_id);
    }

    void OnGUI()
    {
        int areaWidth = 200;
        int unitWidth = areaWidth / 4;
        int unitHeight = 20;
        int xx = 10 + 110 * m_id;
        int yy = 10;
        GUI.Box(new Rect(xx, 10, areaWidth, unitHeight), m_gamepad.Name);
        yy += unitHeight;
        GUI.Box(new Rect(xx, yy, areaWidth, unitHeight), "buttons");
        yy += unitHeight;
        for (int ii = 0; ii < m_gamepad.buttons.Length; ++ii) {
            int x = ii % 4;
            int y = ii / 4;
            GUI.Box(new Rect(xx + x * unitWidth, yy + y * unitHeight, unitWidth, unitHeight), m_gamepad.buttons[ii].pressed ? "*" : "");
        }
        yy += unitHeight * ((m_gamepad.buttons.Length + 3) / 4);

        GUI.Box(new Rect(xx, yy, areaWidth, unitHeight), "axes");
        yy += unitHeight;
        for (int ii = 0; ii < m_gamepad.axes.Length; ++ii) {
            int x = ii % 4;
            int y = ii / 4;
            GUI.Box(new Rect(xx + x * unitWidth, yy + y * unitHeight, unitWidth, unitHeight), m_gamepad.axes[ii].ToString());
        }

        yy += unitHeight * ((m_gamepad.axes.Length + 3) / 4);

        GUI.Box(new Rect(xx, yy, areaWidth, unitHeight), "touch");
        yy += unitHeight;
        int numTouch = m_hftInput.touchCount;
        unitWidth = areaWidth / 3;
        unitHeight *= 2;
        for (int ii = 0; ii < numTouch; ++ii) {
            int x = ii % 3;
            int y = ii / 3;
            HFTInput.Touch touch = m_hftInput.GetTouch(ii);
            GUI.Box(new Rect(xx + x * unitWidth, yy + y * unitHeight, unitWidth, unitHeight / 2), touch.phase.ToString());
            GUI.Box(new Rect(xx + x * unitWidth, yy + y * unitHeight + unitHeight / 2, unitWidth, unitHeight / 2), touch.rawPosition.x.ToString() + "," + touch.rawPosition.y.ToString());
        }
    }
}
