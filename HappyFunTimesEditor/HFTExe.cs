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
using HappyFunTimes;
using System.Collections;
using System.IO;

namespace HappyFunTimesEditor {

    class HFTExe {

        public static HFTExe Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = new HFTExe();
                }
                return s_instance;
            }
        }

        public string nodePath;
        public string startPath;
        public string exePath;
        public string hftPath;

        private static HFTExe s_instance;

        public bool GetHFTPath()
        {
            exePath = UnityEditor.EditorPrefs.GetString(HFTConstants.kExeKey);
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    {
                        bool ask = false;
                        if (System.String.IsNullOrEmpty(exePath) || !File.Exists(exePath)) {
                            ask = true;
                            object loc = LocalRegistryGetKey(
                                "Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Greggman HappyFunTimes",
                                "InstallLocation");
                            if (loc != null && !System.String.IsNullOrEmpty((string)loc)) {
                                exePath = System.IO.Path.Combine(StripQuotes((string)loc), "node.exe");
                                ask = !File.Exists(exePath);
                            }
                        }
                        if (ask)
                        {
                            return AskAboutHappyFunTimesApp();
                        }
                    }
                    if (exePath.EndsWith("start.js")) {
                        startPath = exePath;
                        nodePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(exePath), "node.exe");
                        hftPath = Path.Combine(Path.Combine(Path.GetDirectoryName(exePath), "cli"), "hft.js");
                    } else {
                        nodePath = exePath;
                        startPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(exePath), "start.js");
                        hftPath = Path.Combine(Path.Combine(Path.GetDirectoryName(exePath), "cli"), "hft.js");
                    }
                    break;
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    if (System.String.IsNullOrEmpty(exePath) || (!Directory.Exists(exePath) && !(File.Exists(exePath)))) {
                        exePath = "/Applications/HappyFunTimes.app";
                        if (!Directory.Exists(exePath) && !File.Exists(exePath)) {
                            return AskAboutHappyFunTimesApp();
                        }
                    }
                    if (exePath.EndsWith(".app"))
                    {
                        nodePath = Path.Combine(exePath, "MacOS/bin/node");
                        hftPath = Path.Combine(exePath, "MacOS/hft/cli/hft.js");
                        startPath = Path.Combine(exePath, "MacOS/hft/start.js");
                    }
                    else
                    {
                        nodePath = "/usr/local/bin/node";
                        hftPath = Path.Combine(Path.GetDirectoryName(exePath), "cli/hft.js");
                        startPath = Path.Combine(Path.GetDirectoryName(exePath), "start.js");
                    }
                    break;
                default:
                    Debug.LogError("running happyfuntimes from the editor is not supported on this platform yet");
                    return false;
            }
            return true;
        }

        bool Install()
        {
            Application.OpenURL("http://docs.happyfuntimes.net/docs/unity/install.html");
            Application.Quit();
            return false;
        }

        bool FindIt()
        {
            #if UNITY_EDITOR_OSX
            string path = UnityEditor.EditorUtility.OpenFilePanel(
                "Select HappyFunTimes",
                "/Applications",
                "");
            if (!System.String.IsNullOrEmpty(path)) {
                UnityEditor.EditorPrefs.SetString(HFTConstants.kExeKey, path);
                return GetHFTPath();
            }
            #elif UNITY_EDITOR_WIN
            string path = UnityEditor.EditorUtility.OpenFilePanel(
                "Select HappyFunTimes start.js",
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles),
                ".js");
            if (!System.String.IsNullOrEmpty(path)) {
                UnityEditor.EditorPrefs.SetString(HFTConstants.kExeKey, path);
                return GetHFTPath();
            }
            #endif
            return false;
        }

        bool AskAboutHappyFunTimesApp()
        {
            int result = UnityEditor.EditorUtility.DisplayDialogComplex(
                "HappyFunTimes",
                "Could not find HappyFunTimes",
                "Install It",
                "Tell Me Where It's Installed",
                "Quit");
            Debug.Log("result: " + result);
            switch (result) {
                case 0:  // Install It
                    return Install();
                case 1:  // Tell me where
                    return FindIt();
                case 2:  // Quit
                    return false;
                default:
                    Debug.LogError("Something went wrong :(");
                    return false;
            }
        }

        object LocalRegistryGetKey(string path, string sub)
        {
            object v = null;
            try
            {
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(path);
                if (key != null)
                {
                    v = key.GetValue(sub);
                }
            }
            catch (System.Exception ex)  //just for demonstration...it's always best to handle specific exceptions
            {
                Debug.LogException(ex);
            }
            return v;
        }

        string StripQuotes(string s)
        {
            if (s.StartsWith("\""))
            {
                s = s.Substring(1);
            }
            if (s.EndsWith("\""))
            {
                s = s.Substring(0, s.Length - 1);
            }
            return s;
        }

    }

} // namespace HappyFunTimesEditor
