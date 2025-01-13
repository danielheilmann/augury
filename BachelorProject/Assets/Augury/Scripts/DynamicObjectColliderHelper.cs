using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//> This script is to be attached to the dedicated collider-child of a dynamic object, so that if the collider-child is hit by a raycast, the GazePointManager does not have to search for the DynamicObject component in the parent object.
[RequireComponent(typeof(Collider))]
public class DynamicObjectColliderHelper : MonoBehaviour
{
    [SerializeField] public DynamicObject dynamicObject;

    private void Start()
    {
        if (dynamicObject == null)  //< If it was not assigned manually in the inspector.
            dynamicObject = gameObject.transform.parent?.GetComponent<DynamicObject>() ?? null;

        if (dynamicObject == null)
            Debug.LogError($"DynamicObjectColliderHelper on {this.transform.parent?.gameObject.name ?? this.gameObject.name}: No DynamicObject component found.", this.gameObject);
    }
}
