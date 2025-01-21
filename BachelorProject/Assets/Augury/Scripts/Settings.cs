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
    /// <summary> Enabling this skips the "writing to file" step of the saving system. Mostly for debug purposes so that the application does not constantly create new files while running tests for unrelated features. </summary>
    public static bool skipSavingDataToFile { get; private set; } = false;
    /// <summary>Enabling this will cause the application to open the directory of the saved JSON files (in the Windows file explorer) whenever a GazePointData file is created. </summary>
    public static bool openExplorerOnSave { get; private set; } = false;
    /// <summary> This value is used to calculate the initial buffer size for the gazepoint dictionary (in <see cref="GazePointManager"/>) based on session duration and tick rate. </summary>
    public static float expectedSessionRuntimeInMinutes { get; private set; } = 1.0f;
    /// <summary> Turning this off disables pretty formatting in all generated JSON files, which will reduce file size and is therefore recommended. Possible values: 0 (off), 1 (on) </summary>
    public static bool prettyJSONExport { get; private set; } = false;
    /// <summary> Enabling this will result in GazePoints and Fixations being visualized in Record Mode as well. This is useful for debugging of GazePoint / Fixation creation. </summary>
    public static bool visualizeInRecordMode { get; private set; } = false;

    [Header("Global Setting Overrides")]
    [SerializeField] private DeviceMode _deviceMode = DeviceMode.XR;
    [Tooltip("This value is used to calculate the initial buffer size for the gazepoint dictionary (in GazePointManager) based on session duration and tick rate.")]
    [SerializeField] private float _expectedSessionRuntimeInMinutes = 1.0f;
    [Tooltip("Enabling this will result in GazePoints and Fixations being visualized in Record Mode as well. This is useful for debugging of GazePoint / Fixation creation.")]
    [SerializeField] private bool _visualizeInRecordMode = false;
    [Tooltip("Enabling this skips the \"writing to file\" step of the saving system. Mostly for debug purposes so that the application does not constantly create new files while running tests for unrelated features.")]
    [SerializeField] private bool _skipSavingDataToFile = false;
    [Tooltip("Enabling this will cause the application to open the directory of the saved JSON files (in the Windows file explorer) whenever a GazePointData file is created.")]
    [SerializeField] private bool _openExplorerOnSave = false;
    [Tooltip("Turning this off disables pretty formatting in all generated JSON files, which will reduce file size and is therefore recommended.")]
    [SerializeField] private bool _prettyJSONExport = false;

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
        //> Override the default values of the static variables above with the ones set in the inspector
        deviceMode = _deviceMode;
        skipSavingDataToFile = _skipSavingDataToFile;
        openExplorerOnSave = _openExplorerOnSave;
        expectedSessionRuntimeInMinutes = _expectedSessionRuntimeInMinutes;
        prettyJSONExport = _prettyJSONExport;
        visualizeInRecordMode = _visualizeInRecordMode;
    }
}
