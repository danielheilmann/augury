using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SimpleJSON;

public class SessionFileReference
{
    public string appVersion { get; private set; }
    public string sessionIdentifier { get; private set; }
    public string sceneName { get; private set; }
    public string path { get; private set; }
    
    private JSONNode gazePoints = null;
    private List<JSONNode> dynamicObjects = new List<JSONNode>();

    public SessionFileReference(string sessionIdentifier, string appVersion, string sceneName, string path)
    {
        this.sessionIdentifier = sessionIdentifier;
        this.appVersion = appVersion;
        this.sceneName = sceneName;
    }

    public void AddEntry(JSONNode entry, bool isDynamicObjectData)
    {
        if (isDynamicObjectData)
            dynamicObjects.Add(entry);
        else
        {
            if (gazePoints != null)
                Debug.LogError($"{sessionIdentifier} already contains an entry for GazePoints. Overwriting previous entry.");

            gazePoints = entry;
        }
    }

    public override string ToString()
    {
        return $"{sessionIdentifier} | {appVersion} | {sceneName}";
    }
}
