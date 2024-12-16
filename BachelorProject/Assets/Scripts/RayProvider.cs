using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// To be placed on the tracked head, or eyes, to provide hits wherever the user looks. Relies on <see cref="Timer"/> to provide the signal for when a ray is cast. 
/// </summary>
public class RayProvider : MonoBehaviour
{
    public static UnityEvent<RaycastHit> OnHit = new();

    private void OnEnable() => Timer.OnTick.AddListener(ShootRay);

    private void OnDisable() => Timer.OnTick.RemoveListener(ShootRay);

    private void ShootRay()
    {
        if (Physics.Raycast(this.transform.position, this.transform.forward, out RaycastHit hit))
        {
            if (hit.collider != null)
            {
                OnHit.Invoke(hit);

            }
        }
    }

    /// <summary>
    /// Callback to draw gizmos only if the object is selected.
    /// </summary>
    // private void OnDrawGizmosSelected()
    // {
    //     Debug.DrawRay(this.transform.position, this.transform.forward * 10, Color.red, duration: 2.0f, depthTest: true);
    // }
}