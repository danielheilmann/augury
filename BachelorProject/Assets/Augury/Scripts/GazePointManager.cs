using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GazePointManager : MonoBehaviour
{
    public static GazePointManager Instance { get; private set; } //< To prevent a situation where multiple managers track the same GazePoints.
    public static UnityEvent<GazePoint> OnPointCreated = new();

    [SerializeField] private List<GazePoint> points;
    [SerializeField, ReadOnly] private int pointCapacity; //< Determines the amount of GazePoints allowed to be stored in memory.
    [SerializeField, ReadOnly] private int currentIndex = 0;

    #region Monobehaviour Methods
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

    private void OnEnable()
    {
        // RayProvider.OnHit.AddListener(EvaluateRaycastHit);
        SessionManager.OnRecordStart.AddListener(OnRecordSessionStart);
        SessionManager.OnRecordStop.AddListener(OnRecordSessionStop);

        //> Just in case for if this object was disabled when the recording session was started.
        if (SessionManager.currentMode == SessionManager.DataMode.Record)
            OnRecordSessionStart();
    }

    private void OnDisable()
    {
        RayProvider.OnHit.RemoveListener(EvaluateRaycastHit);
    }

    private void OnDestroy() //?< Actually, there should never be a case where the manager would be destroyed before a session stop, but still.
    {
        SessionManager.OnRecordStart.RemoveListener(OnRecordSessionStart);
        SessionManager.OnRecordStop.RemoveListener(OnRecordSessionStop);

        if (SessionManager.currentMode == SessionManager.DataMode.Record)
            SaveGazePoints();
    }
    #endregion

    #region Event Hooks
    private void OnRecordSessionStart()
    {
        Initialize();
        RayProvider.OnHit.AddListener(EvaluateRaycastHit);
    }

    private void OnRecordSessionStop()
    {
        RayProvider.OnHit.RemoveListener(EvaluateRaycastHit);
        SaveGazePoints();
    }
    #endregion

    private void Initialize()
    {
        int expectedRequiredCapacity = Mathf.RoundToInt(Settings.expectedSessionRuntimeInMinutes * 60 * Timer.ticksPerSecond); //< e.g. a capacity of 6000 corresponds to 5 Minutes at 20 Ticks/s.
        InitializeWithCustomCapacity(expectedRequiredCapacity);
    }
    public void InitializeWithCustomCapacity(int capacity)
    {
        pointCapacity = Mathf.Max(1, capacity); //< Ensure that the capacity is at least 1.
        points = new List<GazePoint>(pointCapacity);
        for (int i = 0; i < pointCapacity; i++)
            points.Add(new GazePoint());
        currentIndex = 0;
    }

    private void SaveGazePoints()
    {
        points.RemoveRange(currentIndex, pointCapacity - currentIndex); //< Trim unused points that were added during the last capacity increase. TrimExcess() cannot be used because the "unused" capacity is already filled with buffer GazePoints.
        FileSystemHandler.SaveGazePoints(points);
    }

    #region Evaluation of Raycast into of GazePoint
    private void EvaluateRaycastHit(RaycastHit hit)
    {
        if (hit.collider.gameObject.TryGetComponent<DynamicObjectColliderHelper>(out DynamicObjectColliderHelper helper)) //< This is necessary because most of the DynamicObjects hald their colliders in a child object.
            CreatePointAtPosition(hit.point, hit.normal, helper.dynamicObject);
        else if (hit.collider.gameObject.TryGetComponent<DynamicObject>(out DynamicObject dynObj))
            CreatePointAtPosition(hit.point, hit.normal, dynObj);
        else
            CreatePointAtPosition(hit.point, hit.normal);
    }

    private void CreatePointAtPosition(Vector3 globalPosition, Vector3 hitNormal, DynamicObject connectedDynObj = null)
    {
        DateTime timeStamp = Timer.latestTimestamp;  //?: Maybe I should store the realtimesincestartup instead, as it would be less data (probably) and is definitely more relevant for the replay feature.

        SetPoint(timeStamp, globalPosition, hitNormal, connectedDynObj);

        //> Automatically increase GazePoint list size whenever necessary.
        if (currentIndex >= pointCapacity)
            ExpandCachedList();

        void ExpandCachedList()
        {
            Debug.LogWarning($"You just surpassed the expected session duration of {pointCapacity / 60 / Timer.ticksPerSecond} minutes. This forced an increase in data capacity during runtime, which might cause measurements to be dropped due to the sudden drop in framerate.\nTo prevent this from happening, it is recommended to either keep sessions shorter, edit the expected session duration (currently {Settings.expectedSessionRuntimeInMinutes} minutes) or reduce the tick rate (currently at {Timer.ticksPerSecond} ticks/s).\n(Increased GazePoint list capacity from {pointCapacity} to {pointCapacity * 2}.)");
            pointCapacity *= 2;
            points.Capacity = pointCapacity;

            int capacityToFill = pointCapacity / 2;
            for (int i = 0; i < capacityToFill; i++)
                points.Add(new GazePoint());
        }
    }
    private GazePoint SetPoint(DateTime timeStamp, Vector3 globalPosition, Vector3 surfaceNormal, DynamicObject dynObj = null)
    {
        return SetPoint(currentIndex, timeStamp, globalPosition, surfaceNormal, dynObj);
    }

    private GazePoint SetPoint(int index, DateTime timeStamp, Vector3 globalPosition, Vector3 surfaceNormal, DynamicObject dynObj = null) //< This overload allows for setting / overwriting a specific index.
    {
        GazePoint point;

        point = points[index].Set(timeStamp, globalPosition, surfaceNormal, dynObj);
        // Debug.Log($"{timeStamp.ToString(FileSystemHandler.TimestampFormat, System.Globalization.CultureInfo.InvariantCulture)} | Created Global Point at {position}.");
        OnPointCreated.Invoke(point);

        currentIndex++;
        return point;
    }
    #endregion

    public void LoadGazePoint(DateTime timeStamp, Vector3 position, Vector3 surfaceNormal, DynamicObject dynObj = null)
    {
        SetPoint(timeStamp, position, surfaceNormal, dynObj);
    }
}
