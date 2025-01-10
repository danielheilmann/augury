using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SimpleJSON;
using System.IO;

public class SessionFileReference
{
    public string appVersion { get; private set; }
    public string sessionIdentifier { get; private set; }
    public string sceneName { get; private set; }
    public string path { get; private set; }
    public DateTime sessionStartTime => DateTime.ParseExact(sessionIdentifier.Trim("Session ".ToCharArray()), RecordManager.FileNameSafeTimeFormat, null); //< Is used very rarely, so we can afford to calculate it on the fly.

    public JSONObject gazePointsJSON { get; private set; }
    public List<JSONObject> dynamicObjectJSONs { get; private set; }

    public SessionFileReference(string sessionIdentifier, string appVersion, string sceneName, string folderTitle)
    {
        this.sessionIdentifier = sessionIdentifier;
        this.appVersion = appVersion;
        this.sceneName = sceneName;
        this.path = folderTitle;

        gazePointsJSON = null;
        dynamicObjectJSONs = new List<JSONObject>();
    }

    public void StoreJSONEntry(JSONObject entry, bool isDynamicObjectData)
    {
        if (isDynamicObjectData)
            dynamicObjectJSONs.Add(entry);
        else
        {
            if (gazePointsJSON != null)
                Debug.LogWarning($"{sessionIdentifier} already contains an entry for GazePoints. Overwriting previous entry. This should not happen.");

            gazePointsJSON = entry;
        }
    }

    public override string ToString()
    {
        // return $"{sessionIdentifier} | {sceneName} | v{appVersion}";
        // Debug.Log($"Filepath: {path}");
        return $"{path}";
    }
}
