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

    public static string SaveDictionaryToFile(string participantIdentifier, Dictionary<DateTime, Vector3> dictionary)
    {
        string dataDirectory = Directory.CreateDirectory($"{new DirectoryInfo(Application.dataPath).Parent.ToString()}/RecordedData").Name;
        string filePath = $"{dataDirectory}/{participantIdentifier + FileEnding}";
        string fileText = ParseDictionaryToJSONString(dictionary);

        File.WriteAllText(filePath, fileText);
        Debug.Log($"Saved data to file: {filePath}");

        return filePath;
    }

    private static string ParseDictionaryToJSONString(Dictionary<DateTime, Vector3> dictionary)
    {
        JSONObject exportData = new JSONObject();

        exportData.Add("version", Application.version);

        foreach (var gazePoint in dictionary)
        {
            JSONArray position = new JSONArray();
            position.Add(gazePoint.Value.x);
            position.Add(gazePoint.Value.y);
            position.Add(gazePoint.Value.z);

            string timeStamp = gazePoint.Key.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            exportData.Add(timeStamp, position);
        }

        return exportData.ToString(1);  //TODO: Removing the argument here disables pretty formatting, which will reduce file size and is therefore recommended.
    }
}