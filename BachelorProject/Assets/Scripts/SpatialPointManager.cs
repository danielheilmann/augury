using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpatialPointManager : MonoBehaviour
{
    public static SpatialPointManager Instance { get; private set; }
    public static UnityEvent<SpatialPoint> OnPointCreated = new();

    [SerializeField] private List<SpatialPoint> points = new List<SpatialPoint>();

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

    public SpatialPoint CreatePointAt(Vector3 position)
    {
        DateTime timeStamp = DateTime.Now; // TODO: Maybe it's the DateTime.Now that is causing the lags actually...
        SpatialPoint point = new SpatialPoint(timeStamp, position);  
        // TODO: This is the exact point that causes the lags. It got better after making SpatialPoints structs and reducing the framerate to 60fps, but at 20 Ticks/s, the lag is still very noticable. Maybe a solution would be to store them all in a Dictionary instead?
        Debug.Log($"{timeStamp} | Created Point at {position}");

        points.Add(point);
        OnPointCreated.Invoke(point);
        return point;
    }

}
