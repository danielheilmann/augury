using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Should be disabled in the hierarchy as it will be enabled by the GameManager once needed.
public class StartGameUponLeavingArea : MonoBehaviour
{
    private GameObject playerCharacter;
    [SerializeField] private float areaRadiusInMeters = 2f;

    private void OnEnable() => GameManager.OnGameStart.AddListener(DeactivateSelf); //< Deactivate this script once the game has started as its job is done.
    //TODO: Could use OnReadyForGameStart here
    private void OnDisable() => GameManager.OnGameStart.RemoveListener(DeactivateSelf);
    private void DeactivateSelf() => this.enabled = false;

    private void Update()
    {
        // if (GameManager.currentGameStage != GameManager.GameStage.WaitingForPlayerInput)
        //     return;

        //< All of this is not needed any more, because this object stays inactive until the GameManager enables it.

        if (Vector3.Distance(GameManager.Instance.PlayerCharacter.transform.position, transform.position) > areaRadiusInMeters)
            GameManager.Instance.StartGame();
    }


    /// <summary>
    /// Callback to draw gizmos only if the object is selected.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, areaRadiusInMeters);
    }
}
