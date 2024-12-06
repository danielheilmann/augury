using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GazePointManager : MonoBehaviour
{
    //#> Static Variables 
    public static UnityEvent<Vector3> OnPointCreated = new();

    //#> Private Variables 
    [SerializeField] private List<GazePoint> points;  //TODO: Will need to be modified for the implementation of localized gaze points.
    [SerializeField, ReadOnly] private int pointCapacity; //< Determines the amount of GazePoints allowed to be stored in memory.
    [SerializeField, ReadOnly] private int currentWorkingIndex;

    private void Start()
    {
        pointCapacity = Mathf.RoundToInt(Settings.ExpectedSessionRuntimeInMinutes * 60 * Timer.ticksPerSecond); //< e.g. a capacity of 6000 corresponds to 5 Minutes at 20 Ticks/s.
        points = new List<GazePoint>(pointCapacity);
        for (int i = 0; i < pointCapacity; i++)
            points.Add(new GazePoint(DateTime.Now, Vector3.zero));
    }

    private void OnEnable()
    {
        RayProvider.OnHit.AddListener(EvaluateRaycastHit);
    }

    private void OnDisable()
    {
        RayProvider.OnHit.RemoveListener(EvaluateRaycastHit);
        points.RemoveRange(currentWorkingIndex, pointCapacity - currentWorkingIndex); //TODO: Needs to be verified to work correctly.
        FileSystemHandler.SaveGazePoints(points);
    }

    private void EvaluateRaycastHit(RaycastHit hit)
    {
        CreatePointAt(hit.point);
    }

    public void CreatePointAt(Vector3 position)
    {
        DateTime timeStamp = DateTime.Now;  //TODO: Maybe I should store the realtimesincestartup instead, as it would be less data (probably) and is definitely more relevant for the replay feature.
        Debug.Log($"{timeStamp} | Created Point at {position}");

        SetPoint(currentWorkingIndex, timeStamp, position);

        OnPointCreated.Invoke(position);
        currentWorkingIndex++;

        //> Automatically increase GazePoint dictionary size if necessary.
        if (currentWorkingIndex >= pointCapacity)
        {
            Debug.LogWarning($"You have just surpassed the expected session duration of {pointCapacity / 60 / Timer.ticksPerSecond} minutes. This forced an increase in data capacity during runtime, which might cause measurements to be dropped due to the sudden drop in framerate.\nTo prevent this from happening, it is recommended to either keep sessions shorter, edit the expected session duration (currently {Settings.ExpectedSessionRuntimeInMinutes} minutes) or reduce the tick rate (currently at {Timer.ticksPerSecond} ticks/s).\n(Increased GazePoint list capacity from {pointCapacity} to {pointCapacity * 2}.)");
            pointCapacity *= 2;
            points.Capacity = pointCapacity;

            int capacityToFill = pointCapacity / 2;
            for (int i = 0; i < capacityToFill; i++)
                points.Add(new GazePoint(DateTime.Now, Vector3.zero));
        }
    }

    private void SetPoint(int index, DateTime timeStamp, Vector3 position)
    {
        points[index].Set(timeStamp, position);
    }
}
