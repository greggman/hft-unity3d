using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(HappyFunTimes.HFTReadme))]
[CanEditMultipleObjects]
public class HFTReadmeEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        if (GUILayout.Button("Show Readme")) {
            HappyFunTimes.HFTReadme readme = target as HappyFunTimes.HFTReadme;
            readme.ShowReadme(true);
        }
    }
}


