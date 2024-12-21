using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField] private new Renderer renderer;

    void Start()
    {
        mainCamera = Camera.main;
        // renderer = GetComponentInChildren<Renderer>(); //< Opted for manual assignment instead.
    }

    void LateUpdate()
    {
        if (renderer.isVisible) //< Also returns true when the renderer is visible in the editor
        {
            // Debug.Log($"{this.gameObject.name} is visible.");
            transform.rotation = mainCamera.transform.rotation;
        }
            // transform.LookAt(mainCamera.transform);
    }
}
