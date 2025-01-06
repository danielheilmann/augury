using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Teleports the object that collides with this object to the teleportDestination. Intended to catch any object that has fallen off the map (e.g. by clipping through the floor).
/// </summary>
[RequireComponent(typeof(Collider))]
public class TeleportOnCollision : MonoBehaviour
{
    [SerializeField] private Transform teleportDestination;
    private new Collider collider;

    private void Awake()
    {
        collider = GetComponent<Collider>();
        collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Rigidbody otherRB))
        {
            otherRB.transform.position = teleportDestination.position;
        }
        else if (other.transform.parent != null && other.transform.parent.TryGetComponent(out Rigidbody otherParentRB))
        {
            otherParentRB.transform.position = teleportDestination.position;
        }
    }
}
