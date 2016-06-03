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
    public class HFTWindow : EditorWindow
    {
//        [SerializeField]
        private HFTPackageEditorHelper m_packageEditorHelper;

        private Vector2 m_scrollPos = new Vector2(0, 0);

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
            m_packageEditorHelper = HFTPackageEditorHelper.Instance;

            EditorApplication.playmodeStateChanged += OnPlaymodeStateChange;
        }

        private void OnDestroy()
        {
            EditorApplication.playmodeStateChanged -= OnPlaymodeStateChange;

            Persist();
            Cleanup();
        }

        void Cleanup()
        {
            if (m_packageEditorHelper != null)
            {
                m_packageEditorHelper.Cleanup();
            }
        }

        void Persist()
        {
            if (m_packageEditorHelper != null) {
                m_packageEditorHelper.Persist();
            }
        }

        void OnPlaymodeStateChange()
        {
            if (!EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Persist();
                Cleanup();
            }
        }

        void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            m_scrollPos = EditorGUILayout.BeginScrollView(m_scrollPos, GUILayout.Width(EditorGUIUtility.currentViewWidth), GUILayout.Height (position.height));
            m_packageEditorHelper.DoGUI();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
    }
}  // namespace


