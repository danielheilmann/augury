using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpatialPointManager : MonoBehaviour
{
    [SerializeField] private const int MaxPointCountStoredInMemory = 4; //65536;
    public static SpatialPointManager Instance { get; private set; }
    public static UnityEvent<Vector3> OnPointCreated = new();
    
    // [SerializeField] private List<SpatialPoint> points = new List<SpatialPoint>();
    [SerializeField] private Dictionary<DateTime, Vector3> points = new(MaxPointCountStoredInMemory); //< Capacity of 65536 corresponds to ~11 Minutes at 100 Ticks/s. 
    //? For memory-optimization, maybe I should use two arrays instead?
    //? Instead, I could also save the points to a file occasionally (e.g. in batches, every 5 minutes). This would lighten the load on RAM but increase Disk R/W.
    [SerializeField] private int pointCount = 0;
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
        DateTime timeStamp = DateTime.Now;
        Debug.Log($"{timeStamp} | Created Point at {position}");

        points.Add(timeStamp, position);
        OnPointCreated.Invoke(position);
        pointCount++;

        if (pointCount == pointCapacity)
        {
            Debug.LogWarning($"You have just surpassed {pointCapacity} SpatialPoint entries. Doubling dictionary length (to {pointCapacity*2}). It is recommended to either keep sessions shorter or to reduce the tick rate (currently at {Timer.Instance.ticksPerSecond} ticks/s).");
            pointCapacity *= 2;
            points.EnsureCapacity(pointCapacity);
        }
    }
}
