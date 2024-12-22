using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using SimpleJSON;
using System.Globalization;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

//TODO Make sure to implement failsaves for corrupted or scrambled files
public static class FileSystemHandler
{
    public const string TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff";
    private const string FileExtension = ".json";
    private const string DataDirectoryName = "RecordedSessionData";
    private const string KEY_SESSION_ID = "session";
    private const string KEY_APP_VERSION = "appVersion";
    private const string KEY_SCENE_ID = "inScene";
    public const string KEY_TIMESTAMP = "timestamp";
    private const string KEY_POSITION = "position";
    private const string KEY_SURFACENORMAL = "surfaceNormal";
    private const string KEY_DYNOBJ_ID = "dynObjID";
    private const string KEY_GAZEPOINTS = "gazepoints";
    private static char dirSeperator = Path.DirectorySeparatorChar; //< static because it cannot be const as Path.DirectorySeparatorChar needs to be read.
    private static string dataDir { get { if (!Directory.Exists(DataDirectoryName)) Directory.CreateDirectory(DataDirectoryName); return DataDirectoryName; } }

    #region Writing Data
    /// <summary> Creates a file in the application's data path, in a subfolder as declared by the "FolderName" const in the <see cref="FileSystemHandler"/> class. </summary>   
    /// <param name="fileTitle"> The title of the created file, WITHOUT the file extension (e.g. ".txt") </param>
    /// <param name="fileContent"> The content of the file in the form of a single string. It may also be JSON, as it is nothing but text. </param>
    /// <returns> The full file path of the file that was created. </returns>
    private static string CreateFile(string fileTitle, string fileContent)
    {
        string filePath = $"{dataDir}{dirSeperator}{fileTitle}{FileExtension}";

        if (!Settings.TestModeEnabled)  //< So that the application does not constantly create new files while running tests for unrelated features.
        {
            File.WriteAllText(filePath, fileContent);
            Debug.Log($"<color=#cc80ff>Saved data to file: </color>{filePath}\n{fileContent}");
        }

        return filePath;
    }

    #region Saving Gaze Points
    public static void SaveGazePoints(List<GazePoint> gazePoints)
    {
        if (gazePoints.Count == 0)  //< There is no reason to save a file from a session without any point entries, as that was most likely a start-stop situation.
            return;

        string filepath = CreateFile(fileTitle: $"{SessionManager.sessionIdentifier} - GazePoints", fileContent: ParseListToJSONString(gazePoints));

        if (Settings.OpenExplorerOnSave)
            OpenFileExplorerAt(filepath);
    }

    private static string ParseListToJSONString(List<GazePoint> gazePoints)
    {
        JSONObject output = new JSONObject();

        output.Add(KEY_SESSION_ID, SessionManager.sessionIdentifier);
        output.Add(KEY_APP_VERSION, Application.version);
        output.Add(KEY_SCENE_ID, SceneManager.GetActiveScene().name);

        JSONArray points = new JSONArray();
        foreach (GazePoint gp in gazePoints)
        {
            JSONObject point = new JSONObject();

            point.Add(KEY_TIMESTAMP, gp.timeStamp.ToString(TimestampFormat, CultureInfo.InvariantCulture));
            point.Add(KEY_DYNOBJ_ID, gp.dynObjID); //< Needs to write a dynObjID entry for every JSON-Object, otherwise it does not write any dynObjID entry.

            JSONArray surfaceNormal = new JSONArray();
            surfaceNormal.Add(gp.surfaceNormal.x);
            surfaceNormal.Add(gp.surfaceNormal.y);
            surfaceNormal.Add(gp.surfaceNormal.z);
            point.Add(KEY_SURFACENORMAL, surfaceNormal);

            JSONArray position = new JSONArray();
            position.Add(gp.position.x);
            position.Add(gp.position.y);
            position.Add(gp.position.z);
            point.Add(KEY_POSITION, position);

            points.Add(point);
        }
        output.Add(KEY_GAZEPOINTS, points);

        return output.ToString(Settings.PrettyJSONExportIndent);
    }
    #endregion

    #region Saving DynamicObjects
    public static void SaveDynamicObject(DynamicObject dynamicObject)
    {
        CreateFile(fileTitle: $"{SessionManager.sessionIdentifier} - DynObj_{dynamicObject.id}", fileContent: ParseDynamicObjectToJSONString(dynamicObject));
    }

    private static string ParseDynamicObjectToJSONString(DynamicObject dynamicObject) //?< This could be moved into the DynamicObject class to serve as a simple ".ToJSON()" method
    {
        JSONObject output = new JSONObject();

        output.Add(KEY_SESSION_ID, SessionManager.sessionIdentifier);
        output.Add(KEY_APP_VERSION, Application.version);
        output.Add(KEY_SCENE_ID, SceneManager.GetActiveScene().name);
        output.Add(KEY_DYNOBJ_ID, dynamicObject.id);

        #region Position History
        JSONArray positionHist = new JSONArray();
        Dictionary<DateTime, Vector3> positionDictionary = dynamicObject.positionHistory;
        foreach (var entry in positionDictionary) //?< Are they even sorted in this case?
        {
            string timestamp = entry.Key.ToString(TimestampFormat, CultureInfo.InvariantCulture);

            JSONArray position = new JSONArray();
            position.Add(entry.Value.x);
            position.Add(entry.Value.y);
            position.Add(entry.Value.z);

            JSONObject timestampedPosition = new JSONObject();
            timestampedPosition.Add(timestamp, position);

            positionHist.Add(timestampedPosition);
        }
        output.Add("positionHistory", positionHist);
        #endregion

        #region Rotation History
        JSONArray rotationHist = new JSONArray();
        Dictionary<DateTime, Quaternion> rotationDictionary = dynamicObject.rotationHistory;
        foreach (var entry in rotationDictionary) //< Are they even sorted in this case?
        {
            string timestamp = entry.Key.ToString(TimestampFormat, CultureInfo.InvariantCulture);

            JSONArray rotation = new JSONArray();
            rotation.Add(entry.Value.x);
            rotation.Add(entry.Value.y);
            rotation.Add(entry.Value.z);
            rotation.Add(entry.Value.w);

            JSONObject timestampedPosition = new JSONObject();
            timestampedPosition.Add(timestamp, rotation);

            rotationHist.Add(timestampedPosition);
        }
        output.Add("rotationHistory", rotationHist);
        #endregion

        #region Scale History
        JSONArray scaleHist = new JSONArray();
        Dictionary<DateTime, Vector3> scaleDictionary = dynamicObject.scaleHistory;
        foreach (var entry in scaleDictionary) //< Are they even sorted in this case?
        {
            string timestamp = entry.Key.ToString(TimestampFormat, CultureInfo.InvariantCulture);

            JSONArray scale = new JSONArray();
            scale.Add(entry.Value.x);
            scale.Add(entry.Value.y);
            scale.Add(entry.Value.z);

            JSONObject timestampedPosition = new JSONObject();
            timestampedPosition.Add(timestamp, scale);

            scaleHist.Add(timestampedPosition);
        }
        output.Add("scaleHistory", scaleHist);
        #endregion

        return output.ToString(Settings.PrettyJSONExportIndent);
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
    public static List<KeyValuePair<string, string>> FetchSessions()
    {
        string[] filePaths = Directory.GetFiles(DataDirectoryName, "*" + FileExtension, searchOption: SearchOption.AllDirectories);
        Debug.Log(filePaths.ToCommaSeparatedString());

        List<KeyValuePair<string, string>> sessionIdentifiers = new(); //identifier, filepath
        //TODO: Maybe this could be improved by implementing the saving in a way where each session gets it's own session subfolder. That way, the reading system would only have to read the folder name. This way, the user would also be able to name the folder any way they want and it would come up in the app this way. Furthermore, it would enable the storage of an additional session meta file, containing information about start and end times, session duration, participant number, etc.
        foreach (string filePath in filePaths)
        {
            string fileContent = File.ReadAllText(filePath);
            JSONNode data = JSONNode.Parse(fileContent);

            // Debug.Log($"{data[KEY_SESSION_ID]}");
            string sessionIdentifier = data[KEY_SESSION_ID];
            sessionIdentifiers.Add(new(sessionIdentifier, filePath));
        }
        return sessionIdentifiers;
    }

    public static Dictionary<string, string> GetSessionContents(string sessionIdentifier)
    {
        Dictionary<string, string> output = new(); // fileIdentifier, content

        string[] filePaths = Directory.GetFiles(dataDir, "*" + FileExtension, searchOption: SearchOption.AllDirectories);
        foreach (string filePath in filePaths)
        {
            string fileContentString = File.ReadAllText(filePath);
            JSONNode fileConstentNode = JSONNode.Parse(fileContentString);
            if (fileConstentNode[KEY_SESSION_ID] != sessionIdentifier)
                continue;
            if (fileConstentNode[KEY_APP_VERSION] != Application.version)
                continue;
            string fileIdentifier = fileConstentNode[KEY_DYNOBJ_ID];
            if (string.IsNullOrEmpty(fileIdentifier))
                fileIdentifier = "gazepoints";
            output.TryAdd(fileIdentifier, fileContentString);
        }
        return output;
    }
    #endregion
}