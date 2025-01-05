using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;

public class GameManager : MonoBehaviour
{
    public enum GameStage { Menu, WaitingForPlayerInput, InGame, PostGame }
    // public enum GameStatus { NotStarted, Paused, Running, Ended }

    public static GameManager Instance;
    public static UnityEvent OnReadyForGameStart = new UnityEvent();
    public static UnityEvent OnGameStart = new UnityEvent();
    public static UnityEvent OnGamePause = new UnityEvent();
    public static UnityEvent OnGameResume = new UnityEvent();
    public static UnityEvent OnGameEnd = new UnityEvent();
    public GameObject activePlayerCharacter;
    public static GameStage currentGameStage = GameStage.Menu;
    public static bool hasGameStarted = false;
    public static bool isGamePaused = false;

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

    [ContextMenu("Initialize")]
    private void Initialize()
    {
        hasGameStarted = false;
        isGamePaused = false;
        activePlayerCharacter = DiscoverActivePlayerCharacter();
        currentGameStage = GameStage.WaitingForPlayerInput;
        OnReadyForGameStart.Invoke();

        FindObjectOfType<StartGameUponLeavingArea>(true).enabled = true;
    }

    [ContextMenu("Start Game")]
    public void StartGame()
    {
        Debug.Log("Game Started!");
        hasGameStarted = true;
        OnGameStart.Invoke();
    }

    [ContextMenu("Pause Game")]
    public void PauseGame()
    {
        if (isGamePaused) { Debug.Log($"Game is already paused!"); return; }

        isGamePaused = true;
        Debug.Log("Game Paused!");
        OnGamePause.Invoke();
    }

    [ContextMenu("Resume Game")]
    public void ResumeGame()
    {
        if (!isGamePaused) { Debug.Log($"Game is not paused!"); return; }

        Debug.Log("Game Resumed!");
        OnGameResume.Invoke();
    }

    [ContextMenu("End Game")]
    public void RollCredits()
    {
        if (!hasGameStarted) { Debug.Log($"Game has not started yet!"); return; }

        // If (SessionManager.mode == Replay) clean up and then do nothing. Do not reset the scene.
        Debug.Log("Game Ended!");
        OnGameEnd.Invoke();
    }

    private GameObject DiscoverActivePlayerCharacter()
    {
        return GameObject.FindGameObjectWithTag("Player");
    }
}
