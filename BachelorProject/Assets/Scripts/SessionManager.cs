using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionManager : MonoBehaviour
{
    public static DateTime sessionStartTime;
    /// <summary>
    /// Ensures system file name compliancy by using the custom format "yyyy-MM-dd HH-mm-ss".
    /// </summary>
    public static string sessionTitle;

    private void Start() //TODO: Should trigger OnRecordingStart instead
    {
        StartSession();
    }

    public static void StartSession()
    {
        sessionStartTime = DateTime.Now;
        sessionTitle = $"Session {sessionStartTime.ToString("yyyy-MM-dd HH-mm-ss", System.Globalization.CultureInfo.InvariantCulture)}"; //! Pay attention that this is format is not the same as the FileSystemHandler.TimestampFormat, as that one is not "file name compliant"! (will lead to error due to the colons)
    }
}
