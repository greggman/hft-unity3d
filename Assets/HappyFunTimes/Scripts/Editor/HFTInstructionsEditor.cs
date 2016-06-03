using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(HappyFunTimes.HFTInstructions))]
[CanEditMultipleObjects]
public class HFTInstructionsEditor : Editor {

    public override void OnInspectorGUI() {
        string help =
@"You can set these from the command line with

--instructions=""your instructions""

and

--bottom

Leave 'show' unchecked except for testing.";
        EditorGUILayout.HelpBox(help, MessageType.Info);
        GUILayout.Space(5);
        DrawDefaultInspector();
    }
}


