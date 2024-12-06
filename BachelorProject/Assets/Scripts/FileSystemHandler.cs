using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using SimpleJSON;
using System.Globalization;
using UnityEngine.SceneManagement;

public static class FileSystemHandler
{
    private const string FileEnding = ".txt";

    #region Saving Gaze Points
    public static void SaveGazePoints(List<GazePoint> gazePoints)
    {
        if (gazePoints.Count == 0)  //< There is no reason to save a file without point entries
            return;

        string filepath = SaveGazePointsToFile(fileTitle: $"Session {SessionManager.sessionStartTime.ToString("yyyy-MM-dd HH-mm-ss")}", gazePoints);
        if (Settings.OpenExplorerOnSave)
            OpenFileExplorerAt(filepath);
    }

    private static string SaveGazePointsToFile(string fileTitle, List<GazePoint> gazePoints)
    {
        string dataDirectory = Directory.CreateDirectory($"{new DirectoryInfo(Application.dataPath).Parent.ToString()}/RecordedSessionData").Name;
        string filePath = $"{dataDirectory}/{fileTitle}{FileEnding}";
        string fileContent = ParseListToJSONString(gazePoints);

        File.WriteAllText(filePath, fileContent);
        Debug.Log($"Saved data to file: {filePath}");

        return filePath;
    }

    private static string ParseListToJSONString(List<GazePoint> gazePoints)
    {
        JSONObject exportData = new JSONObject();

        exportData.Add("appVersion", Application.version);

        foreach (var gazePoint in gazePoints)
        {
            JSONArray position = new JSONArray();
            position.Add(gazePoint.position.x);
            position.Add(gazePoint.position.y);
            position.Add(gazePoint.position.z);

            string timeStamp = gazePoint.timeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            exportData.Add(timeStamp, position);
        }

        return exportData.ToString(Settings.PrettyJSONExportIndent);
    }
    #endregion

    #region Saving DynamicObjects
    public static string SaveDynamicObjectToFile(string fileTitle, DynamicObject dynamicObject)
    {
        string dataDirectory = Directory.CreateDirectory($"{new DirectoryInfo(Application.dataPath).Parent.ToString()}/RecordedSessionData").Name;
        string filePath = $"{dataDirectory}/{fileTitle}{FileEnding}";
        string fileContent = ParseDynamicObjectToJSONString(dynamicObject); ;

        File.WriteAllText(filePath, fileContent);
        Debug.Log($"Saved data to file: {filePath}");

        return filePath;
    }

    private static string ParseDynamicObjectToJSONString(DynamicObject dynamicObject)   //TODO: This looks very bloated, maybe it should be put into something like a DynamicObject.ToJSON() method?
    {
        JSONObject exportData = new JSONObject();

        exportData.Add("appVersion", Application.version);
        exportData.Add("inScene", SceneManager.GetActiveScene().name);
        exportData.Add("objectId", dynamicObject.id);

        #region Position History
        JSONArray positionHist = new JSONArray();
        Dictionary<DateTime, Vector3> positionDictionary = dynamicObject.positionHistory;
        foreach (var entry in positionDictionary) //< Are they even sorted in this case?
        {
            string timestamp = entry.Key.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);

            JSONArray position = new JSONArray();
            position.Add(entry.Value.x);
            position.Add(entry.Value.y);
            position.Add(entry.Value.z);

            JSONObject timestampedPosition = new JSONObject();
            timestampedPosition.Add(timestamp, position);

            positionHist.Add(timestampedPosition);
        }
        exportData.Add("positionHistory", positionHist);    //TODO: Nesting JSONArrays like this deletes the original array's name field (in this case "timestamp").
        #endregion

        #region Rotation History
        JSONArray rotationHist = new JSONArray();
        Dictionary<DateTime, Quaternion> rotationDictionary = dynamicObject.rotationHistory;
        foreach (var entry in rotationDictionary) //< Are they even sorted in this case?
        {
            string timestamp = entry.Key.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);

            JSONArray rotation = new JSONArray();
            rotation.Add(entry.Value.x);
            rotation.Add(entry.Value.y);
            rotation.Add(entry.Value.z);
            rotation.Add(entry.Value.w);

            JSONObject timestampedPosition = new JSONObject();
            timestampedPosition.Add(timestamp, rotation);

            rotationHist.Add(timestampedPosition);
        }
        exportData.Add("rotationHistory", rotationHist);
        #endregion

        #region Scale History
        JSONArray scaleHist = new JSONArray();
        Dictionary<DateTime, Vector3> scaleDictionary = dynamicObject.scaleHistory;
        foreach (var entry in scaleDictionary) //< Are they even sorted in this case?
        {
            string timestamp = entry.Key.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);

            JSONArray scale = new JSONArray();
            scale.Add(entry.Value.x);
            scale.Add(entry.Value.y);
            scale.Add(entry.Value.z);

            JSONObject timestampedPosition = new JSONObject();
            timestampedPosition.Add(timestamp, scale);

            scaleHist.Add(timestampedPosition);
        }
        exportData.Add("scaleHistory", scaleHist);
        #endregion

        return exportData.ToString(Settings.PrettyJSONExportIndent);  //TODO: Removing the argument here disables pretty formatting, which will reduce file size and is therefore recommended.
    }
    #endregion


    #region Automatically open File Explorer at file location
    public static void OpenFileExplorerAt(string filePath)
    {
        System.Diagnostics.ProcessStartInfo process = new System.Diagnostics.ProcessStartInfo();
        process.FileName = "explorer.exe";
        process.Arguments = $"/select,\"{ConvertToWindowsPath(filePath)}\"";

        System.Diagnostics.Process.Start(process);

    }

    private static string ConvertToWindowsPath(string path)
    {
        char[] charList = path.ToCharArray();
        for (int i = 0; i < charList.Length; i++)
        {
            if (charList[i] == '/')
                charList[i] = '\\';
        }
        return new string(charList);
    }
    #endregion
}