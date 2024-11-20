using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Should probably be placed on the camera
public class RayProvider : MonoBehaviour
{
    public static UnityEvent<RaycastHit> OnHit = new();

    private void OnEnable()
    {
        Timer.OnTick.AddListener(ShootRay);
    }

    private void OnDisable()
    {
        Timer.OnTick.RemoveListener(ShootRay);
    }

    private void ShootRay()
    {
        if (Physics.Raycast(this.transform.position, this.transform.forward, out RaycastHit hit))
        {
            if (hit.collider != null)
            {
                OnHit.Invoke(hit);
                // SpatialPointManager.CreatePointAt(hit.point);
            }
        }
    }
}