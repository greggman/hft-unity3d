using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(HappyFunTimes.HFTReadmeFile))]
[CanEditMultipleObjects]
public class HFTReadmeFileEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        if (GUILayout.Button("Show Readme")) {
            HappyFunTimes.HFTReadmeFile readme = target as HappyFunTimes.HFTReadmeFile;
            readme.ShowReadme(true);
        }
    }
}


