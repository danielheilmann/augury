using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//TODO: Newly added dynamic gaze points are not supported by FixationManager yet
public class FixationManager : MonoBehaviour
{
    //#> Constants 
    [SerializeField] private const float distanceThreshold = 0.1f;  //< Tweakable
    [SerializeField] private const int pointCountThresholdForFixationCreation = 10;  //< Tweakable  //? Maybe change this so that the tweakable part states how long an are needs to be fixated? (calculate this against timer tick rate)

    //#> Static Variables 
    public static UnityEvent<Vector3, int, DynamicObject> OnFixationCreated = new();

    //#> Private Variables 
    [SerializeField] private List<GazePoint> activeGazePointGroup = new(); //< Only serialized for visualization in editor
    [SerializeField] private List<Vector3> fixations = new List<Vector3>(); //< Only serialized for visualization in editor

    private void OnEnable()
    {
        GazePointManager.OnPointCreated.AddListener(EvaluateFixation);
    }

    private void OnDisable()
    {
        GazePointManager.OnPointCreated.RemoveListener(EvaluateFixation);
    }

    private void EvaluateFixation(GazePoint gazePoint)
    {
        Vector3 currentFixationGroupAveragePosition = CalculateAveragePosition(activeGazePointGroup);

        if (Vector3.Distance(gazePoint.position, currentFixationGroupAveragePosition) > distanceThreshold || IsDifferentIDthanLastEntry(gazePoint.dynObjID))
        {
            if (activeGazePointGroup.Count > pointCountThresholdForFixationCreation) //< Collapse current active fixation group into a fixation, but only if it contains enough points.
                CreateFixation(currentFixationGroupAveragePosition, activeGazePointGroup[0].dynamicObject);

            activeGazePointGroup.Clear();
        }

        activeGazePointGroup.Add(gazePoint);
    }

    private Vector3 CalculateAveragePosition(List<GazePoint> gazePoints)
    {
        Vector3 sum = Vector3.zero;

        foreach (GazePoint point in gazePoints)
            sum += point.position;

        return sum /= gazePoints.Count; ;
    }

    private void CreateFixation(Vector3 position, DynamicObject dynObj)
    {
        fixations.Add(position);
        OnFixationCreated.Invoke(position, fixations.Count, dynObj);
    }

    private bool IsDifferentIDthanLastEntry(int id)
    {
        if (activeGazePointGroup.Count > 0)
            return id != activeGazePointGroup[activeGazePointGroup.Count-1].dynObjID;
        else
            return false;
    }

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
