using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

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
    public GameObject PlayerCharacter => SessionManager.currentMode == SessionManager.DataMode.Replay ? Fakeplayer : XRRig;
    [SerializeField] private GameObject XRRig;
    [SerializeField] private GameObject Fakeplayer;
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

    private void OnEnable()
    {
        SessionManager.OnRecordStart.AddListener(Initialize);
    }

    private void OnDisable()
    {
        SessionManager.OnRecordStart.RemoveListener(Initialize);
    }

    [ContextMenu("Initialize")]
    public void Initialize()
    {
        hasGameStarted = false;
        isGamePaused = false;
        currentGameStage = GameStage.WaitingForPlayerInput;
        // PlayerCharacter = DiscoverActivePlayerCharacter();
        OnReadyForGameStart.Invoke();
    }

    [ContextMenu("Start Game")]
    public void StartGame()
    {
        Debug.Log("Game Started!");
        hasGameStarted = true;
        currentGameStage = GameStage.InGame;
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

        if (SessionManager.currentMode == SessionManager.DataMode.Replay)
            // Do not proceed to post-game
            return;
        Debug.Log("Game Ended!");
        currentGameStage = GameStage.PostGame;
        OnGameEnd.Invoke();

        if (SessionManager.currentMode == SessionManager.DataMode.Record)
            SessionManager.StopCurrentSession();
    }

    private GameObject DiscoverActivePlayerCharacter()
    {
        return GameObject.FindGameObjectWithTag("Player");
    }
}
