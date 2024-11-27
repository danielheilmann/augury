using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixationVisualizer : MonoBehaviour
{
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

    public void VisualizeAt(Vector3 position, int fixationNumber)
    {
        if (!this.gameObject.activeInHierarchy | prefab == null)
            return;

        ConfigureFixationGO(go: pool[currentIndex], position: position, id: fixationNumber, precedingFixation: GetPrecedingFixationFromList(currentIndex));

        if (currentIndex < poolSize - 1) currentIndex++;
        else currentIndex = 0;
    }

    //TODO: Implement a way to adapt the scale of the fixation circle based on the amount of fixations points / fixation duration.
    private void ConfigureFixationGO(GameObject go, Vector3 position, int id, Fixation precedingFixation)
    {
        go.SetActive(false);
        go.transform.position = position;
        go.name = $"{id} {position}";
        Fixation fixation = go.GetComponent<Fixation>();
        fixation.SetID(id);
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

        if (precedingFixation.isActiveAndEnabled)   //TODO: This does not fix the line-looping yet, as after going through the initial pool - nevermind
            return precedingFixation;
        else return null;
    }
}
