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

    private JSONNode gazePointsJSON = null;
    private List<JSONNode> dynamicObjectJSONs = new List<JSONNode>();

    public SessionFileReference(string sessionIdentifier, string appVersion, string sceneName, string path)
    {
        this.sessionIdentifier = sessionIdentifier;
        this.appVersion = appVersion;
        this.sceneName = sceneName;
    }

    public void AddEntry(JSONNode entry, bool isDynamicObjectData)
    {
        if (isDynamicObjectData)
            dynamicObjectJSONs.Add(entry);
        else
        {
            if (gazePointsJSON != null)
                Debug.LogError($"{sessionIdentifier} already contains an entry for GazePoints. Overwriting previous entry.");

            gazePointsJSON = entry;
        }
    }

    public List<GazePoint> GetGazePoints()
    {
        if (gazePointsJSON == null)
        {
            Debug.LogError($"{sessionIdentifier} does not contain any GazePoints data.");
            return null;
        }

        var output = new List<GazePoint>();

        JSONArray JSONPoints = gazePointsJSON[FileSystemHandler.KEY_GAZEPOINTS].AsArray;
        foreach (var entry in JSONPoints)
        {
            JSONObject JSONPoint = entry.Value.AsObject;

            DateTime timestamp = JSONPoint[FileSystemHandler.KEY_TIMESTAMP].AsDateTime;
            string dynObjID = JSONPoint[FileSystemHandler.KEY_DYNOBJ_ID].Value;
            DynamicObject dynObjReference = string.IsNullOrEmpty(dynObjID) ? null : DynamicObjectManager.GetByID(dynObjID);
            Vector3 surfaceNormal = JSONPoint[FileSystemHandler.KEY_SURFACENORMAL].AsArray;
            // FileSystemHandler.JSONArrayToVector3(JSONPoint[FileSystemHandler.KEY_SURFACENORMAL].AsArray);    //< Alternative, I'm not sure if it's even necessary
            Vector3 position = JSONPoint[FileSystemHandler.KEY_POSITION].AsArray;

            if (dynObjReference != null)  //< Because we always store global position into the JSON file, we have to convert it back to local position here if it is connected to a dynamic object.
                position = position - dynObjReference.transform.position;

            output.Add(new GazePoint(timestamp, position, surfaceNormal, dynObjReference));
        }

        return output;
    }

    public override string ToString()
    {
        return $"{sessionIdentifier} | {appVersion} | {sceneName}";
    }
}
