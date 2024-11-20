using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemsCombiner : MonoBehaviour
{
    private void Awake()
    {
        RayProvider.OnHit.AddListener(CreatePointAtRayHitLocation);
    }

    private void Start()
    {
        // Set up Frame Limiter
        // float vSyncFactor = (float)Screen.currentResolution.refreshRateRatio.value / 60f;
        // int targetFPS = Mathf.Clamp(Mathf.RoundToInt(vSyncFactor), 1, 4);
        // Application.targetFrameRate = targetFPS * 60;
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 1;
    }

    private void CreatePointAtRayHitLocation(RaycastHit hit)
    {
        SpatialPointManager.Instance.CreatePointAt(hit.point);
    }
}
