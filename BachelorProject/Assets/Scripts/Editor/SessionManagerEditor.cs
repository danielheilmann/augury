using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SessionManager))]
public class GenerationHandlerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var handler = target as SessionManager;
        if (handler != null)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Start Recording"))
                handler.StartRecordSession();
            if (GUILayout.Button("Start Replaying"))
                handler.StartReplaySession();
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Stop Current"))
                handler.StopCurrentSession();
            GUILayout.EndVertical();
        }

    }
}