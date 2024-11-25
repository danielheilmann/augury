using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    public static Timer Instance { get; private set; }
    public static UnityEvent OnTick = new();
    public static float ticksPerSecond = 10;

    private float waitTime;
    private Coroutine coroutine;

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
        waitTime = 1 / ticksPerSecond;
        coroutine = StartCoroutine(Tick()); //< Having this in OnEnable() allows modification of tick rate during runtime. After overwriting the tick rate, simply disable and reenable this component.
    }

    private void OnDisable()
    {
        StopCoroutine(coroutine);
    }

    private IEnumerator Tick()
    {
        while (Application.isPlaying)
        {
            // Debug.Log($"Emitted Tick Signal");
            OnTick.Invoke();
            yield return new WaitForSeconds(waitTime);
        }
    }
}