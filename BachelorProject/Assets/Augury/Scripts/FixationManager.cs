using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//TODO | Feature List:
// 1. Distance check before creation of fixations to make sure fixations are not created right next to each other.
// 2. Adaptive circle size based on amount of gazepoints in active group before fixation creation
public class FixationManager : MonoBehaviour
{
    public static UnityEvent<Fixation> OnFixationCreated = new();
    public static FixationManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private float distanceThresholdInMeters = 0.3f;
    [SerializeField] private int pointCountThresholdForFixationCreation = 4;  //? Maybe change this so that the tweakable variable states how long (in milliseconds) a point needs to be fixated? (e.g. calculate the current variable against timer tick rate)

    [Header("Visualization of Private Lists")]  //> Only serialized for visualization in editor
    [SerializeField, NonReorderable, ReadOnly] private List<GazePoint> activeGazePointGroup = new();
    [SerializeField, NonReorderable, ReadOnly] private List<Fixation> fixations = new();

    private int effectiveDurationThresholdInMilliseconds => Mathf.RoundToInt((1000 / Timer.ticksPerSecond) * pointCountThresholdForFixationCreation);
    
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
        if (Settings.visualizeInRecordMode)  //< No need to evaluate fixations if they are not going to be visualized.
        {
            SessionManager.OnRecordStart.AddListener(OnSessionStart);
            SessionManager.OnRecordStop.AddListener(OnSessionStop);
        }

        SessionManager.OnReplayStart.AddListener(OnSessionStart);
        SessionManager.OnReplayStop.AddListener(OnSessionStop);
    }

    private void OnDisable()
    {
        GazePointManager.OnPointCreated.RemoveListener(EvaluateFixation);
    }

    private void OnDestroy() //> Unsubscribe from all events to prevent null reference exceptions
    {
        SessionManager.OnRecordStart.RemoveListener(OnSessionStart);
        SessionManager.OnRecordStop.RemoveListener(OnSessionStop);

        SessionManager.OnRecordStart.RemoveListener(OnSessionStart);
        SessionManager.OnRecordStop.RemoveListener(OnSessionStop);
    }

    private void OnSessionStart()
    {
        GazePointManager.OnPointCreated.AddListener(EvaluateFixation);
    }

    private void OnSessionStop()
    {
        Clear();
        GazePointManager.OnPointCreated.RemoveListener(EvaluateFixation);
    }

    private void Clear()
    {
        //?> Maybe using .Clear() is better than creating a new list every time in terms of memory management?
        activeGazePointGroup.Clear();
        fixations.Clear();
    }

    #region Create Fixation
    private void EvaluateFixation(GazePoint gazePoint)
    {
        Vector3 currentFixationGroupAveragePosition = CalculateAveragePosition(activeGazePointGroup);
        bool evaluationTerminationCondition = Vector3.Distance(gazePoint.globalPosition, currentFixationGroupAveragePosition) > distanceThresholdInMeters || IsOnDifferentDynObj(gazePoint.dynObjID);

        if (evaluationTerminationCondition)
        {
            if (activeGazePointGroup.Count > pointCountThresholdForFixationCreation) //< If current PointGroup contains enough points, collapse into a new Fixation.
            {
                Vector3 averageSurfaceNormal = CalculateAverageSurfaceNormal(activeGazePointGroup);
                int fixationID = fixations.Count;
                CreateFixation(currentFixationGroupAveragePosition, averageSurfaceNormal, fixationID, activeGazePointGroup[0].dynamicObject);
            }
            activeGazePointGroup.Clear();
        }

        activeGazePointGroup.Add(gazePoint);

        //> Local Functions
        Vector3 CalculateAveragePosition(List<GazePoint> gazePoints)
        {
            Vector3 sum = Vector3.zero;

            foreach (GazePoint point in gazePoints)
                sum += point.globalPosition;

            return sum /= gazePoints.Count;
        }

        Vector3 CalculateAverageSurfaceNormal(List<GazePoint> gazePoints)
        {
            Vector3 sum = Vector3.zero;

            foreach (GazePoint point in gazePoints)
                sum += point.surfaceNormal;

            return sum /= gazePoints.Count;
        }
    }

    private void CreateFixation(Vector3 globalPosition, Vector3 surfaceNormal, int fixationIndex, DynamicObject dynObj = null)
    {
        Fixation newFixation = new Fixation(globalPosition, surfaceNormal, fixationIndex, dynamicObject: dynObj);
        fixations.Add(newFixation);
        OnFixationCreated.Invoke(newFixation);
    }

    private bool IsOnDifferentDynObj(string id)
    {
        if (activeGazePointGroup.Count > 0)
            return id != activeGazePointGroup[activeGazePointGroup.Count - 1].dynObjID;
        else
            return false;
    }
    #endregion

    // private void OnDrawGizmos() //< For debug visualization
    // {
    //     float radius = 0.2f;
    //     Color gizmoColor = Color.red;

    //     for (int i = 0; i < fixations.Count; i++)
    //     {
    //         Vector3 thisFixation = fixations[i];

    //         Gizmos.color = gizmoColor;
    //         Gizmos.DrawSphere(thisFixation, radius);

    //         if (i + 1 <= fixations.Count - 1)   //< If the following check would not go out of bounds. (This is probably cheaper than a custom exception handler)
    //         {
    //             Vector3 nextFixation = fixations[i + 1];
    //             Gizmos.DrawLine(thisFixation, nextFixation);
    //         }
    //     }
    // }
}
