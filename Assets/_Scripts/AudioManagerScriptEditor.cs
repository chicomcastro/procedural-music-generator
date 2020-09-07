using UnityEditor;
using UnityEngine;

namespace PMM.Demo
{
    [CustomEditor(typeof(AudioManager))]
    public class AudioManagerScriptEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            AudioManager myScript = (AudioManager)target;
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate Melody"))
            {
                myScript.GenerateMelody();
            }
            GUILayout.EndHorizontal();
        }

    }
}
