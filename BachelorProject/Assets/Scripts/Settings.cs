using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static bool OpenExplorerOnSave = false;
    public static float expectedSessionRuntimeInMinutes = 1f;  //< This value is used to calculate the initial buffer size for the gazepoint dictionary based on session duration and tick rate.
    public static int PrettyJSONExportIndent = 1;   //< 0 = off; 1 = on; >1 = increased indentation  //* Turning it off disables pretty formatting, which will reduce file size and is therefore recommended.
    private void Start()
    {
        // Set up Frame Limiter
        // Application.targetFrameRate = 60;
        // QualitySettings.vSyncCount = 1;
    }
}
