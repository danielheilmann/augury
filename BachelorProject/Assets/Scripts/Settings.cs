using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// May be attached to a GameObject to provide In-Editor access to the settings. Can also be used as an entirely static script with the default values (if no overrides are needed). 
/// </summary>
[DefaultExecutionOrder(-2)]
public class Settings : MonoBehaviour
{
    public enum DeviceMode { Desktop, XR }

    public static DeviceMode deviceMode { get; private set; } = DeviceMode.XR;
    public static bool saveDataOnQuit { get; private set; } = true;
    public static bool openExplorerOnSave { get; private set; } = false;
    /// <summary> This value is used to calculate the initial buffer size for the gazepoint dictionary (in <see cref="GazePointManager"/>) based on session duration and tick rate. </summary>
    public static float expectedSessionRuntimeInMinutes { get; private set; } = 1f;
    /// <summary> Turning this off disables pretty formatting in all generated JSON files, which will reduce file size and is therefore recommended. Possible values: 0 (off), 1 (on) </summary>
    public static int prettyJSONExportIndent { get; private set; } = 0;

#if UNITY_EDITOR    //! I am not sure, but this may completely remove any overrides from the build, which is not the intention.
    [Header("Global Setting Overrides")]
    [SerializeField] private DeviceMode _deviceMode = DeviceMode.XR;
    [SerializeField] private bool _saveDataOnQuit = true;
    [SerializeField] private bool _openExplorerOnSave = false;
    [Tooltip("This value is used to calculate the initial buffer size for the gazepoint dictionary (in GazePointManager) based on session duration and tick rate.")]
    [SerializeField] private float _expectedSessionRuntimeInMinutes = 1f;
    [Tooltip("Turning this off disables pretty formatting in all generated JSON files, which will reduce file size and is therefore recommended.")]
    [SerializeField] private bool _prettyJSONExportIndent = false;

    private void Awake()
    {
        ApplyOverrides();

        // Set up Frame Limiter
        // Application.targetFrameRate = 60;
        // QualitySettings.vSyncCount = 1;
    }

    private void OnValidate()
    {
        ApplyOverrides();
    }

    /// <summary>
    /// Is executed on Start() and OnValidate(). Overrides default values set in <see cref="Settings"/>.
    /// </summary>
    private void ApplyOverrides()
    {
        deviceMode = _deviceMode;
        saveDataOnQuit = _saveDataOnQuit;
        openExplorerOnSave = _openExplorerOnSave;
        expectedSessionRuntimeInMinutes = _expectedSessionRuntimeInMinutes;
        prettyJSONExportIndent = _prettyJSONExportIndent ? 1 : 0;
    }
#endif
}
