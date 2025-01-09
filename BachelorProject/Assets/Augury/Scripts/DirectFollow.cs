using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private GameObject visualRepresentation;

    private void OnEnable()
    {
        SessionManager.OnReplayStart.AddListener(InReplayMode);
        SessionManager.OnRecordStart.AddListener(InRecordMode);
    }

    private void OnDisable()
    {
        SessionManager.OnReplayStart.RemoveListener(InReplayMode);
        SessionManager.OnRecordStart.RemoveListener(InRecordMode);
    }

    private void InReplayMode()
    {
        visualRepresentation.SetActive(true);
        // this.enabled = false; //< Because this object will be driven by the DynamicObject-Updates of the ReplayTimeline instead.
    }

    private void InRecordMode()
    {
        visualRepresentation.SetActive(false);
        // this.enabled = true;
    }

    private void Update()   //< This could be made more performant by only turning on the object during Record Mode. That way, the SessionMode check here would not be necessary.
    {
        if (target == null) return;

        if (SessionManager.currentMode == SessionManager.DataMode.Record)
        {
            transform.position = target.position;
            transform.rotation = target.rotation;
            transform.localScale = target.localScale;
        }
    }
}
