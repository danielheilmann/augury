using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: I should probably rework this entire thing, removing the unnecessary ObjectPool could allow streamlining the Fixations, which, right now just feel overcomplicated.
public class FixationVisualizer : MonoBehaviour
{
    private const float offsetFactor = 0.01f;   //< During placement, the visualizer canvas is lifted from the surface by an amount equal to surfaceNormal * offsetFactor.
    public static FixationVisualizer Instance { get; private set; }
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
        FixationManager.OnFixationCreated.AddListener(VisualizeAt);
    }

    private void OnDisable()
    {
        FixationManager.OnFixationCreated.RemoveListener(VisualizeAt);
    }

    void Start()
    {
        if (prefab == null)
            return;

        for (int i = 0; i < poolSize; i++)
            IncreasePool();
    }

    public void IncreasePool()
    {
        GameObject go = Instantiate(prefab);
        pool.Add(go);
        go.transform.SetParent(this.transform);
        go.SetActive(false);
    }

    public void VisualizeAt(Vector3 position, int fixationNumber, Vector3 surfaceNormal, DynamicObject dynObj)
    {
        if (!this.gameObject.activeInHierarchy | prefab == null)
            return;

        ConfigureFixationGO(go: pool[currentIndex], position: position, surfaceNormal:surfaceNormal, id: fixationNumber, precedingFixation: GetPrecedingFixationFromList(currentIndex), dynObj: dynObj);

        if (currentIndex < poolSize - 1) currentIndex++;
        else currentIndex = 0;
    }

    //TODO: Implement a way to adapt the scale of the fixation circle based on the amount of fixations points / fixation duration.
    private void ConfigureFixationGO(GameObject go, Vector3 position, Vector3 surfaceNormal, int id, Fixation precedingFixation, DynamicObject dynObj)
    {
        go.SetActive(false);
        go.transform.position = position + surfaceNormal * offsetFactor;    //< surfaceNormal is added here to prevent Z-Fighting
        go.GetComponentInChildren<Canvas>().transform.LookAt(position - surfaceNormal);
        go.transform.SetParent(dynObj == null ? this.transform : dynObj.transform, true);
        go.name = $"{id} {position}";
        Fixation fixation = go.GetComponent<Fixation>();
        fixation.SetID(id);
        fixation.SetDynamicObject(dynObj);
        fixation.ConnectToPrecedingFixation(precedingFixation);
        go.SetActive(true);
    }

    //TODO: This can be simplified as the fixation can just check whether it is #1, in which case it will just not try to connect to the "preceding" fixation, as there is none.
    private Fixation GetPrecedingFixationFromList(int currentIndex)
    {
        //< If currentIndex is 0, the previous fixation would be the last one in the list)
        int previousIndex;
        if (currentIndex == 0) previousIndex = pool.Count - 1;
        else previousIndex = currentIndex - 1;

        Fixation precedingFixation = pool[previousIndex].GetComponent<Fixation>();

        if (precedingFixation.isActiveAndEnabled)
            return precedingFixation;
        else return null;
    }
}
