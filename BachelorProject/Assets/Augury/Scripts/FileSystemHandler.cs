using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using SimpleJSON;
using System.Globalization;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Linq;

//TODO Make sure to implement failsaves for corrupted or scrambled files
public static class FileSystemHandler
{
    //> File-Related Constants
    private const string DataDirectoryName = "RecordedSessionData";
    private const string FileExtension = ".json";

    //> Session-Related Constants
    private const string KEY_SESSION_ID = "session";
    private const string KEY_SCENE_ID = "scene";
    private const string KEY_APP_VERSION = "appVersion";

    //> DataType-Related Constants
    private const string KEY_DATATYPE = "dataType";
    private const string KEY_DATAIDENTIFIER_DYNOBJ = "DynamicObjectData";
    private const string KEY_DATAIDENTIFIER_GAZEPOINT = "GazePointData";

    //> Content-Related Constants
    public const string KEY_TIMESTAMP = "timestamp";
    public const string TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff";
    public const string KEY_DYNOBJ_ID = "dynObjID";
    public const string KEY_SURFACENORMAL = "surfaceNormal";
    public const string KEY_POSITION = "position";
    public const string KEY_ROTATION = "rotation";
    public const string KEY_SCALE = "scale";

    //> JSON-Array Identification Constants
    public const string KEY_GAZEPOINTS = "gazepoints";
    public const string KEY_POSITIONHISTORY = "positionHistory";
    public const string KEY_ROTATIONHISTORY = "rotationHistory";
    public const string KEY_SCALEHISTORY = "scaleHistory";

    //> Parameters to simplify access to certain values
    private static char directorySeparator = Path.DirectorySeparatorChar; //< static because it cannot be const as Path.DirectorySeparatorChar needs to be read.
    private static string currentSceneName => SceneManager.GetActiveScene().name;

    private static string dataDir { get { if (!Directory.Exists(DataDirectoryName)) Directory.CreateDirectory(DataDirectoryName); return DataDirectoryName; } }

    #region Writing Data
    /// <summary> Creates a file in the application's data path, in a subfolder as declared by the "FolderName" const in the <see cref="FileSystemHandler"/> class. </summary>   
    /// <param name="label"> The title of the created file, WITHOUT the file extension (e.g. ".txt") </param>
    /// <param name="fileContent"> The content of the file in the form of a single string. It may also be JSON, as it is nothing but text. </param>
    /// <returns> The full file path of the file that was created. If saving to file was skipped through global setting flag, return will be empty. </returns>
    private static string CreateFile(string label, string fileContent)
    {
        if (Settings.skipSavingDataToFile)
        {
            Debug.Log($"<color=#cc80ff>Skipped saving data to file. </color>");
            return "";
        }

        string sessionDir = $"{dataDir}{directorySeparator}{RecordManager.sessionIdentifier}";
        if (!Directory.Exists(sessionDir))
            Directory.CreateDirectory(sessionDir);

        string fileName = $"{RecordManager.sessionIdentifier}_{currentSceneName}_{label}{FileExtension}";
        string filePath = $"{sessionDir}{directorySeparator}{fileName}";
        File.WriteAllText(filePath, fileContent);
        Debug.Log($"<color=#cc80ff>Saved data to file: </color>{filePath}\n{fileContent}");

        return filePath;
    }

    #region Saving Gaze Points
    public static void SaveGazePoints(List<GazePoint> gazePoints)
    {
        if (gazePoints.Count == 0)  //< There is no reason to save a file from a session without any point entries, as that was most likely a start-stop situation.
            return;

        string filepath = CreateFile(label: $"GazePoints", fileContent: ParseListToJSONString(gazePoints));

        if (Settings.openExplorerOnSave)
            OpenFileExplorerAt(filepath);
    }

    private static string ParseListToJSONString(List<GazePoint> gazePoints)
    {
        JSONObject output = new JSONObject();

        output.Add(KEY_APP_VERSION, Application.version);
        output.Add(KEY_SESSION_ID, RecordManager.sessionIdentifier);
        output.Add(KEY_SCENE_ID, currentSceneName);
        output.Add(KEY_DATATYPE, KEY_DATAIDENTIFIER_GAZEPOINT);

        JSONArray points = new JSONArray();
        foreach (GazePoint gp in gazePoints)
        {
            JSONObject point = new JSONObject();

            point.Add(KEY_TIMESTAMP, gp.timeStamp.ToString(TimestampFormat, CultureInfo.InvariantCulture));
            point.Add(KEY_DYNOBJ_ID, gp.dynObjID); //< Needs to write a dynObjID entry for every JSON-Object, otherwise it does not write any dynObjID entry.

            JSONArray surfaceNormal = Vector3ToJSONArray(gp.surfaceNormal);
            point.Add(KEY_SURFACENORMAL, surfaceNormal);

            JSONArray position = Vector3ToJSONArray(gp.globalPosition);
            point.Add(KEY_POSITION, position);

            points.Add(point);
        }
        output.Add(KEY_GAZEPOINTS, points);
        if (Settings.prettyJSONExport)
            return output.ToString(1);
        else
            return output.ToString();
    }
    #endregion

    #region Saving DynamicObjects
    public static void SaveDynamicObject(DynamicObject dynamicObject)
    {
        CreateFile(label: $"DynObj_{dynamicObject.name.RemoveInvalidFileNameChars()}", fileContent: ParseDynamicObjectToJSONString(dynamicObject));
    }

    private static string ParseDynamicObjectToJSONString(DynamicObject dynamicObject) //?< This could be moved into the DynamicObject class to serve as a simple ".ToJSON()" method
    {
        JSONObject output = new JSONObject();

        output.Add(KEY_APP_VERSION, Application.version);
        output.Add(KEY_SESSION_ID, RecordManager.sessionIdentifier);
        output.Add(KEY_SCENE_ID, currentSceneName);
        output.Add(KEY_DATATYPE, KEY_DATAIDENTIFIER_DYNOBJ);
        output.Add(KEY_DYNOBJ_ID, dynamicObject.id);

        #region Position History
        JSONArray positionHist = new JSONArray();
        Dictionary<DateTime, Vector3> positionDictionary = dynamicObject.positionHistory;
        foreach (var entry in positionDictionary) //?< Are they even sorted in this case?
        {
            JSONObject timestampedPosition = new JSONObject();

            timestampedPosition.Add(KEY_TIMESTAMP, entry.Key.ToString(TimestampFormat, CultureInfo.InvariantCulture));

            JSONArray position = Vector3ToJSONArray(entry.Value);
            timestampedPosition.Add(KEY_POSITION, position);

            positionHist.Add(timestampedPosition);
        }
        output.Add(KEY_POSITIONHISTORY, positionHist);
        #endregion

        #region Rotation History
        JSONArray rotationHist = new JSONArray();
        Dictionary<DateTime, Quaternion> rotationDictionary = dynamicObject.rotationHistory;
        foreach (var entry in rotationDictionary) //< Are they even sorted in this case?
        {
            JSONObject timestampedRotation = new JSONObject();

            timestampedRotation.Add(KEY_TIMESTAMP, entry.Key.ToString(TimestampFormat, CultureInfo.InvariantCulture));

            JSONArray rotation = new JSONArray();
            rotation.Add(entry.Value.x);
            rotation.Add(entry.Value.y);
            rotation.Add(entry.Value.z);
            rotation.Add(entry.Value.w);
            timestampedRotation.Add(KEY_ROTATION, rotation);

            rotationHist.Add(timestampedRotation);
        }
        output.Add(KEY_ROTATIONHISTORY, rotationHist);
        #endregion

        #region Scale History
        JSONArray scaleHist = new JSONArray();
        Dictionary<DateTime, Vector3> scaleDictionary = dynamicObject.scaleHistory;
        foreach (var entry in scaleDictionary) //< Are they even sorted in this case?
        {
            JSONObject timestampedScale = new JSONObject();

            timestampedScale.Add(KEY_TIMESTAMP, entry.Key.ToString(TimestampFormat, CultureInfo.InvariantCulture));

            JSONArray scale = Vector3ToJSONArray(entry.Value);
            timestampedScale.Add(KEY_SCALE, scale);

            scaleHist.Add(timestampedScale);
        }
        output.Add(KEY_SCALEHISTORY, scaleHist);
        #endregion

        if (Settings.prettyJSONExport)
            return output.ToString(1);
        else
            return output.ToString();
    }

    /// <summary></summary>
    /// <param name="vector"></param>
    /// <returns>A JSONArray containing the x,y and z entries of a Vector3</returns>
    private static JSONArray Vector3ToJSONArray(Vector3 vector)
    {
        JSONArray array = new JSONArray();

        array.Add(vector.x);
        array.Add(vector.y);
        array.Add(vector.z);

        return array;
    }

    /// <summary></summary>
    /// <param name="array">A JSONArray containing x,y,z values for a Vector3</param>
    /// <returns>A Vector3 constructed from the first three values in the JSONArray</returns>
    public static Vector3 JSONArrayToVector3(JSONArray array)   //< Ended up not being used as this conversion can be done implicitly by the JSONArray class.
    {
        return new Vector3(
            array[0].AsFloat,
            array[1].AsFloat,
            array[2].AsFloat
        );
    }

    private static string RemoveInvalidFileNameChars(this string input)
    {
        string output = input;
        foreach (char c in Path.GetInvalidFileNameChars())
            output = output.Replace(c, '_');

        return output;
    }
    #endregion

    #region Automatically open File Explorer at file location
    private static void OpenFileExplorerAt(string filePath)
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
    #endregion

    #region Reading Data
    public static List<SessionFileReference> FetchAllSessions()
    {
        if (!Directory.Exists(DataDirectoryName))
        {
            Debug.LogWarning($"The data directory is empty, there are no sessions to fetch.");
            return null;
        }
        
        Dictionary<string, SessionFileReference> sessionCollection = new();

        string[] filePaths = Directory.GetFiles(DataDirectoryName, "*" + FileExtension, searchOption: SearchOption.AllDirectories);
        foreach (string filePath in filePaths)
        {
            string fileContentString = File.ReadAllText(filePath);
            JSONNode fileContentNode = JSONNode.Parse(fileContentString);

            string appVersion = fileContentNode[KEY_APP_VERSION].Value;
            string sessionIdentifier = fileContentNode[KEY_SESSION_ID].Value;
            string sceneName = fileContentNode[KEY_SCENE_ID].Value;
            bool isDynamicObjectData = fileContentNode[KEY_DATATYPE].Value == KEY_DATAIDENTIFIER_DYNOBJ;

            if (!sessionCollection.ContainsKey(sessionIdentifier)) //< Create a new session only if the collection does not contain one with that identifier already.
            {
                SessionFileReference newSessionReference = new(sessionIdentifier, appVersion, sceneName, filePath);
                sessionCollection.Add(sessionIdentifier, newSessionReference);
            }
            //> Store entries in the respective session. This allows for multiple entries per session.
            sessionCollection[sessionIdentifier].StoreJSONEntry((JSONObject)fileContentNode, isDynamicObjectData);
        }
        return sessionCollection.Values.ToList();
    }
    #endregion
}