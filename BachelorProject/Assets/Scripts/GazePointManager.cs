using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GazePointManager : MonoBehaviour
{
    private const int MaxPointCountStoredInMemory = 65536;  //< Capacity of 65536 corresponds to ~11 Minutes at 100 Ticks/s. 

    public static GazePointManager Instance { get; private set; }
    public static UnityEvent<Vector3> OnPointCreated = new();

    // [SerializeField] private List<SpatialPoint> points = new List<SpatialPoint>();
    [SerializeField] private Dictionary<DateTime, Vector3> points = new(MaxPointCountStoredInMemory);
    //? For memory-optimization, maybe I should use two arrays instead?
    //?~ Instead, I could also save the points to a file occasionally (e.g. in batches, every 5 minutes). This would lighten the load on RAM but increase Disk R/W.
    [SerializeField] private int currentPointCount = 0;
    private int pointCapacity = MaxPointCountStoredInMemory;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Debug.LogError($"{this} cannot set itself as instance as one has already been set by {Instance.gameObject}. Deleting self.");
            Destroy(this);
        }
    }

    public void CreatePointAt(Vector3 position)
    {
        DateTime timeStamp = DateTime.Now;  //TODO: Maybe I should store the realtimesincestartup instead, as it would be less data (probably) and is definitely more relevant for the replay feature.
        Debug.Log($"{timeStamp} | Created Point at {position}");

        points.Add(timeStamp, position);
        OnPointCreated.Invoke(position);
        currentPointCount++;

        if (currentPointCount == pointCapacity)
        {
            Debug.LogWarning($"You have just surpassed {pointCapacity} SpatialPoint entries. Doubling dictionary length (to {pointCapacity * 2}). It is recommended to either keep sessions shorter or to reduce the tick rate (currently at {Timer.Instance.ticksPerSecond} ticks/s).");
            pointCapacity *= 2;
            points.EnsureCapacity(pointCapacity);
        }
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    private void OnDisable()
    {
        string formattedTimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");   //TODO: Ideally, this would use the session start time instead, but I guess I can implement that when there is an actual "Start Session" button.
        string filepath = FileSystemHandler.SaveDictionaryToFile("Session " + formattedTimeStamp, points);
        OpenFileExplorerAt(filepath);
    }

    public void OpenFileExplorerAt(string filePath)
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
}
