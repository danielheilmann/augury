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
        if (hit.collider.gameObject.TryGetComponent<DynamicObject>(out DynamicObject dynObj))
            CreatePointAt(hit.point, dynObj);
        else
            CreatePointAt(hit.point);
    }

    public void CreatePointAt(Vector3 pointPosition, DynamicObject connectedDynObj = null)
    {
        DateTime timeStamp = DateTime.Now;  //TODO: Maybe I should store the realtimesincestartup instead, as it would be less data (probably) and is definitely more relevant for the replay feature.

        GazePoint point = SetPoint(currentWorkingIndex, timeStamp, pointPosition, connectedDynObj);
        OnPointCreated.Invoke(point);
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

    private GazePoint SetPoint(int index, DateTime timeStamp, Vector3 position, DynamicObject dynObj = null)
    {
        if (dynObj == null)
        {
            points[index].Set(timeStamp, position, dynObj);
            Debug.Log($"{timeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture)} | Created Global Point at {position}.");
        }
        else
        {
            Vector3 relativePosition = position - dynObj.transform.position;    //TODO: This offset is causing issues because it does not take into account scaled objects. If an object is scaled, the offset will be wrong because it also gets multiplied by the scaling modifier. (for some reason) But if I just adjust the relative position when the gazepoint is created, they will become misplaced if the object changes scale after they attach themselves. So maybe I should forgo parenting entirely and instead include my own following behaviour in the... DynamicObject? GazePointVisualizers?
            points[index].Set(timeStamp, relativePosition, dynObj);
            Debug.Log($"{timeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture)} | Created Local Point at {position}, which is attached to \"{dynObj.name}\" with an offset of {relativePosition}", dynObj.gameObject);
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
