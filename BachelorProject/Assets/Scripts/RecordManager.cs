using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class RecordManager : MonoBehaviour
{
    private const string FileNameSafeTimeFormat = "yyyy-MM-dd_HH-mm-ss"; //< Ensures system file name compliance by using dashes as delimiter characters.

    public static RecordManager Instance { get; private set; } //< To ensure sessionStartTime is not accidentally overwritten during e.g. a scene change (into a scene that also contains a SessionManager).
    public static DateTime sessionStartTime { get; private set; }
    public static string sessionIdentifier { get; private set; }

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

    private void OnEnable()
    {
        SessionManager.OnRecordStart.AddListener(OnStart);
        SessionManager.OnRecordStop.AddListener(OnStop);
    }

    private void OnDisable()
    {
        SessionManager.OnRecordStart.RemoveListener(OnStart);
        SessionManager.OnRecordStop.RemoveListener(OnStop);
    }

    public void OnStart()
    {
        Timer.Instance?.Begin();

        sessionStartTime = Timer.latestTimestamp;
        sessionIdentifier = $"Session {sessionStartTime.ToString(FileNameSafeTimeFormat, System.Globalization.CultureInfo.InvariantCulture)}"; //! Pay attention that this is format is not the same as the FileSystemHandler.TimestampFormat, as that one is not "file name compliant"! (will lead to error due to the colons)
    }

    public void OnStop()
    {
        Timer.Instance?.Stop();
    }
}
