using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopScreens : MonoBehaviour
{
    [SerializeField] private List<GameObject> screens;
    [SerializeField] private float displayDurationInSeconds = 5;
    private Coroutine coroutine;

    private void OnEnable()
    {
        coroutine = StartCoroutine(LoopScreensCoroutine());
    }

    private void OnDisable()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
    }

    private IEnumerator LoopScreensCoroutine()
    {
        foreach (var screen in screens)
            screen.SetActive(false); //< Ensure all screens are off before starting the loop.

        while (true)
        {
            foreach (var screen in screens)
            {
                screen.SetActive(true);
                yield return new WaitForSeconds(displayDurationInSeconds);
                screen.SetActive(false);
            }
        }
    }
}
