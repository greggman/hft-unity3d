using UnityEngine;
using System;
using System.Collections;

namespace HappyFunTimes {
    public class HFTDialog : MonoBehaviour {

        Rect m_windowRect;
        Action m_action;
        string m_title;
        string m_msg;

        static public void MessageBox(string title, string msg, Action action = null)
        {
            GameObject go = new GameObject("HFTDialog");
            HFTDialog dlg = go.AddComponent<HFTDialog>();
            dlg.Init(title, msg, action);
        }

        void Init(string title, string msg, Action action)
        {
            m_title = title;
            m_msg = msg;
            m_action = action;
        }

        void OnGUI()
        {
            const int maxWidth = 640;
            const int maxHeight = 480;

            int width = Mathf.Min(maxWidth, Screen.width - 20);
            int height = Mathf.Min(maxHeight, Screen.height - 20);
            m_windowRect = new Rect(
                (Screen.width - width) / 2,
                (Screen.height - height) / 2,
                width,
                height);

            m_windowRect = GUI.Window(0, m_windowRect, WindowFunc, m_title);
        }

        void WindowFunc(int windowID)
        {
            const int border = 10;
            const int width = 50;
            const int height = 25;
            const int spacing = 10;

            Rect l = new Rect(
                border,
                border + spacing,
                m_windowRect.width - border * 2,
                m_windowRect.height - border * 2 - height - spacing);
            GUI.Label(l, m_msg);

            Rect b = new Rect(
                m_windowRect.width - width - border,
                m_windowRect.height - height - border,
                width,
                height);

            if (GUI.Button(b, "ok"))
            {
                Destroy(this.gameObject);
                if (m_action != null)
                {
                    m_action();
                }
            }

        }
    }
}

