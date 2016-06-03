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
    public class HFTExportWindow : EditorWindow
    {
        public ExportOptions exportOptions = new ExportOptions();
        static private HFTPlatforms s_installPlatforms;
        static private HFTPlatforms s_exportPlatforms;
        static private HFTPlatforms s_publishPlatforms;
        private SerializedObject m_platformsSerializedObject;
        private SerializedObject m_publishInfoSerializedObject;
        private Vector2 m_scrollPos = new Vector2(0, 0);
        public string buttonLabel = "";
        public string runLabel = "";
        private bool m_busy = false;
        private HFTPackageEditorHelper m_packageEditorHelper;

        void OnEnable()
        {
            s_installPlatforms = ScriptableObject.CreateInstance<HFTPlatforms>();
            s_exportPlatforms  = ScriptableObject.CreateInstance<HFTPlatforms>();
            s_publishPlatforms = ScriptableObject.CreateInstance<HFTPlatforms>();
        }

        void Init()
        {
            if (exportOptions.publishInfo == null)
            {
                exportOptions.publishInfo = ScriptableObject.CreateInstance<HFTPublishInfo>();
            }

            if (m_packageEditorHelper == null)
            {
                m_packageEditorHelper = HFTPackageEditorHelper.Instance;
            }

            if (m_platformsSerializedObject == null) {
                m_platformsSerializedObject = new UnityEditor.SerializedObject(exportOptions.platforms);
                m_publishInfoSerializedObject = new UnityEditor.SerializedObject(exportOptions.publishInfo);

                ExpandProperties(m_platformsSerializedObject);
                ExpandProperties(m_publishInfoSerializedObject);
            }
        }

        void ExpandProperties(SerializedObject so)
        {
            SerializedProperty sp = so.GetIterator();
            sp.isExpanded = true;
            bool more = sp.Next(true);
            int depth = sp.depth;
            while (more)
            {
                if (sp.depth == depth)
                {
                    sp.isExpanded = true;
                }
                more = sp.Next(true);
            }
        }

        void OnGUI()
        {
            if (m_busy)
            {
                EditorGUI.LabelField(new Rect(0, 0, EditorGUIUtility.currentViewWidth, position.height), runLabel, HFTGUIStyles.Center);
                EditorUtility.DisplayProgressBar(runLabel, runLabel, 0.25f);
                return;
            }
            EditorUtility.ClearProgressBar();

            // This is here because as we edit this source, when unity recompiles, code below
            // will fail because everything will have been seralized, purged, an restored.
            Init();

            EditorGUILayout.BeginVertical();
            m_scrollPos = EditorGUILayout.BeginScrollView(m_scrollPos, GUILayout.Width(EditorGUIUtility.currentViewWidth), GUILayout.Height (position.height));

            //options.exportOptions.install = EditorGUILayout.Toggle("Install to local HappyFunTimes", options.exportOptions.install);
            //EditorGUILayout.LabelField("Platforms");
            //++EditorGUI.indentLevel;
            //options.exportOptions.platforms.Windows = EditorGUILayout.Toggle("Windows", options.exportOptions.platforms.Windows);
            //options.exportOptions.platforms.Mac = EditorGUILayout.Toggle("Mac", options.exportOptions.platforms.Mac);
            //options.exportOptions.platforms.Linux = EditorGUILayout.Toggle("Linux", options.exportOptions.platforms.Linux);
            //--EditorGUI.indentLevel;

            switch (exportOptions.mode)
            {
                case ExportOptions.Mode.Publish:
                    {
        //                m_publishInfoSerializedObject.Update();
                        EditorGUILayout.LabelField("Github Login Info");
                        exportOptions.publishInfo.username = EditorGUILayout.TextField("Username", exportOptions.publishInfo.username);
                        exportOptions.publishInfo.password = EditorGUILayout.PasswordField("Password", exportOptions.publishInfo.password);
        //                m_publishInfoSerializedObject.ApplyModifiedProperties();
                        GUILayout.Box(GUIContent.none, HFTGUIStyles.EditorLine, GUILayout.ExpandWidth(true), GUILayout.Height(1f));
                        m_packageEditorHelper.DoGUI();
                    }
                    break;
                case ExportOptions.Mode.Install:
                    {
                        m_packageEditorHelper.DoGUI();
                    }
                    break;
                case ExportOptions.Mode.Export:
                    {
    //                m_platformsSerializedObject.Update();
    //                SerializedProperty sp = m_platformsSerializedObject.GetIterator();
    //                sp.NextVisible(true);
    //                EditorGUILayout.PropertyField(sp, true);
    //                m_platformsSerializedObject.ApplyModifiedProperties();
                        EditorGUILayout.LabelField("Platforms");
                        exportOptions.platforms.Windows = EditorGUILayout.Toggle("Windows", exportOptions.platforms.Windows);
                        exportOptions.platforms.Mac = EditorGUILayout.Toggle("Mac", exportOptions.platforms.Mac);
                        exportOptions.platforms.Linux = EditorGUILayout.Toggle("Linux", exportOptions.platforms.Linux);
                        m_packageEditorHelper.DoGUI();
                    }
                    break;
            }

            bool close = false;
            bool disabled = !exportOptions.platforms.HaveAtLeastOne() || EditorApplication.isPlaying;
            EditorGUI.BeginDisabledGroup(disabled);
            bool execute = GUILayout.Button(buttonLabel);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            if (execute)
            {
                Cleanup();
                m_busy = true;
                try
                {
                    close = ExportHFT();
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(ex);
                }
                m_busy = false;
            }
            if (close)
            {
                Close();
            }
        }

        void ExportWindowFunc(int windowId)
        {
            EditorGUILayout.BeginVertical();
            GUILayout.Label(runLabel);
            EditorGUILayout.EndVertical();
        }

        bool ExportHFT()
        {
            if (EditorApplication.isSceneDirty)
            {
                if (!EditorApplication.SaveCurrentSceneIfUserWantsTo())
                {
                    return false;
                }
            }
            Persist();
            string gameId = m_packageEditorHelper.GetString("gameId");
            if (exportOptions.mode == ExportOptions.Mode.Install)
            {
                HFTCmdRunner runner = new HFTCmdRunner();
                runner.HFTRun(new string[] { "get-info", gameId } );
                if (runner.ExitCode == 0)
                {
                    if (!EditorUtility.DisplayDialog("Overwrite " + gameId, "There is already a game installed with gameId \"" + gameId + "\". Are you sure you want to overwrite?", "Overwrite", "Cancel"))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!EditorUtility.DisplayDialog("Install " + gameId, "Install as \"" + gameId + "\"?", "Install", "Cancel"))
                    {
                        return false;
                    }
                }
            }

            HFTExport exporter = new HFTExport();
            if (!exporter.Export(new HFTExport.Options(exportOptions.platforms)))
            {
               EditorUtility.DisplayDialog("Error Exporting", exporter.errors, "Ok");
               return false;
            }
            else
            {
                switch (exportOptions.mode) {
                    case ExportOptions.Mode.Publish:
                        {
                            HFTCmdRunner publisher = new HFTCmdRunner();
                            List<string> args = new List<string>();
                            args.Add("publish");
                            args.Add("--user=" + exportOptions.publishInfo.username);
                            args.Add("--pass=" + exportOptions.publishInfo.password);
                            args.Add("--force");
                            args.Add("--no-export");
                            string url = System.Environment.GetEnvironmentVariable("HFT_ENDPOINT");
                            if (!String.IsNullOrEmpty(url))
                            {
                                args.Add("--endpoint=" + url);
                            }
                            string workPath = System.IO.Path.GetDirectoryName(Application.dataPath);
                            publisher.HFTRun(args.ToArray(), workPath);
                            if (publisher.ExitCode != 0)
                            {
                                EditorUtility.DisplayDialog("Error Installing " + gameId, publisher.stderr, "Ok");
                                return false;
                            }
                            else
                            {
                                EditorUtility.DisplayDialog("Publish", gameId + " published", "Ok");
                            }
                        }
                        break;
                    case ExportOptions.Mode.Install:
                        {
                            string platform = "";
                            switch (Application.platform)
                            {
                                case RuntimePlatform.WindowsEditor:
                                case RuntimePlatform.WindowsPlayer:
                                    platform = "win";
                                    break;
                                case RuntimePlatform.OSXEditor:
                                case RuntimePlatform.OSXPlayer:
                                    platform = "mac";
                                    break;
                                default:
                                    Debug.Log("install not implemented on this platform yet");
                                    return false;
                            }
                            if (!System.String.IsNullOrEmpty(platform))
                            {
                                HFTExport.ExportInfo exportInfo = null;
                                if (!exporter.exportInfoDict.TryGetValue(platform, out exportInfo))
                                {
                                    Debug.LogError("Unknown platform: " + platform);
                                    return false;
                                }
                                else
                                {
                                    HFTCmdRunner installer = new HFTCmdRunner();
                                    installer.HFTRun(new string[] { "install", "--upgrade", exportInfo.filename });
                                    if (installer.ExitCode != 0)
                                    {
                                        EditorUtility.DisplayDialog("Error Installing " + gameId, installer.stderr, "Ok");
                                        return false;
                                    }
                                    else
                                    {
                                        EditorUtility.DisplayDialog("Installed", gameId + " installed", "Ok");
                                    }
                                }
                            }
                        }
                        break;
                    case ExportOptions.Mode.Export:
                        {
                            string files = "Files:\n";
                            foreach (HFTExport.ExportInfo exportInfo in exporter.exportInfos)
                            {
                                files += exportInfo.filename + "\n";
                            }
                            EditorUtility.DisplayDialog("Exported " + gameId, files, "Ok");
                            HFTCmdRunner openFolder = new HFTCmdRunner();
                            openFolder.Open(new string[] { System.IO.Path.GetDirectoryName(exporter.exportInfos[0].filename), });
                        }
                        break;
                }
            }
            return true;
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

        void OnDestroy()
        {
            Cleanup();
        }

        [MenuItem ("Window/HappyFunTimes/Export for HappyFunTimes")]
        static void Export()
        {
            HFTExportWindow w = EditorWindow.GetWindow<HFTExportWindow>();
            w.buttonLabel = "Export";
            w.runLabel = "Exporting";
            w.exportOptions.mode = ExportOptions.Mode.Export;
            w.exportOptions.platforms = s_exportPlatforms;
            w.Init();
        }

        [MenuItem ("Window/HappyFunTimes/Install into Local HappyFunTimes")]
        static void Install()
        {
            HFTExportWindow w = EditorWindow.GetWindow<HFTExportWindow>();
            w.buttonLabel = "Install";
            w.runLabel = "Installing";
            w.exportOptions.mode = ExportOptions.Mode.Install;
            w.exportOptions.platforms = s_installPlatforms;
            switch (Application.platform) {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    s_installPlatforms.None();
                    s_installPlatforms.Windows = true;
                    break;
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    s_installPlatforms.None();
                    s_installPlatforms.Mac = true;
                    break;
            }
            w.Init();
        }

        [MenuItem ("Window/HappyFunTimes/Publish to SuperHappyFunTimes.net")]
        static void Publish()
        {
            HFTExportWindow w = EditorWindow.GetWindow<HFTExportWindow>();
            w.buttonLabel = "Publish";
            w.runLabel = "Publishing";
            w.exportOptions.mode = ExportOptions.Mode.Publish;
            w.exportOptions.platforms = s_publishPlatforms;
            w.Init();
        }
    }

    public class HFTPublishInfo : ScriptableObject
    {
        public string username = "";
        public string password = "";

        void OnEnable()
        {
            hideFlags = HideFlags.DontSave;
        }
    }

    public class ExportOptions
    {
        public enum Mode
        {
            Export,
            Install,
            Publish
        }
        public Mode mode = Mode.Export;
        public HFTPlatforms platforms = null;
        public HFTPublishInfo publishInfo = null;
    }


}  // namespace HappyFunTimesEditor


