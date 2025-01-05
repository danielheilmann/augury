using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static UnityEvent OnGameStart = new UnityEvent();

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

    public void StartGame()
    {
        Debug.Log("Game Started!");
    }

    public void RollCredits()
    {
        // If (SessionManager.mode == Replay) clean up and then do nothing. Do not reset the scene.
        Debug.Log("Game Ended!");
    }
}
