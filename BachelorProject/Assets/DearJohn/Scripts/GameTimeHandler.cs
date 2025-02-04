using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimeHandler : MonoBehaviour
{
    public static float EndGameAfterSeconds = 4 * 60; //< Game ends automatically after exactly 4 minutes.
    public static GameTimeHandler Instance { get; private set; }
    public static float currentGameTimeInSeconds = 0;
    private bool isPaused = true;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    private void OnEnable()
    {
        GameManager.OnGameStart.AddListener(Begin);
        GameManager.OnGamePause.AddListener(Pause);
        GameManager.OnGameResume.AddListener(Resume);
        GameManager.OnGameEnd.AddListener(Stop);
    }

    private void OnDisable()
    {
        GameManager.OnGameStart.RemoveListener(Begin);
        GameManager.OnGamePause.RemoveListener(Pause);
        GameManager.OnGameResume.RemoveListener(Resume);
        GameManager.OnGameEnd.RemoveListener(Stop);
    }

    private void Update()
    {
        if (isPaused)
            return;

        if (SessionManager.currentMode == SessionManager.DataMode.Replay)
            currentGameTimeInSeconds += Time.deltaTime * ReplayManager.Instance.timeline.speedMultiplier;
        else
            currentGameTimeInSeconds += Time.deltaTime;

        // Debug.Log($"Current Game Time: {currentGameTimeInSeconds}");

        if (currentGameTimeInSeconds >= EndGameAfterSeconds)
        {
            GameManager.Instance.RollCredits();
        }
    }

    public void Begin()
    {
        isPaused = false;
    }

    public void Pause()
    {
        if (GameManager.hasGameStarted)
            isPaused = true;
    }

    public void Resume()
    {
        if (GameManager.hasGameStarted)
            isPaused = false;
    }

    public void Stop()
    {
        CustomReset();
    }

    public void CustomReset()
    {
        isPaused = true;
        currentGameTimeInSeconds = 0;
    }
}
