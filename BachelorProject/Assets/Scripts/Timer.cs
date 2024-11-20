using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    public static Timer Instance { get; private set; }
    public static UnityEvent OnTick = new();
    [SerializeField] private float ticksPerSecond;
    private float waitTime;
    private Coroutine coroutine;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
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
        coroutine = StartCoroutine(Tick());
        // InvokeRepeating("Tick", 0f, waitTime);
    }

    private void OnDisable()
    {
        StopCoroutine(coroutine);
    }

    // public void Tick()
    // {
    //     OnTick.Invoke();
    // }

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