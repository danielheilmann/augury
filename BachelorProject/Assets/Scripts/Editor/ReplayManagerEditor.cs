using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ReplayManager))]
public class ReplayManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var handler = target as ReplayManager;

        if (handler == null) return;
        if (SessionManager.currentMode != SessionManager.DataMode.Replay) return;

        if (handler.isActivelyReplaying)
        {
            if (handler.isPaused)
            {
                if (GUILayout.Button("Unpause Replay"))
                    handler.UnpauseReplay();
            }
            else
            {
                if (GUILayout.Button("Pause Replay"))
                    handler.PauseReplay();
            }
        }
        else
        {
            if (handler.validSessions != null && handler.validSessions.Count > 0)
            {
                int selectedIndex = handler.validSessions.IndexOf(handler.selectedSession);
                int newSelectedIndex = EditorGUILayout.Popup("Select Session", selectedIndex,
                    handler.validSessions.ConvertAll(s => s.ToString()).ToArray());

                if (newSelectedIndex != selectedIndex)
                {
                    handler.SelectSession(newSelectedIndex);
                }
            }
            else
            {
                EditorGUILayout.LabelField("No valid sessions available");
            }

            if (handler.selectedSession != null)
                if (GUILayout.Button("Load selected Session & Begin Replay"))
                {
                    handler.LoadSelectedSession();
                    handler.BeginReplay();
                }
        }
    }
}