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

You can also set them from the Window->HappyFunTimes->Settings menu.

Leave 'show' unchecked except for testing.

(WIFI) will be automatically changed to the local Wifi name";
        EditorGUILayout.HelpBox(help, MessageType.Info);
        GUILayout.Space(5);
        DrawDefaultInspector();
    }
}


