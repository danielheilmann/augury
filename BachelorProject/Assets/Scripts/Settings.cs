using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    public const bool TestModeEnabled = true;
    public static bool OpenExplorerOnSave = false;
    public static float ExpectedSessionRuntimeInMinutes = 1f;  //< This value is used to calculate the initial buffer size for the gazepoint dictionary based on session duration and tick rate.
    /// <summary> Turning this off disables pretty formatting, which will reduce file size and is therefore recommended. Possible values: 0 (off), 1 (on) </summary>
    public static int PrettyJSONExportIndent = 1;

    // private void Start()
    // {
    //     // Set up Frame Limiter
    //     // Application.targetFrameRate = 60;
    //     // QualitySettings.vSyncCount = 1;
    // }
}
