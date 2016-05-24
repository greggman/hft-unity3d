using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(HappyFunTimes.HFTInstructions))]
[CanEditMultipleObjects]
public class HFTInstructionsEditor : Editor {
    Texture2D m_dark;

    public void OnEnable()
    {
        m_dark = HappyFunTimes.HFTUtil.MakeColor(new Color(0.2f, 0.2f, 0.2f));
    }

    public override void OnInspectorGUI() {
        GUIStyle style = new GUIStyle(GUI.skin.GetStyle("Label"));
        style.wordWrap = true;
        style.richText = true;
        style.normal.background = m_dark;
        style.normal.textColor = Color.white;
        GUILayout.Label("You can set these from the command line with <color=ctan>--hft-instructions=\"your instructions\"</color> and <color=cyan>--hft-instuctions-bottom</color>\n\nOr of course you can design your own interface", style);
        GUILayout.Space(5);
        DrawDefaultInspector();
    }
}


