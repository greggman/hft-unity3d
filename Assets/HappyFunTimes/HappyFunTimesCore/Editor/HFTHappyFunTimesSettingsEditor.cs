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
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using HappyFunTimes;

namespace HappyFunTimesEditor
{
    [CustomEditor(typeof(HappyFunTimes.HFTHappyFunTimesSettings))]
    public class HFTHappyFunTimesSettingsEditor : Editor
    {
        // UnityEditor.EditorPrefs, UnityEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
        public override void OnInspectorGUI()
        {
            GUIStyle labelStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
            labelStyle.wordWrap = true;
            labelStyle.richText = true;
            GUIStyle textAreaStyle = new GUIStyle(GUI.skin.GetStyle("TextArea"));
            textAreaStyle.wordWrap = true;

            GUILayout.Label("These settings are used in editor only and only take effect when a game starts.", labelStyle);
            GUILayout.Space(5);

            bool showInstructions = HFTHappyFunTimesSettings.showInstructions;
            bool instructionsPosition = HFTHappyFunTimesSettings.instructionsPosition;
            string instructions = HFTHappyFunTimesSettings.instructions;
            bool installationMode = HFTHappyFunTimesSettings.installationMode;
            bool showMessages = HFTHappyFunTimesSettings.showMessages;
            string debug = HFTHappyFunTimesSettings.debug;

            bool newShowInstructions = GUILayout.Toggle(showInstructions, "Show Instructions");
            bool newInstructionsPosition = GUILayout.Toggle(instructionsPosition, "Instructions on bottom");
            string newInstructions;

            using (var v = new EditorGUILayout.VerticalScope ("Box"))
            {
                GUILayout.Label("Instructions:");
                newInstructions = EditorGUILayout.TextArea(instructions, textAreaStyle, GUILayout.Height(100));
                GUILayout.Label(@"example:

connect to (WIFI) and go to happyfuntimes.net

* (WIFI) will be replaced by local wifi.
* rich text codes allowed
  eg: < color=red >msg< /color >

<b>NOTE: you must have an HFTInstructions component in your scene</b>", labelStyle);
            }
            GUILayout.Space(20);
            bool newInstallationMode = GUILayout.Toggle(installationMode, "Installation Mode");
            bool newShowMessages = GUILayout.Toggle(showMessages, "show messages [game <-> controller]");

            string newDebug;
            using (var v = new EditorGUILayout.VerticalScope ("Box"))
            {
                GUILayout.Label("Debug:");
                newDebug = EditorGUILayout.TextArea(debug, textAreaStyle, GUILayout.Height(100));
                GUILayout.Label(@"Example:
name1
name2
prefix1*
prefix2*

'*' = all", labelStyle);
            }

            HFTHappyFunTimesSettings.showInstructions = newShowInstructions;
            HFTHappyFunTimesSettings.instructionsPosition = newInstructionsPosition;
            HFTHappyFunTimesSettings.instructions = newInstructions;
            HFTHappyFunTimesSettings.installationMode = newInstallationMode;
            HFTHappyFunTimesSettings.showMessages = newShowMessages;
            HFTHappyFunTimesSettings.debug = newDebug;
        }
    }

}  // namespace HappyFunTimesEditor



