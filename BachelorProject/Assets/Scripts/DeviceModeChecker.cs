using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// It is not recommended to place this script on the target object itself (e.g. the object it disables through the event), as the script cannot run if the object starts inactive.
/// </summary>
[DefaultExecutionOrder(-1000)]
public class DeviceModeChecker : MonoBehaviour
{
    [SerializeField] private Settings.DeviceMode matchAgainstThisMode;
    [SerializeField] private UnityEvent OnModeMatch = new();
    [SerializeField] private UnityEvent OnModeMismatch = new();

    private void Awake()
    {
        if (matchAgainstThisMode == Settings.deviceMode)
            OnModeMatch.Invoke();
        else
            OnModeMismatch.Invoke();
    }
}
