using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using SimpleJSON;
using System.Globalization;

public class FileSystemHandler : MonoBehaviour
{
    private const string FileEnding = ".txt";

    public static void Save(List<GazePoint> gazePoints)
    {
        if (gazePoints.Count == 0)  //< There is no reason to save a file without point entries
            return;

        string filepath = SaveGazePointsToFile(fileTitle: $"Session {SessionManager.sessionStartTime.ToString("yyyy-MM-dd HH-mm-ss")}", gazePoints);
        if (Settings.OpenExplorerOnSave)
            FileSystemHandler.OpenFileExplorerAt(filepath);
    }

    private static string SaveGazePointsToFile(string fileTitle, List<GazePoint> gazePoints)
    {
        string dataDirectory = Directory.CreateDirectory($"{new DirectoryInfo(Application.dataPath).Parent.ToString()}/RecordedSessionData").Name;
        string filePath = $"{dataDirectory}/{fileTitle}{FileEnding}";
        string fileContent = ParseDictionaryToJSONString(gazePoints);

        File.WriteAllText(filePath, fileContent);
        Debug.Log($"Saved data to file: {filePath}");

        return filePath;
    }

    private static string ParseDictionaryToJSONString(List<GazePoint> gazePoints)
    {
        JSONObject exportData = new JSONObject();

        exportData.Add("version", Application.version);

        foreach (var gazePoint in gazePoints)
        {
            JSONArray position = new JSONArray();
            position.Add(gazePoint.position.x);
            position.Add(gazePoint.position.y);
            position.Add(gazePoint.position.z);

            string timeStamp = gazePoint.timeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            exportData.Add(timeStamp, position);
        }

        return exportData.ToString(1);  //TODO: Removing the argument here disables pretty formatting, which will reduce file size and is therefore recommended.
    }

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