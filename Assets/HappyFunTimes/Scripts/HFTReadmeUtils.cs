using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HappyFunTimes {

    public class HFTReadmeUtils {

        const string kLastScene = "happyFunTimesReadmeSceneHash";
        const string kWasNotEditor = "happyFunTimesReadmeWasNotEditor";

        #if UNITY_EDITOR
        static Regex m_tickRE = new Regex("`(.*?)`", RegexOptions.CultureInvariant | RegexOptions.Singleline);
        static Regex m_tripleTickRE = new Regex("```(.*?)```", RegexOptions.CultureInvariant | RegexOptions.Singleline);
        static Regex m_tripleTickPlaceholderRE = new Regex(@"%%TRIPLETICK%%", RegexOptions.CultureInvariant | RegexOptions.Singleline);
        static Regex m_boldRE = new Regex(@"\*\*(.+?)\*\*", RegexOptions.CultureInvariant | RegexOptions.Singleline);
        static Regex m_1HashRE = new Regex(@"^\# *(.+?)$", RegexOptions.CultureInvariant | RegexOptions.Multiline);
        static Regex m_2HashRE = new Regex(@"^\#\# *(.+?)$", RegexOptions.CultureInvariant | RegexOptions.Multiline);
        static Regex m_3HashRE = new Regex(@"^\#\#\# *(.+?)$", RegexOptions.CultureInvariant | RegexOptions.Multiline);
        static Regex m_4HashRE = new Regex(@"^\#\#\#\# *(.+?)$", RegexOptions.CultureInvariant | RegexOptions.Multiline);
        static Regex m_5HashRE = new Regex(@"^\#\#\#\#\# *(.+?)$", RegexOptions.CultureInvariant | RegexOptions.Multiline);
        static Regex m_linkRE = new Regex(@"\[(.*?)\]\((.*?)\)", RegexOptions.CultureInvariant | RegexOptions.Multiline);

        class Snippets {
            public string startTick;
            public string endTick;
            public string startTripleTick;
            public string endTripleTick;
            public string startBold;
            public string endBold;
            public string startOneHash;
            public string endOneHash;
            public string startTwoHash;
            public string endTwoHash;
            public string startThreeHash;
            public string endThreeHash;
            public string startFourHash;
            public string endFourHash;
            public string startFiveHash;
            public string endFiveHash;
        }

        public class Link {
            public Link(string description, string url)
            {
                this.description = description;
                this.url = url;
            }
            public string description;
            public string url;
        }
        public class Markdownish {
            public Markdownish(string richText, Link[] links)
            {
                this.richText = richText;
                this.links = links;
            }

            public string richText;
            public Link[] links;
        }

        static Snippets s_lightTheme;
        static Snippets s_darkTheme;
        static Snippets s_currentTheme;

        static bool isDarkTheme()
        {
            return UnityEditor.EditorGUIUtility.isProSkin;
        }

        static void SetCurrentTheme()
        {
            if (s_lightTheme == null)
            {
                s_lightTheme = new Snippets();
                s_darkTheme = new Snippets();

                s_lightTheme.startTick       = "<color=blue>";   s_lightTheme.endTick       = "</color>";
                s_lightTheme.startTripleTick = "<color=purple>"; s_lightTheme.endTripleTick = "</color>";
                s_lightTheme.startBold       = "<color=red><b>"; s_lightTheme.endBold       = "</b></color>";
                s_lightTheme.startOneHash    = "<size=24><b>";   s_lightTheme.endOneHash    = "</b></size>";
                s_lightTheme.startTwoHash    = "<size=18><b>";   s_lightTheme.endTwoHash    = "</b></size>";
                s_lightTheme.startThreeHash  = "<size=14><b>";   s_lightTheme.endThreeHash  = "</b></size>";
                s_lightTheme.startFourHash   = "<size=12><b>";   s_lightTheme.endFourHash   = "</b></size>";
                s_lightTheme.startFiveHash   = "<size=11><b>";   s_lightTheme.endFiveHash   = "</b></size>";

                s_darkTheme.startTick       = "<color=cyan>";    s_darkTheme.endTick       = "</color>";
                s_darkTheme.startTripleTick = "<color=magenta>"; s_darkTheme.endTripleTick = "</color>";
                s_darkTheme.startBold       = "<color=red><b>";  s_darkTheme.endBold       = "</b></color>";
                s_darkTheme.startOneHash    = "<size=24><b>";    s_darkTheme.endOneHash    = "</b></size>";
                s_darkTheme.startTwoHash    = "<size=18><b>";    s_darkTheme.endTwoHash    = "</b></size>";
                s_darkTheme.startThreeHash  = "<size=14><b>";    s_darkTheme.endThreeHash  = "</b></size>";
                s_darkTheme.startFourHash   = "<size=12><b>";    s_darkTheme.endFourHash   = "</b></size>";
                s_darkTheme.startFiveHash   = "<size=11><b>";    s_darkTheme.endFiveHash   = "</b></size>";
            }
            s_currentTheme = isDarkTheme() ? s_darkTheme : s_lightTheme;
        }

        static public void CloseReadme(Component component)
        {
            System.Type t = GetWindowType();
            if (t != null)
            {
                UnityEditor.EditorWindow w = t.GetMethod("GetInstanceIfExists").Invoke(null, null) as UnityEditor.EditorWindow;
                if (w != null)
                {
                    t.GetMethod("CloseIfOurs").Invoke(w, new object[]{ component });
                }
            }
        }

        static string ReplaceTick(Match m)
        {
            return s_currentTheme.startTick + m.Groups[1].Value + s_currentTheme.endTick;
        }

        static string ReplaceBold(Match m)
        {
            return s_currentTheme.startBold + m.Groups[1].Value + s_currentTheme.endBold;
        }

        static string Replace1Hash(Match m)
        {
            return s_currentTheme.startOneHash + m.Groups[1].Value + s_currentTheme.endOneHash;
        }

        static string Replace2Hash(Match m)
        {
            return s_currentTheme.startTwoHash + m.Groups[1].Value + s_currentTheme.endTwoHash;
        }

        static string Replace3Hash(Match m)
        {
            return s_currentTheme.startThreeHash + m.Groups[1].Value + s_currentTheme.endThreeHash;
        }

        static string Replace4Hash(Match m)
        {
            return s_currentTheme.startFourHash + m.Groups[1].Value + s_currentTheme.endFourHash;
        }

        static string Replace5Hash(Match m)
        {
            return s_currentTheme.startFiveHash + m.Groups[1].Value + s_currentTheme.endFiveHash;
        }


        static public Markdownish MarkdownishToRichText(string markdownish)
        {
            SetCurrentTheme();
            List<Link> links = new List<Link>();
            List<string> tripleTicks = new List<string>();

            string s = markdownish.Replace("\r", "");
            // Pull out triple tick areas
            s = m_tripleTickRE.Replace(s, (Match m) => {
                tripleTicks.Add(m.Groups[1].Value);
                return "%%TRIPLETICK%%";
            });
            // change single \n to space
            s = s.Replace("\n\n", "%%EOL%%").Replace("\n", " ").Replace("%%EOL%%", "\n\n");
            // Put the triple ticks back
            s = m_tripleTickPlaceholderRE.Replace(s, (Match m) => {
                string replacement = s_currentTheme.startTripleTick + tripleTicks[0] + s_currentTheme.endTripleTick;
                tripleTicks.RemoveAt(0);
                return replacement;
            });
            s = m_tickRE.Replace(s, ReplaceTick);
            s = m_boldRE.Replace(s, ReplaceBold);
            s = m_5HashRE.Replace(s, Replace5Hash);
            s = m_4HashRE.Replace(s, Replace4Hash);
            s = m_3HashRE.Replace(s, Replace3Hash);
            s = m_2HashRE.Replace(s, Replace2Hash);
            s = m_1HashRE.Replace(s, Replace1Hash);
            s = m_linkRE.Replace(s, (Match m) => {
                string description = m.Groups[1].Value;
                string url = m.Groups[2].Value;
                try
                {
                    new System.Uri(url);
                }
                catch (System.Exception)
                {
                    // NOTE a url = no link
                    return m.Groups[0].Value;
                }
                links.Add(new Link(description, url));
                return description + "[" + links.Count.ToString() + "]";
            });

            return new Markdownish(s, links.ToArray());
        }

        static System.Type GetWindowType() {
            string[] names = new string[]
            {
                "HappyFunTimesEditor.HFTReadmeWindow",
                "HappyFunTimesEditor.HFTReadmeWindow, Assembly-CSharp-firstpass, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null",
                "HappyFunTimesEditor.HFTReadmeWindow, Assembly-CSharp-Editor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null",
                "HFTReadmeWindow",
            };
            foreach (string name in names)
            {
                System.Type t = System.Type.GetType(name);
                if (t != null)
                {
                    return t;
                }
            }
            return null;
        }

        static string GetSceneHash()
        {
            // NOTE: I tried getting the list of loaded scenes but it's 0
            // when I get here. But active scene seems to be valid
            string sceneHash = "";
            var scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
            if (!String.IsNullOrEmpty(scene.path))
            {
                sceneHash = scene.path;
            }
            else if (!String.IsNullOrEmpty(scene.name))
            {
                sceneHash = scene.name;
            }
            return sceneHash;
        }

        // This is an attempt to not make the README too annoying.
        // Without this, every time the scene loads (which means every time
        // you press stop after runnign the game) the readme would re-appear
        // It's in the way so you close it only to have it show up again
        //
        // With this it will only open if the user loads a new scene. Not sure
        // that's the best but I want the readme to appear.
        static bool IsSameSceneAsLastTime()
        {
            string sceneHash = GetSceneHash();
            string oldSceneHash = PlayerPrefs.GetString(kLastScene, "");
            if (sceneHash.Equals(oldSceneHash))
            {
                return true;
            }

            return false;
        }

        static void RememberScene()
        {
            PlayerPrefs.SetString(kLastScene, GetSceneHash());
        }

        static bool ShouldShowReadme(bool force)
        {
            bool wasEditor = PlayerPrefs.GetInt(kWasNotEditor, 0) > 0;
            bool badState =
                UnityEditor.BuildPipeline.isBuildingPlayer ||
                Application.isPlaying ||
                UnityEditor.EditorApplication.isPlaying ||
                UnityEditor.EditorApplication.isUpdating ||
                UnityEditor.EditorApplication.isCompiling ||
                UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode;
            if (wasEditor || badState)
            {
                PlayerPrefs.SetInt(kWasNotEditor, badState ? 1 : 0);
                return false;
            }

            if (force)
            {
                return true;
            }

            bool isSameScene = IsSameSceneAsLastTime();
            if (isSameScene)
            {
                return false;
            }
            RememberScene();

            return true;
        }

        static public void ShowReadme(string name, string text, bool richText, bool markdownish, Component component, bool force)
        {
            if (!ShouldShowReadme(force))
            {
                return;
            }

            System.Type t = GetWindowType();
            if (t != null)
            {
                UnityEditor.EditorWindow w = t.GetMethod("GetInstance").Invoke(null, null) as UnityEditor.EditorWindow;
                if (w != null)
                {
                    t.GetMethod("SetContent").Invoke(w, new object[]{ name, text, richText, markdownish, component });
                    w.ShowUtility();
                }
            }
        }
        #endif
    }
}


