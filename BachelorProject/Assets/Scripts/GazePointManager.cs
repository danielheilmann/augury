using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GazePointManager : MonoBehaviour
{
    //#> Static Variables 
    public static GazePointManager Instance { get; private set; }
    public static UnityEvent<Vector3> OnPointCreated = new();

    //#> Private Variables 
    [SerializeField] private Dictionary<DateTime, Vector3> points;
    //? For memory-optimization, maybe I should use two arrays instead?
    //?~ Instead, I could also save the points to a file occasionally (e.g. in batches, every 5 minutes). This would lighten the load on RAM but increase Disk R/W.
    [SerializeField] private int pointCapacity; //< Determines the amount of GazePoints allowed to be stored in memory.
    [SerializeField] private int currentPointCount;

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

    private void Start()
    {
        pointCapacity = Mathf.RoundToInt(Settings.expectedSessionRuntimeInMinutes * 60 * Timer.ticksPerSecond); //< e.g. a capacity of 65536 corresponds to ~11 Minutes at 100 Ticks/s.
        points = new Dictionary<DateTime, Vector3>(pointCapacity);
    }

    public void CreatePointAt(Vector3 position)
    {
        DateTime timeStamp = DateTime.Now;  //TODO: Maybe I should store the realtimesincestartup instead, as it would be less data (probably) and is definitely more relevant for the replay feature.
        Debug.Log($"{timeStamp} | Created Point at {position}");

        points.Add(timeStamp, position);
        OnPointCreated.Invoke(position);
        currentPointCount++;

        //> Automatically increase GazePoint dictionary size if necessary.
        if (currentPointCount >= pointCapacity)
        {
            Debug.LogWarning($"You have just surpassed {pointCapacity} SpatialPoint entries. Doubling dictionary length (to {pointCapacity * 2}). It is recommended to either keep sessions shorter, edit the expected session duration (currently {Settings.expectedSessionRuntimeInMinutes} min.) or reduce the tick rate (currently at {Timer.ticksPerSecond} ticks/s).");
            pointCapacity *= 2;
            points.EnsureCapacity(pointCapacity);
        }
    }

    private void OnDisable()
    {
        Save();
    }

    private void Save()
    {
        string filepath = FileSystemHandler.SaveGazePointsToFile(fileTitle: $"Session {SessionManager.sessionStartTime.ToString("yyyy-MM-dd HH-mm-ss")}", points);
        if (Settings.OpenExplorerOnSave)
            FileSystemHandler.OpenFileExplorerAt(filepath);
    }
}
