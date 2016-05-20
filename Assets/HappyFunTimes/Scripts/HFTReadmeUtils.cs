using UnityEngine;
using System;
using System.Collections;

namespace HappyFunTimes {

    public class HFTReadmeUtils {

        #if UNITY_EDITOR
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

        static public void ShowReadme(string name, string text, bool richText, Component component)
        {
            System.Type t = GetWindowType();
            if (t != null)
            {
                UnityEditor.EditorWindow w = t.GetMethod("GetInstance").Invoke(null, null) as UnityEditor.EditorWindow;
                if (w != null)
                {
                    t.GetMethod("SetContent").Invoke(w, new object[]{ name, text, richText, component });
                    w.ShowUtility();
                }
            }
        }
        #endif
    }
}


