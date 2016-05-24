using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(HappyFunTimes.HFTInstructions))]
[CanEditMultipleObjects]
public class HFTInstructionsEditor : Editor {

    public override void OnInspectorGUI() {
        string help =
@"You can set these from the command line with

--hft-instructions=""your instructions""

and

--hft-instuctions-bottom

Leave 'show' unchecked except for testing.";
        EditorGUILayout.HelpBox(help, MessageType.Info);
        GUILayout.Space(5);
        DrawDefaultInspector();
    }
}


