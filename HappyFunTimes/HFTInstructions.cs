using UnityEngine;
using System.Collections;

namespace HappyFunTimes {

    public class HFTInstructions : MonoBehaviour
    {
        private Vector2 m_guiSize;
        private GUIStyle m_guiMsgStyle = new GUIStyle();
        private GUIStyle m_guiBackgroundStyle = new GUIStyle();
        private GUIContent m_guiContent = null;
        private string m_guiMsg = "";
        private Rect m_guiMsgRect = new Rect(0,0,0,0);
        private Rect m_guiBackgroundRect = new Rect(0,0,0,0);
        private float m_startScrollOffset = 0;
        private float m_scrollOffset = 0;
        private float m_minScrollOffset = 0;
        private float m_scrollSpeed = 0;
        private bool m_bottom = false;

        private const float SCROLL_SPEED = 1.0f;

        [CmdName("instructions")]
        private class MessageInstructions : MessageCmdData
        {
            public string msg = "";
            public bool bottom = false;
        }

        public void Init(GameSystem gameSystem)
        {
            gameSystem.NetPlayer.RegisterCmdHandler<MessageInstructions>(HandleInstructions);
        }

        private void HandleInstructions(MessageInstructions data)
        {
            SetInstructions(data.msg, data.bottom);
        }

        public void SetInstructions(string msg, bool bottom)
        {
            m_bottom = bottom;
            m_guiMsg = msg;
            SetupInstructions();
        }

        void Awake()
        {
            SetInstructions("", false);
            //SetInstructions("Connect to HappyFunTimes, on iOS just wait, on Android use Chrome and go to \"h.com\"", true);
        }

        void OnGUI()
        {
            if (System.String.IsNullOrEmpty(m_guiMsg))
            {
                return;
            }

            int oldDepth = GUI.depth;
            GUI.depth = 1000000;

            if (m_guiBackgroundRect.width != Screen.width)
            {
                SetupInstructions();
            }

            m_scrollOffset -= m_scrollSpeed;
            if (m_scrollOffset < m_minScrollOffset)
            {
                m_scrollOffset = m_startScrollOffset;
            }

            GUI.Box(m_guiBackgroundRect, "", m_guiBackgroundStyle);
            m_guiMsgRect.x = m_scrollOffset;
            GUI.Box(m_guiMsgRect, m_guiMsg, m_guiMsgStyle);

            GUI.depth = oldDepth;   // I don't think this is needed
        }

        void SetupInstructions()
        {
            Color[] pix = new Color[1];
            pix[0] = new Color(0f, 0f, 0f, 0.5f);
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixels(pix);
            tex.Apply();

            m_guiMsgStyle.normal.textColor = Color.white;
            m_guiMsgStyle.fontSize = 32;
            m_guiMsgStyle.fontStyle = FontStyle.Bold;

            m_guiBackgroundStyle.normal.background = tex;

            m_guiContent = new GUIContent(m_guiMsg);
            m_guiSize = m_guiMsgStyle.CalcSize(m_guiContent);
            m_guiMsgRect.x = 0.0f;
            m_guiMsgRect.y = m_bottom ?  Screen.height - m_guiSize.y : 0.0f;
            m_guiMsgRect.width = Screen.width;
            m_guiMsgRect.height = m_guiSize.y;

            m_guiBackgroundRect = new Rect(m_guiMsgRect);

            m_startScrollOffset = Screen.width;
            m_scrollOffset = m_startScrollOffset;
            m_minScrollOffset = -m_guiSize.x - 400.0f;  // the -400 makes it not repeat immediately
            m_scrollSpeed = SCROLL_SPEED;

            bool msgFitsOnScreen = m_guiSize.x < Screen.width;
            if (msgFitsOnScreen)
            {
                m_scrollOffset = Screen.width * 0.5f - m_guiSize.x * 0.5f;
                m_scrollSpeed = 0.0f;
            }
        }
    }
}  // namespace HappyFunTimes

