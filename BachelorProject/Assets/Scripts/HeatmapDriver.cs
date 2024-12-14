using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class HeatmapDriver : MonoBehaviour
{
    private Material material;
    private MeshRenderer meshRenderer;

    private float[] points;
    private int hitCount;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        material = meshRenderer.material;

        // if (material.shader.name != "Heatmap")
        //     Debug.LogWarning($"You are using the HeatmapDriver script on {this.gameObject.name}, but it this object not using the Heatmap Shader. Either remove the script or assign a material of the correct shader to this object.", this.gameObject);

        points = new float[32 * 3]; //< For 32 points with 3 "attributes" (x, y & intensity) each
    }

    public void OnHit(RaycastHit hit)
    {
        AddHitPoint(hit.textureCoord.x, hit.textureCoord.y);
    }

    private void AddHitPoint(float x, float y)
    {
        points[hitCount * 3] = x * 4 - 2;     //< To account for the uv- offset in the shader
        points[hitCount * 3 + 1] = y * 4 - 2; //< To account for the uv- offset in the shader
        points[hitCount * 3 + 2] = 1f;    //< Hardcoded intensity value //TODO: Could be turned into tweakable variable

        hitCount++;
        hitCount %= 32; //< Limit hitCount to values between 0 and 31

        material.SetFloatArray("_Hits", points);
        material.SetInt("_HitCount", hitCount);
    }
}
