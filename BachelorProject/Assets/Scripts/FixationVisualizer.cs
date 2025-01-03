using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Instead of a Reuse-ObjectPool, this class should use the ObjectPool as a pregenerated cache to reduce frame overhead from constant instantiation. This one should expand automatically whenever it is full. Though maybe the cache isn't even necessary because fixations are not generated that often?
public class FixationVisualizer : MonoBehaviour
{
    public static FixationVisualizer Instance { get; private set; }
    [SerializeField] private GameObject prefab;
    [SerializeField] private int poolSize = 60; //< Determines how many datapoints will be visible at the same time / concurrently
    [SerializeField, ReadOnly] private List<GameObject> pool;
    [SerializeField, ReadOnly] private List<FixationVisualization> visualizations; //< Could be converted into Dictionary<Fixation, Visualizer> if I do need the search capability later.
    private int currentIndex = 0;

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
        SessionManager.OnRecordStart.AddListener(Initialize);
        SessionManager.OnReplayStart.AddListener(Initialize);

        SessionManager.OnRecordStop.AddListener(DeleteAllVisualizations);
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
        SessionManager.OnReplayStart.RemoveListener(Initialize);

        SessionManager.OnRecordStop.RemoveListener(DeleteAllVisualizations);
        SessionManager.OnReplayStop.RemoveListener(DeleteAllVisualizations);
    }

    private void Initialize()
    {
        if (prefab == null)
        {
            Debug.LogError($"{this} does not contain a valid prefab. Please stop the application and assign a prefab.");
            return;
        }

        DeleteAllVisualizations();

        visualizations = new List<FixationVisualization>(poolSize);
        pool = new List<GameObject>(poolSize);
        for (int i = 0; i < poolSize; i++)
            IncreasePool();

        currentIndex = 0;
    }

    private void DeleteAllVisualizations()
    {
        if (pool.Count != 0)   //< If a pool from a previous session exists, discard the entire pool.
            for (int i = pool.Count - 1; i >= 0; i--)
                Destroy(pool[i]);
    }

    public void IncreasePool() // This setup should ensure that the two lists (pool & FixationVisualizations) should be in sync in terms of indeces.
    {
        GameObject go = Instantiate(prefab);
        pool.Add(go);
        visualizations.Add(go.GetComponent<FixationVisualization>()); //< Caches the references to the FixationVisualizations immediately when the pool is created.
        go.transform.SetParent(this.transform);
        go.SetActive(false);
    }

    public void Visualize(Fixation fixation)
    {
        if (!this.gameObject.activeInHierarchy) //< Allows for the pool to be deactivated by disabling it in the hierarchy
            return;

        ConfigureVisualization(GetFixationVisualization(currentIndex), fixation);

        if (currentIndex < poolSize - 1) currentIndex++;
        else currentIndex = 0;  //< To loop back and overwrite old entries once the pool is full
    }

    //TODO: Implement a way to adapt the scale of the fixation circle based on the amount of fixations points / fixation duration.
    private void ConfigureVisualization(FixationVisualization visualization, Fixation fixation)
    {
        GameObject visualizationGO = visualization.gameObject;

        visualizationGO.SetActive(false);
        visualization.Configure(fixation, currentIndex);
        visualizationGO.SetActive(true);
        // visualization.SetVisible(false);
    }

    //**~> Is already implemented to not use the pool.
    public FixationVisualization GetFixationVisualization(int index)
    {
        //> Looping around index to work with the pool 
        //TODO: This does not account for disabled & overwritten objects in the pool yet
        if (index < 0) index = pool.Count - 1;
        else if (index > pool.Count - 1) index %= poolSize;
        //> Alternative:
        // if (index <= 0 || index >= fixationVisualizations.Count - 1) return null; //< To prevent trying to access out-of-bounds values

        // if (!fixationVisualizations[index].isActiveAndEnabled) Debug.LogWarning($"Fetched disabled visualization do not proceed.");
        // if (fixationVisualizations[index] == null) Debug.LogWarning($"Fetched null visualization do not proceed.");
        return visualizations[index];
    }
}
