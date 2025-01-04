using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(2)]
public class SessionManager : MonoBehaviour
{
    public enum DataMode { Idle, Record, Replay }
    public static UnityEvent OnRecordStart = new();
    public static UnityEvent OnRecordStop = new();
    public static UnityEvent OnReplayStart = new();
    public static UnityEvent OnReplayStop = new();

    public static DataMode currentMode = DataMode.Idle;
    private static bool wasRecordingInterrupted = false; //< As a check when entering a new scene. If the earlier session was interrupted by SceneChange, a new one should be started right after the scene change is complete. 

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    private void Start()
    {
        if (wasRecordingInterrupted)
            StartRecordSession();
    }
    private void OnApplicationQuit() => StopCurrentSession(); //< Is executed before OnDestroy and will therefore close the session before the OnDestroy-Fallbacks of DynamicObject and GazePointManager activate.
    private void OnDestroy() //< Will be triggered OnSceneChange to make sure the current scene data is saved.
    {
        if (currentMode == DataMode.Record)
            wasRecordingInterrupted = true;
        StopCurrentSession();
    }

    public void StartRecordSession()
    {
        if (currentMode != DataMode.Idle)
        {
            Debug.LogWarning($"Already in {currentMode} Mode. Please stop the current session before starting a new one.");
            return;
        }

        wasRecordingInterrupted = false;
        Debug.Log($"Starting Record Mode.");
        currentMode = DataMode.Record;
        OnRecordStart?.Invoke();
    }

    public void StartReplaySession()
    {
        if (currentMode != DataMode.Idle)
        {
            Debug.LogWarning($"Already in {currentMode} Mode. Please stop the current session before starting a new one.");
            return;
        }

        Debug.Log($"Starting Replay Mode.");
        currentMode = DataMode.Replay;
        OnReplayStart?.Invoke();
    }

    public void StopCurrentSession()
    {
        switch (currentMode)
        {
            case DataMode.Idle:
                Debug.Log($"Currently in \"Idle\". There is nothing to stop.");
                break;
            case DataMode.Record:
                StopRecording();
                break;
            case DataMode.Replay:
                StopReplaying();
                break;
        }
    }

    public void StopRecording()
    {
        Debug.Log($"Stopping Record Mode.");
        currentMode = DataMode.Idle;
        OnRecordStop?.Invoke();
    }

    public void StopReplaying()
    {
        Debug.Log($"Stopping Replay Mode.");
        currentMode = DataMode.Idle;
        OnReplayStop?.Invoke();
    }
}
