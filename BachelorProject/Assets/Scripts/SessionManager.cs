using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionManager : MonoBehaviour
{
    public static DateTime sessionStartTime;
    public static string sessionStartTimeString => sessionStartTime.ToString(FileSystemHandler.TimestampFormat, System.Globalization.CultureInfo.InvariantCulture);
    public static string sessionTitle;

    private void Start() //TODO: Should trigger OnRecordingStart instead
    {
        StartSession();
    }

    public static void StartSession()
    {
        sessionStartTime = DateTime.Now;
        sessionTitle = $"Session {sessionStartTime.ToString(FileSystemHandler.TimestampFormat, System.Globalization.CultureInfo.InvariantCulture)}";
    }
}
