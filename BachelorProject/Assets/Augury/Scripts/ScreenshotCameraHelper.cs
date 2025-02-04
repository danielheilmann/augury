using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotCameraHelper : MonoBehaviour
{
    public Camera cam => GetComponent<Camera>();
    public string camIdentifier = "";
    public float screenshotScalar = 1f;
    public bool isTopDown = false;
}
