using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class SessionManager : MonoBehaviour
{
    private enum DataMode { Record, Replay }

    private const string FileNameSafeTimeFormat = "yyyy-MM-dd HH-mm-ss"; //< Ensures system file name compliance by using dashes as delimiter characters.

    public static SessionManager Instance { get; private set; } //< Is important to ensure the sessionStartTime is not overwritten after e.g. a scene change (into a scene that also contains a SessionManager).
    public static UnityEvent OnRecordStart { get; private set; } = new();
    public static UnityEvent OnRecordStop { get; private set; } = new();
    public static UnityEvent OnReplayStart { get; private set; } = new();
    public static UnityEvent OnReplayStop { get; private set; } = new();
    public static DateTime sessionStartTime { get; private set; }
    public static string sessionIdentifier { get; private set; }

    [SerializeField] private DataMode mode = DataMode.Record;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Debug.LogError($"{this} cannot set itself as instance as one has already been set by {Instance.gameObject}. Deleting self.");
            Destroy(this);
        }
    }

    private void Start()
    {
        switch (mode)
        {
            case DataMode.Record:
                StartRecording();
                break;
            case DataMode.Replay:
                StartReplaying();
                break;
        }
    }

    private void StartRecording()
    {
        Debug.Log($"Started Record Mode.");

        sessionStartTime = Timer.latestTimestamp;
        sessionIdentifier = $"Session {sessionStartTime.ToString(FileNameSafeTimeFormat, System.Globalization.CultureInfo.InvariantCulture)}"; //! Pay attention that this is format is not the same as the FileSystemHandler.TimestampFormat, as that one is not "file name compliant"! (will lead to error due to the colons)

        OnRecordStart.Invoke();
    }

    private void StartReplaying()
    {
        Debug.Log($"Started Replay Mode.");

        // Execute any other code.

        OnReplayStart.Invoke();
    }
}
