using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FixationManager : MonoBehaviour
{
    //#> Constants 
    [SerializeField] private const float distanceThreshold = 0.3f;  //< Tweakable
    [SerializeField] private const int pointCountThresholdForFixationCreation = 5;  //< Tweakable  //? Maybe change this so that the tweakable part states how long an are needs to be fixated? (calculate this against timer tick rate)

    //#> Static Variables 
    public static UnityEvent<Fixation> OnFixationCreated = new();

    //#> Private Variables 
    [SerializeField] private List<GazePoint> activeGazePointGroup = new(); //< Only serialized for visualization in editor
    [SerializeField] private List<Fixation> fixations = new List<Fixation>(); //< Only serialized for visualization in editor

    private void OnEnable()
    {
        GazePointManager.OnPointCreated.AddListener(EvaluateFixation);
    }

    private void OnDisable()
    {
        GazePointManager.OnPointCreated.RemoveListener(EvaluateFixation);
    }

    #region Create Fixation
    private void EvaluateFixation(GazePoint gazePoint)
    {
        Vector3 currentFixationGroupAveragePosition = CalculateAveragePosition(activeGazePointGroup);

        if (Vector3.Distance(gazePoint.position, currentFixationGroupAveragePosition) > distanceThreshold || IsDifferentIDthanLastEntry(gazePoint.dynObjID))
        {
            if (activeGazePointGroup.Count > pointCountThresholdForFixationCreation) //< Collapse current active newFixation group into a newFixation, but only if it contains enough points.
            {
                Vector3 averageSurfaceNormal = CalculateAverageSurfaceNormal(activeGazePointGroup);
                int fixationID = fixations.Count;
                CreateFixation(currentFixationGroupAveragePosition, averageSurfaceNormal, fixationID, activeGazePointGroup[0].dynamicObject);
            }
            activeGazePointGroup.Clear();
        }

        activeGazePointGroup.Add(gazePoint);
    }

    private Vector3 CalculateAveragePosition(List<GazePoint> gazePoints)
    {
        Vector3 sum = Vector3.zero;

        foreach (GazePoint point in gazePoints)
            sum += point.position;

        return sum /= gazePoints.Count;
    }

    private Vector3 CalculateAverageSurfaceNormal(List<GazePoint> gazePoints)
    {
        Vector3 sum = Vector3.zero;

        foreach (GazePoint point in gazePoints)
            sum += point.surfaceNormal;

        return sum /= gazePoints.Count;
    }

    private void CreateFixation(Vector3 fixationPosition, Vector3 surfaceNormal, int fixationIndex, DynamicObject dynObj = null)
    {
        Fixation newFixation = new Fixation(fixationPosition, surfaceNormal, fixationIndex, dynamicObject: dynObj);
        fixations.Add(newFixation);
        OnFixationCreated.Invoke(newFixation);
    }

    private bool IsDifferentIDthanLastEntry(int id)
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
