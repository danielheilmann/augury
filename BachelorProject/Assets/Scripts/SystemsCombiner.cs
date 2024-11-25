using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: I do not like that this exists. There has to be a better way to link these two. Just rewriting OnHit to send the position instead is not an option though, as the collider will be needed for the static/dynamic check later on.
public class SystemsCombiner : MonoBehaviour
{
    private GazePointManager gazePointManager;

    private void Start()
    {
        gazePointManager = FindObjectOfType<GazePointManager>();
    }

    private void OnEnable()
    {
        RayProvider.OnHit.AddListener(CreatePointAtRayHitLocation);
    }

    private void OnDisable()
    {
        RayProvider.OnHit.RemoveListener(CreatePointAtRayHitLocation);
    }

    private void CreatePointAtRayHitLocation(RaycastHit hit)
    {
        gazePointManager.CreatePointAt(hit.point);
    }
}
