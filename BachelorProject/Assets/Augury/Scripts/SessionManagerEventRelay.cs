using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//> Allows to relay SessionManager events to GameObjects in the scene without having to create separate scripts for each individual reaction a GameObject may have to these events.
//> I use this implementation instead of just making the SessionManager events non-static in order to keep the inspector of the SessionManager clean. Furthermore, making the events in the actual SessionManager non-static causes Errors when other scripts try to subscribe to them, because the SessionManager uses a custom DefaultScriptExecutionOrder.
public class SessionManagerEventRelay : MonoBehaviour
{
    public UnityEvent OnRecordStart = new();
    public UnityEvent OnRecordStop = new();
    public UnityEvent OnReplayStart = new();
    public UnityEvent OnReplayStop = new();

    private void OnEnable()
    {
        SessionManager.OnRecordStart.AddListener(OnRecordStart.Invoke);
        SessionManager.OnRecordStop.AddListener(OnRecordStop.Invoke);
        SessionManager.OnReplayStart.AddListener(OnReplayStart.Invoke);
        SessionManager.OnReplayStop.AddListener(OnReplayStop.Invoke);
    }

    private void OnDisable()
    {
        SessionManager.OnRecordStart.RemoveListener(OnRecordStart.Invoke);
        SessionManager.OnRecordStop.RemoveListener(OnRecordStop.Invoke);
        SessionManager.OnReplayStart.RemoveListener(OnReplayStart.Invoke);
        SessionManager.OnReplayStop.RemoveListener(OnReplayStop.Invoke);
    }
}
