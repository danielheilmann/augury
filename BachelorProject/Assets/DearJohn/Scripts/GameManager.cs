using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    // public enum GameStage { Menu, WaitingForPlayerInput, InGame, PostGame }
    // public enum GameStatus { NotStarted, Paused, Running, Ended }

    public static GameManager Instance;
    public static UnityEvent OnReadyForGameStart = new UnityEvent();
    public static UnityEvent OnGameStart = new UnityEvent();
    public static UnityEvent OnGamePause = new UnityEvent();
    public static UnityEvent OnGameResume = new UnityEvent();
    public static UnityEvent OnGameEnd = new UnityEvent();
    // public static GameStage currentGameStage = GameStage.Menu;
    public static bool hasGameStarted = false;
    public static bool isGamePaused = false;
    [SerializeField] private GameObject XRRig;
    [SerializeField] private GameObject Fakeplayer;

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

    [ContextMenu("Go to WaitingForPlayerInput")]
    public void Initialize()
    {
        Debug.Log("Game Initialized!");
        hasGameStarted = false;
        isGamePaused = false;
        // currentGameStage = GameStage.WaitingForPlayerInput;
        // PlayerCharacter = DiscoverActivePlayerCharacter();
        OnReadyForGameStart.Invoke();
    }

    [ContextMenu("Start Game")]
    public void StartGame()
    {
        Debug.Log("Game Started!");
        hasGameStarted = true;
        // currentGameStage = GameStage.InGame;
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

        if (SessionManager.currentMode == SessionManager.DataMode.Replay)  //< Do not proceed to post-game when Replaying earlier data
            return;

        Debug.Log("Game Ended!");
        // currentGameStage = GameStage.PostGame;
        OnGameEnd.Invoke();
    }

    public GameObject GetCurrentPlayerCharacter()   //< Could be implemented better (performance-wise) but it's okay for now
    {
        if (SessionManager.currentMode == SessionManager.DataMode.Replay)
            return Fakeplayer; //< The representation of the player that is driven by DynamicObject-Updates on the ReplayTimeline.
        else
            return XRRig;
    }
}
