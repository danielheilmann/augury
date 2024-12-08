using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationVisualizer : MonoBehaviour
{
    public static LocationVisualizer Instance { get; private set; }
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
        GazePointManager.OnPointCreated.AddListener(VisualizePoint);
    }

    private void OnDisable()
    {
        GazePointManager.OnPointCreated.RemoveListener(VisualizePoint);
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

    public void VisualizePoint(GazePoint point)
    {
        if (!this.gameObject.activeInHierarchy | prefab == null)    //< The activeInHierarchy check should not be necessary considering that the even is unsubscribed from OnDisable().
            return;

        GameObject availableGO = pool[currentIndex];

        if (point.attachedToDynObj)
            VisualizeLocal(availableGO, point.attachedToDynObj.transform, point.position);
        else
            VisualizeAt(availableGO, point.position);

        if (currentIndex < poolSize - 1) currentIndex++;
        else currentIndex = 0;
    }

    private void VisualizeAt(GameObject visualizationGO, Vector3 position)
    {
        visualizationGO.transform.SetParent(this.transform);
        visualizationGO.transform.localPosition = position;
        visualizationGO.name = position.ToString();
        visualizationGO.SetActive(true);
    }

    private void VisualizeLocal(GameObject visualizationGO, Transform parentToAttachTo, Vector3 position)
    {
        visualizationGO.transform.SetParent(parentToAttachTo);
        visualizationGO.transform.localPosition = position;
        visualizationGO.name = parentToAttachTo.name + " " + position.ToString();
        visualizationGO.SetActive(true);
    }
}
