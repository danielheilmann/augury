using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Should be disabled in the hierarchy as it will be enabled by the GameManager once needed.
public class StartGameUponLeavingArea : MonoBehaviour
{
    [SerializeField] private float areaRadiusInMeters = 2f;
    private GameObject playerCharacter;

    private void OnEnable()
    {
        playerCharacter = GameManager.Instance.GetCurrentPlayerCharacter;
        GameManager.OnGameStart.AddListener(DeactivateSelf); //< Deactivate this script once the game has started as its job is done.
    }

    private void OnDisable() => GameManager.OnGameStart.RemoveListener(DeactivateSelf);
    private void DeactivateSelf() => this.enabled = false;
    private void Update()
    {
        //> This object stays inactive until the GameManager enables it.
        if (Vector3.Distance(playerCharacter.transform.position, transform.position) > areaRadiusInMeters)
            GameManager.Instance.StartGame();
    }

    // private void OnTriggerExit(Collider other)
    // {
    //     if (other.gameObject == playerCharacter)
    //     {
    //         Debug.Log($"Player Exited");
    //         GameManager.Instance.StartGame();
    //     }
    // }

    /// <summary>
    /// /// /// Callback to draw gizmos only if the object is selected.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, areaRadiusInMeters);
    }
}
