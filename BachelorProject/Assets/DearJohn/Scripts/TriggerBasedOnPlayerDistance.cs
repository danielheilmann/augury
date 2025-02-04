using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TriggerBasedOnPlayerDistance : MonoBehaviour
{
    [SerializeField] private float areaRadiusInMeters = 2f; //< This radius only applies to the X and Z axis, thereby forming a cylindrical "collision" shape.
    [SerializeField] private UnityEvent OnDistanceExceeded = new();
    private Coroutine coroutine;


    public void BeginChecking()
    {
        coroutine = StartCoroutine(CheckEachFrame());
    }

    public void StopChecking()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
        else
            Debug.LogWarning($"{this} is not checking for distance yet.");
    }

    private IEnumerator CheckEachFrame()
    {
        yield return new WaitForSeconds(1f); //< Wait for 1 second to ensure that the player follow and everything is in its right place.
        GameObject playerCharacter = GameManager.Instance.GetCurrentPlayerCharacter();

        while (true)
        {
            Vector3 flattenedPosition = new Vector3(transform.position.x, 0, transform.position.z);
            Vector3 flattenedPlayerPosition = new Vector3(playerCharacter.transform.position.x, 0, playerCharacter.transform.position.z);
            float flattenedDistance = Vector3.Distance(flattenedPosition, flattenedPlayerPosition);

            if (flattenedDistance > areaRadiusInMeters)
                OnDistanceExceeded.Invoke();

            yield return null;
        }
    }

    /// <summary> Callback to draw gizmos only if the object is selected. </summary>
    private void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, areaRadiusInMeters);
    }
}
