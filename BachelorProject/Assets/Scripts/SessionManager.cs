using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionManager : MonoBehaviour
{
    public static DateTime sessionStartTime;
    public static string sessionTitle;

    private void Start()
    {
        StartSession();
    }

    public static void StartSession()
    {
        sessionStartTime = DateTime.Now;
        sessionTitle = $"Session {sessionStartTime:yyyy-MM-dd HH-mm-ss}";   //< Simplified version of "sessionStartTime.ToString("yyyy-MM-dd HH-mm-ss")"
    }
}
