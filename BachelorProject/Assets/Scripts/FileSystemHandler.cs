using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class FileSystemHandler : MonoBehaviour
{
    private const string FileEnding = ".txt";

    public static string SaveDictionaryToFile(string participantIdentifier, Dictionary<DateTime, Vector3> dictionary)
    {
        string filePath = Path.Combine(Application.dataPath, participantIdentifier + FileEnding);
        string fileText = ParsePointDictToText(dictionary);

        File.WriteAllText(filePath, fileText);

        // File.Create(filePath);
        // if (File.Exists(filePath))
        // {
        //     using (StreamWriter sw = File.CreateText(filePath))
        //     {
        //         foreach (KeyValuePair<DateTime, Vector3> entry in dictionary)
        //         {
        //             sw.WriteLine(entry.Key.ToString() + ", " + entry.Value.ToString());
        //         }
        //     }
        // }

        Debug.Log($"Saved file to {filePath}");
        return filePath;
    }

    private static string ParsePointDictToText(Dictionary<DateTime, Vector3> input)
    {
        string output = "";
        foreach (KeyValuePair<DateTime, Vector3> entry in input)
        {
            output += entry.Key.ToString() + ", " + entry.Value.ToString() + "\n";
        }
        return output;
    }
}
