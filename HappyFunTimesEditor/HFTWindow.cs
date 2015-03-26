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
    [Serializable]
    public class HFTUIInfo : ScriptableObject
    {
        [SerializeField]
        public HFTPackage m_package;

        [SerializeField]
        private bool m_showPackageJson = true;

        private bool m_dirty = false;

        private static HFTUIInfo s_instance = null;
        public static HFTUIInfo Instance
        {
            get
            {
                if (s_instance == null)
                {
                    var tmp = Resources.FindObjectsOfTypeAll<HFTUIInfo>();
                    if (tmp.Length > 0)
                    {
                        s_instance = tmp[0];
                    }
                    else
                    {
                        s_instance = ScriptableObject.CreateInstance<HFTUIInfo>();
                        s_instance.Init();
                    }
                    s_instance.hideFlags = HideFlags.DontSave;
                }
                return s_instance;
            }
        }

        public void OnEnable() {
            if (m_package == null) {
                m_package = new HFTPackage();
            }
        }

        public void Init()
        {
            m_package.Init();
        }

        public void Persist()
        {
            m_package.Persist();
        }

        private void EditString(/*Dictionary<string, object>dict,*/ string id)
        {
            string s = m_package.GetString(id);
            string n = EditorGUILayout.TextField(id, s);
            if (n != s || GUI.changed)
            {
                Undo.RecordObject(this, "set HappyFunTimes " + id);
                m_package.SetString(id, n);
                m_dirty = true;
            }
        }

        private void EditInt(/*Dictionary<string, object>dict,*/ string id, int min, int max)
        {
            int i = m_package.GetInt(id);
            int n = EditorGUILayout.IntField(id, i);
            n = Math.Max(Math.Min(max, n), min);
            if (n != i || GUI.changed) {
                Undo.RecordObject(this, "set HappyFunTimes " + id);
                m_package.SetInt(id, n);
                m_dirty = true;
            }
        }

        private void EditText(EditorWindow window, string id)
        {
            string s = m_package.GetString(id);
            GUILayout.Label (id, EditorStyles.boldLabel);
            string n = EditorGUILayout.TextArea(s, GUILayout.Height(window.position.height - 30));
            if (n != s || GUI.changed)
            {
                Undo.RecordObject(this, "set HappyFunTimes " + id);
                m_package.SetString(id, n);
                m_dirty = true;
            }
        }


        public void DoGUI(EditorWindow window) {
            m_showPackageJson = EditorGUILayout.Foldout(m_showPackageJson, "Package.json Settings");
            if (m_package != null && m_showPackageJson) {
                EditString("name");
                EditString("version");
                EditString("gameId");
                EditString("category");
                EditString("apiVersion");
                EditString("gameType");
                EditInt("minPlayers", 1, 1000);
                EditText(window, "description");
            }

            if (m_dirty) {
                EditorUtility.SetDirty(this);
                m_dirty = false;
            }
        }
    }

    public class HFTWindow : EditorWindow
    {
        [SerializeField]
        private HFTUIInfo m_uiInfo;

        HFTWindow()
        {
        }

        static void Init() {
            Debug.Log("init");
        }

        static public void ShowWindow () {
            // Get existing open window or if none, make a new one:
            GetWindow<HFTWindow>();
        }

        private void OnEnable()
        {
            m_uiInfo = HFTUIInfo.Instance;

            EditorApplication.playmodeStateChanged += OnPlaymodeStateChange;
        }

        private void OnDestroy()
        {
            EditorApplication.playmodeStateChanged -= OnPlaymodeStateChange;

            Persist();
        }

        void Persist()
        {
            if (m_uiInfo != null) {
                m_uiInfo.Persist();
            }
        }

        void OnPlaymodeStateChange()
        {
            if (!EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Persist();
            }
        }

        void OnGUI()
        {
            m_uiInfo.DoGUI(this);
        }
    }
}  // namespace


