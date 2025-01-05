using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    private const float FictionalStartingTimeOffsetInHours = 3f;
    [SerializeField] Transform hourHand;
    [SerializeField] Transform minuteHand;
    private Coroutine coroutine;
    // private bool isPaused;

    private void OnEnable()
    {
        GameManager.OnGameStart.AddListener(StartClock);
        GameManager.OnGamePause.AddListener(StopClock);
        GameManager.OnGameResume.AddListener(StartClock);
        GameManager.OnGameEnd.AddListener(StopClock);
    }

    private void OnDisable()
    {
        GameManager.OnGameStart.RemoveListener(StartClock);
        GameManager.OnGamePause.RemoveListener(StopClock);
        GameManager.OnGameResume.RemoveListener(StartClock);
        GameManager.OnGameEnd.RemoveListener(StopClock);
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    private void Start()
    {
        SetClock(FictionalStartingTimeOffsetInHours);
    }

    private void StartClock()
    {
        coroutine = StartCoroutine(TickEverySecond());
    }

    private void StopClock()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
    }

    private IEnumerator TickEverySecond()
    {
        while (true)
        {
            //> Each fictional hour is 60 real seconds. And each fictional minute is 1 real second.
            float currentFictionalHours = GameTimeHandler.currentGameTimeInSeconds / 60 + FictionalStartingTimeOffsetInHours;
            float currentFictionalMinutes = GameTimeHandler.currentGameTimeInSeconds;

            SetClock(currentFictionalHours, currentFictionalMinutes);

            yield return new WaitForSeconds(1);
        }
    }

    private void SetClock(float hours, float minutes = 0)
    {
        hourHand.localRotation = Quaternion.Euler(-30 * (hours % 12), 0, 0);
        minuteHand.localRotation = Quaternion.Euler(-6 * (minutes % 60), 0, 0);
    }
}
