using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixationManager : MonoBehaviour
{
    [SerializeField] private const float distanceThreshold = 0.1f;  //< Tweakable
    [SerializeField] private const int pointCountThresholdForFixationCreation = 10;  //< Tweakable

    [SerializeField] private List<Vector3> activeGazePointGroup = new(); //< Only serialized for visualization in editor
    [SerializeField] private List<Vector3> fixations = new List<Vector3>(); //< Only serialized for visualization in editor

    private void OnEnable()
    {
        GazePointManager.OnPointCreated.AddListener(EvaluateFixation);
    }

    private void OnDisable()
    {
        GazePointManager.OnPointCreated.RemoveListener(EvaluateFixation);
    }

    private void EvaluateFixation(Vector3 newGazePoint)
    {
        Vector3 currentFixationGroupAveragePosition = CalculateAveragePosition(activeGazePointGroup);

        if (Vector3.Distance(newGazePoint, currentFixationGroupAveragePosition) > distanceThreshold)
        {
            if (activeGazePointGroup.Count > pointCountThresholdForFixationCreation) //< Collapse current active fixation group into a fixation, but only if it contains enough points.
                fixations.Add(currentFixationGroupAveragePosition);

            activeGazePointGroup.Clear();
        }

        activeGazePointGroup.Add(newGazePoint);
    }

    private Vector3 CalculateAveragePosition(List<Vector3> vectors)
    {
        Vector3 sum = Vector3.zero;

        foreach (Vector3 vector in vectors)
            sum += vector;

        return sum /= vectors.Count; ;
    }

    private void OnDrawGizmos()
    {
        float radius = 0.2f;
        Color gizmoColor = Color.red;

        for (int i = 0; i < fixations.Count; i++)
        {
            Vector3 thisFixation = fixations[i];

            Gizmos.color = gizmoColor;
            Gizmos.DrawSphere(thisFixation, radius);

            if (i + 1 <= fixations.Count - 1)   //< If the following check would not go out of bounds. (This is probably cheaper than a custom exception handler)
            {
                Vector3 nextFixation = fixations[i + 1];
                Gizmos.DrawLine(thisFixation, nextFixation);
            }
        }
    }
}
