using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SessionManager))]
public class GenerationHandlerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var handler = target as SessionManager;
        
        if (handler == null) return;
        if (!Application.isPlaying) return;

        if (SessionManager.currentMode == SessionManager.DataMode.Idle)
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Start Recording"))
                handler.StartRecordSession();
            if (GUILayout.Button("Start Replaying"))
                handler.StartReplaySession();

            GUILayout.EndHorizontal();
        }
        else
        {
            if (GUILayout.Button($"Stop Current ({SessionManager.currentMode})"))
                handler.StopCurrentSession();
        }
    }
}