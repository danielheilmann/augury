using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Events;

//TODO | Feature List:
// 1. Loading of GazePoints based on timecode (done)
// 2. Loading of DynamicObjects based on timecode (done)
// 3. Smooth Lerp between dynObj location

public class ReplayTimeline : MonoBehaviour
{
    public UnityEvent OnTimelineFinished { get; private set; } = new();
    [SerializeField] private List<KeyValuePair<DateTime, ReplayAction>> timeline = new();
    private DateTime currentTimelineTime = DateTime.MinValue;
    [SerializeField, ReadOnly] private bool isInitialized = false;
    [SerializeField, ReadOnly] private bool isPaused = true;
    [Range(0.1f, 10f)]
    [SerializeField] public float speedMultiplier = 1.0f;

    private void Update() => ProgressReplayTime();

    #region Filling Timeline with Actions
    public void GenerateActionsFromGazePointsJSON(JSONObject gazePointsJSON)
    {
        if (GazePointManager.Instance == null)  //< If the GazePoint Module is disabled or not present, do not load any GazePoints into the timeline.
            return;

        if (gazePointsJSON == null)
        {
            Debug.LogError($"Trying to generate actions from null gazePointsJSON. Aborting.");
            return;
        }

        JSONArray JSONPoints = gazePointsJSON[FileSystemHandler.KEY_GAZEPOINTS].AsArray;
        timeline.Capacity += JSONPoints.Count; //< Pre-allocate memory for the timeline to avoid resizing during playback.

        foreach (var item in JSONPoints)
        {
            JSONObject JSONPoint = item.Value.AsObject;
            DateTime timestamp = JSONPoint[FileSystemHandler.KEY_TIMESTAMP].AsDateTime;
            string dynObjID = JSONPoint[FileSystemHandler.KEY_DYNOBJ_ID].Value;
            Vector3 surfaceNormal = JSONPoint[FileSystemHandler.KEY_SURFACENORMAL].AsArray;
            Vector3 globalPosition = JSONPoint[FileSystemHandler.KEY_POSITION].AsArray;

            ReplayActionGP replayActionGP = new ReplayActionGP(timestamp, globalPosition, surfaceNormal, dynObjID);
            AddAction(replayActionGP.timestamp, replayActionGP);
        }
        GazePointManager.Instance.InitializeWithCustomCapacity(JSONPoints.Count); //< Creates capacity equal to the amount of GazePoints that will be loaded this session.
    }

    public void GenerateActionsFromDynamicObjectJSON(JSONObject dynamicObjectJSON)
    {
        if (dynamicObjectJSON == null)
        {
            Debug.LogError($"Trying to generate actions from null dynamicObjectsJSON. Aborting.");
            return;
        }

        // Dictionary<DateTime, ReplayActionDO> actions = new(); //< For merging actions with the same timestamp into one "transform update" action (feature request).

        //> Generate actions for each type of transform update
        DynamicObject dynamicObjectReference = DynamicObjectManager.GetByID(dynamicObjectJSON[FileSystemHandler.KEY_DYNOBJ_ID].Value);

        //> Load the history arrays
        JSONArray JSONPositionHistory = dynamicObjectJSON[FileSystemHandler.KEY_POSITIONHISTORY].AsArray;
        JSONArray JSONRotationHistory = dynamicObjectJSON[FileSystemHandler.KEY_ROTATIONHISTORY].AsArray;
        JSONArray JSONScaleHistory = dynamicObjectJSON[FileSystemHandler.KEY_SCALEHISTORY].AsArray;
        timeline.Capacity += JSONPositionHistory.Count + JSONRotationHistory.Count + JSONScaleHistory.Count; //< Pre-allocate memory for the timeline to avoid resizing during playback.

        #region Load Position Actions
        foreach (KeyValuePair<string, JSONNode> entry in JSONPositionHistory)
        {
            JSONNode item = entry.Value;
            DateTime timestamp = item[FileSystemHandler.KEY_TIMESTAMP].AsDateTime;
            Vector3 position = item[FileSystemHandler.KEY_POSITION].AsArray;

            ReplayActionDO replayActionDO = new ReplayActionDO(dynamicObjectReference, updatedPosition: position);
            AddAction(timestamp, replayActionDO);
        }
        #endregion

        #region Load Rotation Actions
        foreach (KeyValuePair<string, JSONNode> entry in JSONRotationHistory)
        {
            JSONNode item = entry.Value;
            DateTime timestamp = item[FileSystemHandler.KEY_TIMESTAMP].AsDateTime;
            Quaternion rotation = item[FileSystemHandler.KEY_ROTATION].AsArray;

            ReplayActionDO replayActionDO = new ReplayActionDO(dynamicObjectReference, updatedRotation: rotation);
            AddAction(timestamp, replayActionDO);
        }
        #endregion

        #region Load Scale Actions
        foreach (KeyValuePair<string, JSONNode> entry in JSONScaleHistory)
        {
            JSONNode item = entry.Value;
            DateTime timestamp = item[FileSystemHandler.KEY_TIMESTAMP].AsDateTime;
            Vector3 scale = item[FileSystemHandler.KEY_SCALE].AsArray;

            ReplayActionDO replayActionDO = new ReplayActionDO(dynamicObjectReference, updatedScale: scale);
            AddAction(timestamp, replayActionDO);
        }
        #endregion
    }

    private void AddAction(DateTime timestamp, ReplayAction action)
    {
        timeline.Add(new KeyValuePair<DateTime, ReplayAction>(timestamp, action));
    }
    #endregion

    #region Replaying from Timeline
    /// <summary>
    /// Initializes the timeline. Replay cannot be started before this method has been called.
    /// </summary>
    /// <param name="startTimeOfLoadedSession">Requires the start time of the loaded session to calibrate the timeline time.</param>
    public void Initialize(DateTime startTimeOfLoadedSession)
    {
        //< Note: There is no need to clear or reset any of these values before initialization because the Timeline will be destroyed and re-created each time a Replay Session is started.

        currentTimelineTime = startTimeOfLoadedSession;

        //> Guard clauses
        if (timeline.Count == 0)
        {
            Debug.LogError($"Failed to initialize. Timeline contains no actions.");
            return;
        }
        if (currentTimelineTime == DateTime.MinValue)
        {
            Debug.LogError($"Failed to initialize. Timeline time was not set correctly.");
            return;
        }

        //> Sort timeline by timestamp
        timeline.Sort((a, b) => a.Key.CompareTo(b.Key));

        isInitialized = true;
        Debug.Log($"Timeline has successfully been initialized with {timeline.Count} actions.");
    }

    public void Play()
    {
        if (!isInitialized)
            Debug.LogWarning($"Cannot begin playback before initialization is complete. Please run Initialize().");
        else
            isPaused = false;
    }

    public void Pause()
    {
        isPaused = true;
    }

    private void ProgressReplayTime()
    {
        if (isPaused || !isInitialized)
            return;

        while (currentTimelineTime >= timeline[0].Key)  //< The entry at [0] keeps getting removed, so this will always be the next entry to be processed.
        {
            switch (timeline[0].Value.type)
            {
                case ReplayAction.ActionType.GazePoint:
                    ProcessGPAction(timeline[0].Value as ReplayActionGP);
                    break;
                case ReplayAction.ActionType.DynamicObject:
                    ProcessDOAction(timeline[0].Value as ReplayActionDO);
                    break;
            }

            timeline.RemoveAt(0); //< Is probably not very memory friendly, maybe going with a simple incrementer would be better.

            if (timeline.Count == 0)  //< Break out of the loop immediately (and skip time incrementation) if there are no more actions to process.
            {
                OnTimelineFinished.Invoke();
                isInitialized = false;
                return;
            }
        }

        //> Increment timeline time
        currentTimelineTime = currentTimelineTime.AddSeconds(Time.deltaTime * speedMultiplier);
    }

    private void ProcessGPAction(ReplayActionGP action)
    {
        GazePointManager.Instance.LoadGazePoint(action.timestamp, action.globalPosition, action.surfaceNormal, action.dynamicObjectReference);
    }

    private void ProcessDOAction(ReplayActionDO action)
    {
        DynamicObject dynamicObject = action.dynamicObjectReference;

        //> Only update the respective transform variable if the action actually contains a new value.
        if (action.updatedPosition.HasValue)
            dynamicObject.OverwritePosition(action.updatedPosition.Value);
        if (action.updatedRotation.HasValue)
            dynamicObject.OverwriteRotation(action.updatedRotation.Value);
        if (action.updatedScale.HasValue)
            dynamicObject.OverwriteScale(action.updatedScale.Value);
    }
    #endregion
}
