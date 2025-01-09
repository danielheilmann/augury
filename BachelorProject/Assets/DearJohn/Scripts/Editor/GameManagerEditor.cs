using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var handler = target as GameManager;
        if (handler == null) return;

        if (!Application.isPlaying) return;

        GUIContent buttonContent = new GUIContent("Start Game (without Replay / Record Session)", "Start Game without Replay / Record Session");
        if (GUILayout.Button(buttonContent))
            handler.Initialize();

    }
}