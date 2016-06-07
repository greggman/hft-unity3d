/*
 * Copyright 2014, Gregg Tavares.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are
 * met:
 *
 *     * Redistributions of source code must retain the above copyright
 * notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above
 * copyright notice, this list of conditions and the following disclaimer
 * in the documentation and/or other materials provided with the
 * distribution.
 *     * Neither the name of Gregg Tavares. nor the names of its
 * contributors may be used to endorse or promote products derived from
 * this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 * A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
 * OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
 * LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 * THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using UnityEngine;
using System.Collections;

namespace HappyFunTimes {

    public class HFTInstructions : MonoBehaviour
    {
        [TextArea(10, 30)]
        public string instructions = "connect to the <color=yellow>(WIFI)</color> wifi then go to <color=cyan>happyfuntimes.net</color>";
        public bool bottom = false;
        public bool show = false;
        public float scrollSpeed = 1.0f;

        private Vector2 m_guiSize;
        private GUIStyle m_guiMsgStyle = new GUIStyle();
        private GUIStyle m_guiBackgroundStyle = new GUIStyle();
        private GUIContent m_guiContent = null;
        private Rect m_guiMsgRect = new Rect(0,0,0,0);
        private Rect m_guiBackgroundRect = new Rect(0,0,0,0);
        private float m_startScrollOffset = 0;
        private float m_scrollOffset = 0;
        private float m_minScrollOffset = 0;
        private bool m_msgFitsOnScreen = false;
        static string s_defaultWifiName = "local WiFi";
        
        public void Awake() {
            HFTArgParser p = new HFTArgParser();

            bool found = false;
            found |= p.Contains("show-instructions");
            found |= p.TryGet<string>("instructions", ref instructions);
            found |= p.TryGetBool("bottom", ref bottom);
            if (found)
            {
                show = true;
            }
        }

        public void Start() {
            show |= HFTHappyFunTimesSettings.showInstructions;
            bottom |= HFTHappyFunTimesSettings.instructionsPosition;
            if (!System.String.IsNullOrEmpty(HFTHappyFunTimesSettings.instructions))
            {
                instructions = HFTHappyFunTimesSettings.instructions;
            }
        }

        void OnGUI()
        {
            if (!show || System.String.IsNullOrEmpty(instructions))
            {
                return;
            }

            int oldDepth = GUI.depth;
            GUI.depth = 1000000;

            if (m_guiBackgroundRect.width != Screen.width)
            {
                SetupInstructions();
            }

            if (!m_msgFitsOnScreen)
            {
                m_scrollOffset -= scrollSpeed;
                if (m_scrollOffset < m_minScrollOffset)
                {
                    m_scrollOffset = m_startScrollOffset;
                }
            }

            GUI.Box(m_guiBackgroundRect, "", m_guiBackgroundStyle);
            m_guiMsgRect.x = m_scrollOffset;
            GUI.Box(m_guiMsgRect, m_guiContent, m_guiMsgStyle);

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

            m_guiContent = new GUIContent(instructions.Replace("\n", " ").Replace("(WIFI)", GetCurrentSSID()));
            m_guiSize = m_guiMsgStyle.CalcSize(m_guiContent);
            m_guiMsgRect.x = 0.0f;
            m_guiMsgRect.y = bottom ? Screen.height - m_guiSize.y : 0.0f;
            m_guiMsgRect.width = Screen.width;
            m_guiMsgRect.height = m_guiSize.y;

            m_guiBackgroundRect = new Rect(m_guiMsgRect);

            m_startScrollOffset = Screen.width;
            m_scrollOffset = m_startScrollOffset;
            m_minScrollOffset = -m_guiSize.x - 400.0f;  // the -400 makes it not repeat immediately

            m_msgFitsOnScreen = m_guiSize.x < Screen.width;
            if (m_msgFitsOnScreen)
            {
                m_scrollOffset = Screen.width * 0.5f - m_guiSize.x * 0.5f;
            }
        }

        string GetCurrentSSID()
        {
            #if UNITY_STANDALONE_OSX
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo psi = p.StartInfo;
                psi.UseShellExecute = false;
                psi.FileName = "/System/Library/PrivateFrameworks/Apple80211.framework/Versions/Current/Resources/airport";
                psi.Arguments = "-I";
                psi.RedirectStandardError = true;
                psi.RedirectStandardOutput = true;
                p.Start();
                string result = p.StandardOutput.ReadToEnd();
                if (p.ExitCode == 0)
                {
                    var re = new System.Text.RegularExpressions.Regex("^  *SSID: (.+?)$", System.Text.RegularExpressions.RegexOptions.Multiline);
                    var m = re.Match(result);
                    if (m.Success)
                    {
                        return m.Groups[1].Value;
                    }
                }
                else
                {
                    Debug.Log(p.StandardError.ReadToEnd());
                }
            #elif UNITY_STANDALONE_WIN
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo psi = p.StartInfo;
                psi.UseShellExecute = false;
                psi.FileName = "netsh";
                psi.Arguments = "wlan show interfaces";
                psi.RedirectStandardError = true;
                psi.RedirectStandardOutput = true;
                psi.CreateNoWindow = true;
                p.Start();
                string result = p.StandardOutput.ReadToEnd();
                if (p.ExitCode != 0)
                {
                    Debug.Log(p.StandardError.ReadToEnd());
                    return s_defaultWifiName;
                }

                var re = new System.Text.RegularExpressions.Regex("^ *SSID *: (.+?)$", System.Text.RegularExpressions.RegexOptions.Multiline);
                var m = re.Matches(result.Replace("\r",""));
                if (m.Count > 0)
                {
                    return m[0].Groups[1].Value;
                }
            #endif
            return s_defaultWifiName;
        }
    }
}  // namespace HappyFunTimes

