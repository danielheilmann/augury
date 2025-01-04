using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//? This class is very similar to the GazePointVisualizer. Maybe they should be unified into one class? Or at least derive from a common base class?
public class FixationVisualizer : MonoBehaviour
{
    public static FixationVisualizer Instance { get; private set; }
    [SerializeField] private GameObject prefab;
    [SerializeField] private int startingCapacity = 60; //< Is also used as the increment size for the pool.
    [SerializeField, ReadOnly] private List<FixationVisualization> visualizations;
    private int currentIndex = 0;

    #region Monobehaviour Methods
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Debug.LogError($"{this} cannot set itself as instance as one has already been set by {Instance.gameObject}. Deleting self.");
            Destroy(this);
        }

        if (prefab == null)
            Debug.LogError($"No Prefab has been assigned to variable PointPrefab. Skipping creation of pool.");
    }

    private void OnEnable()
    {
        if (Settings.visualizeInRecordMode)
        {
            SessionManager.OnRecordStart.AddListener(Initialize);
            SessionManager.OnRecordStop.AddListener(DeleteAllVisualizations);
        }

        SessionManager.OnReplayStart.AddListener(Initialize);
        SessionManager.OnReplayStop.AddListener(DeleteAllVisualizations);

        FixationManager.OnFixationCreated.AddListener(Visualize);
    }

    private void OnDisable()
    {
        FixationManager.OnFixationCreated.RemoveListener(Visualize);
    }

    private void OnDestroy()
    {
        SessionManager.OnRecordStart.RemoveListener(Initialize);
        SessionManager.OnRecordStop.RemoveListener(DeleteAllVisualizations);

        SessionManager.OnReplayStart.RemoveListener(Initialize);
        SessionManager.OnReplayStop.RemoveListener(DeleteAllVisualizations);
    }
    #endregion

    private void Initialize()
    {
        if (prefab == null)
        {
            Debug.LogError($"{this} does not contain a valid prefab. Please stop the application and assign a prefab.");
            return;
        }

        DeleteAllVisualizations();

        visualizations = new List<FixationVisualization>(startingCapacity);
        FillPool();

        currentIndex = 0;
    }

    private void DeleteAllVisualizations()
    {
        if (visualizations.Count != 0)   //< If a pool from a previous session exists, discard the entire pool.
            for (int i = visualizations.Count - 1; i >= 0; i--)
                Destroy(visualizations[i].gameObject);
    }

    public void FillPool() //< This setup should ensure that the two lists (pool & FixationVisualizations) are always in sync in terms of indices.
    {
        int emptyCapacity = visualizations.Capacity - visualizations.Count;
        for (int i = 0; i < emptyCapacity; i++)
        {
            GameObject go = Instantiate(prefab);
            visualizations.Add(go.GetComponent<FixationVisualization>()); //< Caches the references to the FixationVisualizations immediately when the pool is created.
            go.transform.SetParent(this.transform);
            go.SetActive(false);
        }
    }

    public void IncreasePoolSize(int amount)
    {
        Debug.Log($"Increasing pool size from {visualizations.Capacity} to {visualizations.Capacity + amount}.");
        visualizations.Capacity += amount;
        FillPool();
    }

    //TODO: Implement a way to adapt the scale of the fixation circle based on the amount of fixations points / fixation duration.
    public void Visualize(Fixation fixation)
    {
        visualizations[currentIndex]
            .Configure(fixation, currentIndex)
            .gameObject.SetActive(true);

        currentIndex++;

        if (currentIndex >= visualizations.Capacity - 1)
            IncreasePoolSize(startingCapacity);
    }

    public FixationVisualization GetFixationVisualization(int index)
    {
        if (index < 0 || index > visualizations.Count - 1)  //< To prevent trying to access out-of-bounds values
        {
            Debug.LogWarning($"Index {index} is out of bounds. Returning null.");
            return null;
        }

        return visualizations[index];
    }
}
