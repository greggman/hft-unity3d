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
        private Component m_component;

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
                Destroy(s_window);
            }
        }

        public void CloseIfOurs(Component component) {
            if (component == m_component)
            {
                Close();
            }
        }

        public void SetContent(string title, string text, bool richText, Component component) {
            titleContent = new GUIContent(title);
            m_text = HFTReadmeUtils.MarkdownishToRichText(text);
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

        void OnGUI()
        {
            GUIStyle style = GetStyle(0, 0);
            EditorGUILayout.BeginVertical();
            m_scrollPos = EditorGUILayout.BeginScrollView(m_scrollPos, GUILayout.Width(EditorGUIUtility.currentViewWidth), GUILayout.Height (position.height));
            EditorGUILayout.SelectableLabel(m_text, style, GUILayout.Height(style.CalcHeight(m_content, EditorGUIUtility.currentViewWidth)));
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
    }

}  // namespace HappyFunTimesEditor



