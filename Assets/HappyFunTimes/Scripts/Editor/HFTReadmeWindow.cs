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

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using HappyFunTimes;

namespace HappyFunTimesEditor
{
    public class HFTReadmeWindow : EditorWindow
    {
        private Vector2 m_scrollPos = new Vector2(0, 0);
        private GUIContent m_content;
        private string m_text;
        private bool m_richText;
        private HFTReadmeUtils.Link[] m_links;
        private Component m_component;
        private bool m_showLinks;

        static HFTReadmeWindow s_window = null;

        static public HFTReadmeWindow GetInstance() {
            if (s_window == null)
            {
                s_window = ScriptableObject.CreateInstance(typeof(HFTReadmeWindow)) as HFTReadmeWindow;
            }
            s_window.ShowUtility();
            return s_window;
        }

        static public HFTReadmeWindow GetInstanceIfExists() {
            return s_window;
        }

        void OnDestroy() {
            s_window = null;
        }

        void OnProjectChange() {
            if (s_window)
            {
                Close();
            }
        }

        public void CloseIfOurs(Component component) {
            if (component == m_component)
            {
                Close();
            }
        }

        public void SetContent(string title, string text, bool richText, bool useMarkdownish, Component component) {
            titleContent = new GUIContent(title);
            if (useMarkdownish) {
                var markdownish = HFTReadmeUtils.MarkdownishToRichText(text);
                m_links = markdownish.links;
                m_text = markdownish.richText;
            } else {
                m_text = text;
                m_links = null;
            }
            m_content = new GUIContent(m_text);
            m_richText = richText;
            m_component = component;
        }

        GUIStyle GetStyle(float width, float height) {
            GUIStyle style = new GUIStyle(GUI.skin.GetStyle("Label"));

            style.wordWrap = true;
            style.richText = m_richText;

            return style;
        }

        void DrawLinks()
        {
            GUIStyle style = new GUIStyle(GUI.skin.GetStyle("Button"));
            style.alignment = TextAnchor.MiddleLeft;

            if (m_links != null)
            {
                for (int i = 0; i < m_links.Length; ++i)
                {
                    var link = m_links[i];
                    if (GUILayout.Button("[" + (i + 1).ToString() + "] " + link.description, style))
                    {
                        Application.OpenURL(link.url);
                    }
                }
            }
        }

        void OnGUI()
        {
            bool haveLinks = m_links != null && m_links.Length > 0;

            GUIStyle style = GetStyle(0, 0);
            GUIStyle linkbarStyle = GUI.skin.GetStyle("horizontalSlider");

            EditorGUILayout.BeginVertical();
            m_scrollPos = EditorGUILayout.BeginScrollView(m_scrollPos, GUILayout.Width(EditorGUIUtility.currentViewWidth), GUILayout.Height(position.height));
            if (haveLinks)
            {
                m_showLinks = EditorGUILayout.Foldout(m_showLinks, "*** Links ***");
                if (m_showLinks)
                {
                    DrawLinks();
                }
                GUILayout.Box("", linkbarStyle);
            }
            EditorGUILayout.SelectableLabel(m_text, style, GUILayout.Height(style.CalcHeight(m_content, EditorGUIUtility.currentViewWidth)));

            if (haveLinks)
            {
                GUILayout.Box("", linkbarStyle);
                DrawLinks();
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
    }

}  // namespace HappyFunTimesEditor



