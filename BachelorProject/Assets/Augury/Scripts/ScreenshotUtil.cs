using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class ScreenshotUtil : MonoBehaviour
{
    // public void TakeScreenshot()
    // {
    //     string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
    //     string filename = $"Screenshot_{timestamp}.png";

    //     string folderPath = Application.persistentDataPath + "/Screenshots/";

    //     if (!Directory.Exists(folderPath)) // if this path does not exist yet
    //         Directory.CreateDirectory(folderPath);  // it will get created

    //     string filePath = System.IO.Path.Combine(folderPath, filename);
    //     ScreenCapture.CaptureScreenshot(filePath);
    //     Debug.Log($"Screenshot saved to: {filePath}");
    // }

    [SerializeField] private string ParticipantID = "0";

    [Header("Screenshot Resolution")]
    [SerializeField] private int baseWidth = 3840;  // Base width
    [SerializeField] private int baseHeight = 2160; // Base height
    [SerializeField] private bool showSkybox = false;

    [Header("Camera Setup")]
    [SerializeField] private ScreenshotCameraHelper[] ScreenshotCameras;

    [Header("Town Toggle")]
    [SerializeField] private GameObject NormalTown;
    [SerializeField] private GameObject TopDownTown;


    public void TakeScreenshots()
    {
        // foreach (var entry in ScreenshotCameras)
        //     TakeScreenshot(entry.cam, entry.screenshotScalar, entry.camIdentifier, entry.isTopDown);

        Debug.Log($"Taking screenshots for participant {ParticipantID}. Please wait...");
        StartCoroutine(PaceScreenshots());
    }

    private IEnumerator PaceScreenshots()
    {
        foreach (var entry in ScreenshotCameras)
        {
            TakeScreenshot(entry.cam, entry.screenshotScalar, entry.camIdentifier, entry.isTopDown);
            yield return null;
        }

        Debug.Log($"Screenshots for participant {ParticipantID} have been saved.");
    }

    public void TakeScreenshot(Camera screenshotCamera, float screenshotScalar, string cameraIdentifier, bool useTopDownTown)
    {
        // Step -1: Calculate the screenshot resolution
        int screenshotWidth = Mathf.RoundToInt(baseWidth * screenshotScalar);
        int screenshotHeight = Mathf.RoundToInt(baseHeight * screenshotScalar);

        // Step 0: Set the town view
        NormalTown.SetActive(!useTopDownTown);
        TopDownTown.SetActive(useTopDownTown);

        if (useTopDownTown)
        {
            // Move and rotate all fixations for a better top-down view
            FixationVisualization[] fixationVisualizations = FindObjectsOfType<FixationVisualization>();
            foreach (var fixation in fixationVisualizations)
            {
                Transform fTrans = fixation.gameObject.transform;
                fTrans.position = new Vector3(fixation.gameObject.transform.position.x, 20f, fixation.gameObject.transform.position.z);
                // fixation.canvas.transform.rotation.SetLookRotation(Vector3.up, Vector3.back);
                // fixation.canvas.transform.up = Vector3.back;
                // fixation.canvas.transform.forward = Vector3.up;
                fixation.canvas.transform.rotation = Quaternion.Euler(90f, 0f, -180f);
            }
        }

        // Step 1: Enable the screenshot camera and set its background color to transparent
        screenshotCamera.gameObject.SetActive(true);
        if (showSkybox)
            screenshotCamera.clearFlags = CameraClearFlags.Skybox;
        else
        {
            screenshotCamera.clearFlags = CameraClearFlags.SolidColor;
            screenshotCamera.backgroundColor = new Color(0, 0, 0, 0); // Transparent background
        }

        // Step 2: Create a high-resolution RenderTexture with transparency
        RenderTexture rt = new RenderTexture(screenshotWidth, screenshotHeight, 24, RenderTextureFormat.ARGB32);
        screenshotCamera.targetTexture = rt;

        // Step 3: Render the screenshot camera view to the RenderTexture
        RenderTexture.active = rt;
        screenshotCamera.Render();

        // Step 4: Create a high-resolution Texture2D with transparency to store the rendered image
        Texture2D screenShot = new Texture2D(screenshotWidth, screenshotHeight, TextureFormat.RGBA32, false);
        screenShot.ReadPixels(new Rect(0, 0, screenshotWidth, screenshotHeight), 0, 0);
        screenShot.Apply();

        // Step 5: Save the Texture2D as a PNG file with transparency
        byte[] bytes = screenShot.EncodeToPNG();

        string filePath = ConstructFilePath(cameraIdentifier);
        File.WriteAllBytes(filePath, bytes);

        // Clean up
        screenshotCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        // Step 6: Disable the screenshot camera again
        screenshotCamera.gameObject.SetActive(false);

        // Debug.Log("Screenshot saved to: " + filePath);
    }

    private string ConstructFilePath(string fileNameAddition = "")
    {
        string folderPath = Application.persistentDataPath + "/Screenshots/";
        if (!Directory.Exists(folderPath)) // if this path does not exist yet
            Directory.CreateDirectory(folderPath);  // it will get created

        // string timestamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        string filename = $"P{ParticipantID}_{fileNameAddition}.png";

        string filePath = Path.Combine(folderPath, filename);

        if (File.Exists(filePath)) //< Prevent overwriting existing files
        {
            int counter = 1;
            string filenameWithoutExtension = Path.GetFileNameWithoutExtension(filename);
            string extension = Path.GetExtension(filename);
            while (File.Exists(filePath))
            {
                filename = $"{filenameWithoutExtension} ({counter}){extension}";
                filePath = Path.Combine(folderPath, filename);
                counter++;
            }
        }

        return filePath;
    }
}


