using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GazePointManager : MonoBehaviour
{
    //#> Static Variables 
    public static UnityEvent<GazePoint> OnPointCreated = new();

    //#> Private Variables 
    [SerializeField] private List<GazePoint> points;
    [SerializeField, ReadOnly] private int pointCapacity; //< Determines the amount of GazePoints allowed to be stored in memory.
    [SerializeField, ReadOnly] private int currentIndex;

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

        points.RemoveRange(currentIndex, pointCapacity - currentIndex); //< Trim unused points that were added during the last capacity increase.
        FileSystemHandler.SaveGazePoints(points);
    }

    private void EvaluateRaycastHit(RaycastHit hit)
    {
        if (hit.collider.gameObject.TryGetComponent<DynamicObject>(out DynamicObject dynObj))
            CreatePointAt(hit.point, hit.normal, dynObj);
        else
            CreatePointAt(hit.point, hit.normal);
    }

    private void CreatePointAt(Vector3 pointPosition, Vector3 hitNormal, DynamicObject connectedDynObj = null)
    {
        DateTime timeStamp = DateTime.Now;  //?: Maybe I should store the realtimesincestartup instead, as it would be less data (probably) and is definitely more relevant for the replay feature.

        GazePoint point = SetPoint(currentIndex, timeStamp, pointPosition, hitNormal, connectedDynObj);
        OnPointCreated.Invoke(point);
        currentIndex++;

        //> Automatically increase GazePoint list size whenever necessary.
        if (currentIndex >= pointCapacity)
            ExpandCachedList();

        void ExpandCachedList()
        {
            Debug.LogWarning($"You just surpassed the expected session duration of {pointCapacity / 60 / Timer.ticksPerSecond} minutes. This forced an increase in data capacity during runtime, which might cause measurements to be dropped due to the sudden drop in framerate.\nTo prevent this from happening, it is recommended to either keep sessions shorter, edit the expected session duration (currently {Settings.ExpectedSessionRuntimeInMinutes} minutes) or reduce the tick rate (currently at {Timer.ticksPerSecond} ticks/s).\n(Increased GazePoint list capacity from {pointCapacity} to {pointCapacity * 2}.)");
            pointCapacity *= 2;
            points.Capacity = pointCapacity;

            int capacityToFill = pointCapacity / 2;
            for (int i = 0; i < capacityToFill; i++)
                points.Add(new GazePoint(DateTime.Now, Vector3.zero));
        }
    }

    private GazePoint SetPoint(int index, DateTime timeStamp, Vector3 position, Vector3 surfaceNormal, DynamicObject dynObj = null)
    {
        if (dynObj == null)
        {
            points[index].Set(timeStamp, position, surfaceNormal, dynObj);
            // Debug.Log($"{timeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture)} | Created Global Point at {position}.");
        }
        else
        {
            Vector3 relativePosition = position - dynObj.transform.position;
            points[index].Set(timeStamp, relativePosition, surfaceNormal, dynObj);
            // Debug.Log($"{timeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture)} | Created Local Point at {position}, which is attached to \"{dynObj.name}\" with an offset of {relativePosition}", dynObj.gameObject);
        }

        return points[index];
    }

    /// <summary>
    /// Callback to draw gizmos that are pickable and always drawn.
    /// </summary>
    // private void OnDrawGizmos()
    // {
    //     foreach (var point in points)
    //     {
    //         if (point.attachedToDynObj)
    //         {
    //             Gizmos.color = Color.blue;
    //             Gizmos.DrawSphere(point.position + point.attachedToDynObj.transform.position, 0.2f);
    //         }
    //         else
    //         {
    //             Gizmos.color = Color.yellow;
    //             Gizmos.DrawSphere(point.position, 0.2f);
    //         }
    //     }
    // }
}
