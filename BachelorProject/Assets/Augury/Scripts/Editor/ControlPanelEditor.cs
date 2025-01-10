using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Contains buttons for all the CustomInspector / In-Editor functionalities of the Managers (Session, Replay, Record, Game) to provide a more centralized control panel.
/// </summary>
[CustomEditor(typeof(ControlPanel))]
public class ControlPanelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var handler = target as ControlPanel;
        if (handler == null) return;

        if (!Application.isPlaying) return;

        SessionManager sessionManager = SessionManager.Instance;
        GameManager gameManager = GameManager.Instance;
        ReplayManager replayManager = ReplayManager.Instance;

        switch (SessionManager.currentMode)
        {
            #region Idle
            case SessionManager.DataMode.Idle:
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Start Recording"))
                    sessionManager.StartRecordSession();

                if (GUILayout.Button("Start Replaying"))
                    sessionManager.StartReplaySession();

                GUILayout.EndHorizontal();

                GUILayout.Space(10);
                if (GUILayout.Button("Start Game (without Replay / Record Session)"))
                    gameManager.Initialize();
                break;
            #endregion

            #region Record
            case SessionManager.DataMode.Record:
                if (GUILayout.Button($"Stop Current ({SessionManager.currentMode})"))
                    sessionManager.StopCurrentSession();
                break;
            #endregion

            #region Replay
            case SessionManager.DataMode.Replay:
                if (GUILayout.Button($"Stop Current ({SessionManager.currentMode})"))
                    sessionManager.StopCurrentSession();

                if (replayManager.isActivelyReplaying)
                {
                    replayManager.timeline.speedMultiplier = EditorGUILayout.Slider("Playback Speed", replayManager.timeline.speedMultiplier, 0.1f, 10f);

                    if (replayManager.isPaused)
                    {
                        if (GUILayout.Button("Unpause Replay"))
                            replayManager.UnpauseReplay();
                    }
                    else
                    {
                        if (GUILayout.Button("Pause Replay"))
                            replayManager.PauseReplay();
                    }
                    break;
                }

                if (replayManager.validSessions == null || replayManager.validSessions.Count <= 0)
                    EditorGUILayout.LabelField("No valid sessions available");
                else
                {
                    int selectedIndex = replayManager.validSessions.IndexOf(replayManager.selectedSession);
                    int newSelectedIndex = EditorGUILayout.Popup("Select Session", selectedIndex,
                        replayManager.validSessions.ConvertAll(s => s.ToString()).ToArray());

                    if (newSelectedIndex != selectedIndex)
                        replayManager.SelectSession(newSelectedIndex);
                }

                if (replayManager.selectedSession != null)
                    if (GUILayout.Button("Load selected Session & Begin Replay"))
                    {
                        replayManager.LoadSelectedSession();
                        replayManager.BeginReplay();
                    }
                break;
                #endregion

        }
    }
}
