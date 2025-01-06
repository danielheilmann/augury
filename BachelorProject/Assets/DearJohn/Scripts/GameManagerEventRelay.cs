using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class GameManagerEventRelay : MonoBehaviour
{
    public UnityEvent OnReadyForGameStart = new UnityEvent();
    public UnityEvent OnGameStart = new UnityEvent();
    public UnityEvent OnGamePause = new UnityEvent();
    public UnityEvent OnGameResume = new UnityEvent();
    public UnityEvent OnGameEnd = new UnityEvent();

    private void OnEnable()
    {
        GameManager.OnReadyForGameStart.AddListener(OnReadyForGameStart.Invoke);
        GameManager.OnGameStart.AddListener(OnGameStart.Invoke);
        GameManager.OnGamePause.AddListener(OnGamePause.Invoke);
        GameManager.OnGameResume.AddListener(OnGameResume.Invoke);
        GameManager.OnGameEnd.AddListener(OnGameEnd.Invoke);
    }

    private void OnDisable()
    {
        GameManager.OnReadyForGameStart.RemoveListener(OnReadyForGameStart.Invoke);
        GameManager.OnGameStart.RemoveListener(OnGameStart.Invoke);
        GameManager.OnGamePause.RemoveListener(OnGamePause.Invoke);
        GameManager.OnGameResume.RemoveListener(OnGameResume.Invoke);
        GameManager.OnGameEnd.RemoveListener(OnGameEnd.Invoke);
    }
}
