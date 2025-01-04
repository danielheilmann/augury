using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazePointVisualizer : MonoBehaviour
{
    public static GazePointVisualizer Instance { get; private set; }
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
        if (Settings.visualizeInRecordMode)
        {
            SessionManager.OnRecordStart.AddListener(Initialize);
            SessionManager.OnRecordStop.AddListener(DeleteAllVisualizations);
        }

        SessionManager.OnReplayStart.AddListener(Initialize);
        SessionManager.OnReplayStop.AddListener(DeleteAllVisualizations);

        GazePointManager.OnPointCreated.AddListener(VisualizePoint);
    }

    private void OnDisable()
    {
        GazePointManager.OnPointCreated.RemoveListener(VisualizePoint);
    }

    private void OnDestroy()
    {
        SessionManager.OnRecordStart.RemoveListener(Initialize);
        SessionManager.OnRecordStop.RemoveListener(DeleteAllVisualizations);

        SessionManager.OnReplayStart.RemoveListener(Initialize);
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

        GameObject visualizerGO = pool[currentIndex];
        Transform goParent = point.isLocal ? point.dynamicObject.transform : this.transform;
        string goName = $"GazePoint {point.localPosition}";

        visualizerGO.transform.SetParent(null);
        visualizerGO.transform.position = point.globalPosition;
        visualizerGO.transform.localScale = Vector3.one;    //< To reset scale in case a dynamic object was scaled after this point was attached (which leads to the point itself being scaled as well).
        visualizerGO.transform.SetParent(goParent, true);
        visualizerGO.name = goName;
        visualizerGO.GetComponentInChildren<Renderer>().material.SetColor("_Color", point.isLocal ? new Color(0.243f, 0.231f, 0.961f) : new Color(1f, 0.271f, 0.569f));
        visualizerGO.SetActive(true);

        if (currentIndex < poolSize - 1) currentIndex++;
        else currentIndex = 0;
    }
}
