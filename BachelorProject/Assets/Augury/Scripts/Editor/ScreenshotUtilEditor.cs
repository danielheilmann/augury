using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ScreenshotUtil))]
public class ScreenshotUtilEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var handler = target as ScreenshotUtil;
        if (handler != null)
        {
            if (GUILayout.Button("Take Screenshots"))
                handler.TakeScreenshots();
        }

        base.OnInspectorGUI();
    }
}