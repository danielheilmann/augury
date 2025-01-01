using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(-1)]
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
