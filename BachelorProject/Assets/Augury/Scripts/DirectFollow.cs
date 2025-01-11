using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private GameObject visualRepresentation;

    private void OnEnable()
    {
        visualRepresentation.SetActive(false);  //< Because this will be enabled during recording, when the visual representation should be hidden.
    }

    private void OnDisable()
    {
        visualRepresentation.SetActive(true);
    }

    private void Update()
    {
        if (target == null) return;

        transform.position = target.position;
        transform.rotation = target.rotation;
        transform.localScale = target.localScale;
    }
}
