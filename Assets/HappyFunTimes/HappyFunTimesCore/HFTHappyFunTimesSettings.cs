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
using UnityEngine;
using HappyFunTimes;

namespace HappyFunTimes
{
    public class HFTHappyFunTimesSettings : ScriptableObject
    {
        public const string kHappyFunTimesShowInstructionsKey = "HappyFunTimes.ShowInstructions";
        public const string kHappyFunTimesInstructionsKey = "HappyFunTimes.Instructions";
        public const string kHappyFunTimesInstructionsPositionKey = "HappyFunTimes.InstructionsPosition";
        public const string kHappyFunTimesInstallationModeKey = "HappyFunTimes.InstallationMode";
        public const string kHappyFunTimesShowMessagesKey = "HappyFunTimes.ShowMessages";
        public const string kHappyFunTimesDebugKey = "HappyFunTimes.Debug";

        static HFTHappyFunTimesSettings s_instance;

        static public bool showInstructions
        {
            get
            {
                return GetBool(kHappyFunTimesShowInstructionsKey);
            }
            set
            {
                UpdateIfChanged(value, kHappyFunTimesShowInstructionsKey);
            }
        }

        static public bool instructionsPosition
        {
            get
            {
                return GetBool(kHappyFunTimesInstructionsPositionKey);
            }
            set
            {
                UpdateIfChanged(value, kHappyFunTimesInstructionsPositionKey);
            }
        }

        static public string instructions
        {
            get
            {
                return GetString(kHappyFunTimesInstructionsKey);
            }
            set
            {
                UpdateIfChanged(value, kHappyFunTimesInstructionsKey);
            }
        }

        static public bool showMessages
        {
            get
            {
                return GetBool(kHappyFunTimesShowMessagesKey);
            }
            set
            {
                UpdateIfChanged(value, kHappyFunTimesShowMessagesKey);
            }
        }

        static public bool installationMode
        {
            get
            {
                return GetBool(kHappyFunTimesInstallationModeKey);
            }
            set
            {
                UpdateIfChanged(value, kHappyFunTimesInstallationModeKey);
            }
        }

        static public string debug
        {
            get
            {
                return GetString(kHappyFunTimesDebugKey);
            }
            set
            {
                UpdateIfChanged(value, kHappyFunTimesDebugKey);
            }
        }

        static void UpdateIfChanged(string newValue, string key)
        {
            string oldValue = GetString(key);
            if (newValue != oldValue)
            {
                PlayerPrefs.SetString(key, newValue);
            }
        }

        static void UpdateIfChanged(bool newValue, string key)
        {
            bool oldValue = GetBool(key);
            if (newValue != oldValue)
            {
                PlayerPrefs.SetInt(key, newValue ? 1 : 0);
            }
        }

        static bool GetBool(string key) {
            return Application.isEditor && PlayerPrefs.GetInt(key, 0) != 0;
        }

        static string GetString(string key) {
            return Application.isEditor ? PlayerPrefs.GetString(key, "") : "";
        }

        public static HFTHappyFunTimesSettings GetInstance()
        {
            if (s_instance == null)
            {
                s_instance = ScriptableObject.FindObjectOfType<HFTHappyFunTimesSettings>();
                if (s_instance == null)
                {
                    s_instance = ScriptableObject.CreateInstance<HFTHappyFunTimesSettings>();
                    s_instance.name = "HappyFunTimes Settings";
                }
            }
            return s_instance;
        }
    }

}  // namespace HappyFunTimesEditor



