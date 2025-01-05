using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MaterialAlphaSine : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] private float amplitude = 0.5f;    // Controls wave height
    [SerializeField] private float frequency = 1f;      // Controls wave speed
    [SerializeField] private float phase = 0f;          // Controls wave offset horizontally
    [SerializeField] private float verticalOffset = 0.5f; // Controls wave offset vertically
    private float currentTime = 0f;

    private void Update()
    {
        float alpha = Mathf.Clamp(amplitude * Mathf.Sin(frequency * currentTime + phase) + verticalOffset, 0f, 1f);
        material.color = new Color(material.color.r, material.color.g, material.color.b, alpha);

        currentTime += Time.deltaTime;
    }

    private void OnApplicationQuit()
    {
        material.color = new Color(material.color.r, material.color.g, material.color.b, 0.5f);
    }
}
