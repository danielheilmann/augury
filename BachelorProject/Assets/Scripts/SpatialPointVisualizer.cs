using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpatialPointVisualizer : MonoBehaviour
{
    public static SpatialPointVisualizer Instance { get; private set; }
    [SerializeField] private GameObject prefab;
    [SerializeField] private int poolSize = 60; //< Determines how many datapoints will be visible at the same time / concurrently
    [SerializeField] private List<GameObject> pool = new List<GameObject>();
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
        SpatialPointManager.OnPointCreated.AddListener(VisualizeAt);
    }

    private void OnDisable()
    {
        SpatialPointManager.OnPointCreated.RemoveListener(VisualizeAt);
    }

    void Start()
    {
        if (prefab == null)
            return;

        for (int i = 0; i < poolSize; i++)
        {
            IncreasePool();
        }
    }

    public void IncreasePool()
    {
        GameObject go = Instantiate(prefab);
        pool.Add(go);
        go.transform.SetParent(this.transform);
        go.SetActive(false);
    }

    public void VisualizePoint(SpatialPoint point)
    {
        VisualizeAt(point.position);
    }

    public void VisualizeAt(Vector3 position)
    {
        if (!this.gameObject.activeInHierarchy | prefab == null)
            return;

        GameObject availableGO = pool[currentIndex];

        availableGO.transform.position = position;
        availableGO.name = position.ToString();
        availableGO.SetActive(true);

        if (currentIndex < poolSize - 1) currentIndex++;
        else currentIndex = 0;
    }
}
