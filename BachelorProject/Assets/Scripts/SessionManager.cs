using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionManager : MonoBehaviour
{
    public static DateTime sessionStartTime;

    private void Start()
    {
        StartSession();
    }

    public static void StartSession()
    {
        sessionStartTime = DateTime.Now;
    }
}
