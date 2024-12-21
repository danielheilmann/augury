using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    public static bool TestModeEnabled = true;  //< Only static (and not const), because otherwise Unity complains about unreachable code
    public static bool OpenExplorerOnSave = false;
    /// <summary> This value is used to calculate the initial buffer size for the gazepoint dictionary (in <see cref="GazePointManager"/>) based on session duration and tick rate. </summary>
    public static float ExpectedSessionRuntimeInMinutes = 1f;
    /// <summary> Turning this off disables pretty formatting in all generated JSON files, which will reduce file size and is therefore recommended. Possible values: 0 (off), 1 (on) </summary>
    public static int PrettyJSONExportIndent = 1;

    // private void Start()
    // {
    //     // Set up Frame Limiter
    //     // Application.targetFrameRate = 60;
    //     // QualitySettings.vSyncCount = 1;
    // }
}
