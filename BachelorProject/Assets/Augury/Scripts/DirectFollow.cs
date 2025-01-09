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
        // this.enabled = false;
    }

    private void InRecordMode()
    {
        visualRepresentation.SetActive(false);
        // this.enabled = true;
    }

    private void Update()
    {
        if (target == null)
            return;

        if (SessionManager.currentMode == SessionManager.DataMode.Record)
        {
            transform.position = target.position;
            transform.rotation = target.rotation;
            transform.localScale = target.localScale;
        }
    }
}
