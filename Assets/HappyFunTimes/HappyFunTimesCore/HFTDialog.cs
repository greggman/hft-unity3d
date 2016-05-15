using UnityEngine;
using System;
using System.Collections;

namespace HappyFunTimes {
    public class HFTDialog : MonoBehaviour {

        Rect m_windowRect;
        Action m_action;
        string m_title;
        string m_msg;
        bool m_richText;

        static public void MessageBox(string title, string msg, Action action = null, bool richText = false)
        {
            GameObject go = new GameObject("HFTDialog");
            HFTDialog dlg = go.AddComponent<HFTDialog>();
            dlg.Init(title, msg, action, richText);
        }

        void Init(string title, string msg, Action action, bool richText)
        {
            m_title = title;
            m_msg = msg;
            m_action = action;
            m_richText = richText;
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
            GUIStyle style = new GUIStyle(GUI.skin.GetStyle("Label"));

            style.normal.textColor = Color.white;
            style.wordWrap = true;
            //style.fixedWidth = width - border * 2;
            //style.fixedHeight = height - border * 2;
            style.fontSize = 12;
            style.richText = m_richText;

            GUI.Label(l, m_msg, style);

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

