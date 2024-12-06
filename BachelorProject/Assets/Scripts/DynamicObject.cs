using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class DynamicObjectManager
{
    [SerializeField, ReadOnly] public static Dictionary<int, DynamicObject> dynObjects = new Dictionary<int, DynamicObject>();    //< Should also be turned into a (public get, private set) field to limit access
    public static void Register(DynamicObject dynObject)
    {
        int dynObjID = dynObject.meshName.GetHashCode();    //TODO: Decouple ID from MeshName, as this is likely to lead to a lot of overlaps in an actual application!
        if (dynObjects.TryAdd(dynObjID, dynObject))
            dynObject.id = dynObjID;
        else
            Debug.LogWarning($"There is already a dynamic object with the ID \"{dynObjID}\".");
    }

    public static void Unregister(DynamicObject dynObject)
    {
        FileSystemHandler.SaveDynamicObjectToFile(fileTitle: $"Session {SessionManager.sessionStartTime.ToString("yyyy-MM-dd HH-mm-ss")} - DynObj_{dynObject.id}", dynObject);
        dynObjects.Remove(dynObject.id);
    }
}

public class DynamicObject : MonoBehaviour
{
    [SerializeField, ReadOnly] public int id; //{ get; private set; } //< Currently commented out so it's visible in the inspector
    [SerializeField, ReadOnly] public string meshName;
    [SerializeField, ReadOnly] public Dictionary<DateTime, Vector3> positionHistory = new();
    [SerializeField, ReadOnly] public Dictionary<DateTime, Quaternion> rotationHistory = new();
    [SerializeField, ReadOnly] public Dictionary<DateTime, Vector3> scaleHistory = new();

    #region Monobehaviour Methods
    private void Awake()
    {
        UpdatePropertyValues();
    }

    private void OnValidate()
    {
        UpdatePropertyValues();
    }

    private void OnEnable()
    {
        DynamicObjectManager.Register(this);
        Timer.OnTick.AddListener(OnTimerTick);  //< Could maybe be optimized by the DynObjManager calling this on every DynObj instead? Kind of like a controlling head?
    }

    private void Start()
    {
        Initialize();
    }

    private void OnDisable()
    {
        Timer.OnTick.RemoveListener(OnTimerTick);
    }

    private void OnDestroy()
    {
        DynamicObjectManager.Unregister(this);
    }
    #endregion

    private void Initialize()
    {
        DateTime timestamp = DateTime.Now;
        positionHistory.Add(timestamp, transform.localPosition);
        rotationHistory.Add(timestamp, transform.localRotation);
        scaleHistory.Add(timestamp, transform.localScale);
    }

    private void UpdatePropertyValues()
    {
        meshName = GetComponent<MeshFilter>().sharedMesh.name;
        id = meshName.GetHashCode();    //< This might be updated by the DynamicObjectManager if there is an overlap in the IDs. But by it being set here already, you can at least see the preliminary ID in the editor before pressing play.
    }

    private void OnTimerTick()
    {
        DateTime timestamp = DateTime.Now;
        if (positionHistory.Count == 0 || transform.localPosition != positionHistory.ElementAt(positionHistory.Count - 1).Value)
            positionHistory.Add(timestamp, transform.localPosition);
        if (rotationHistory.Count == 0 || transform.localRotation != rotationHistory.ElementAt(rotationHistory.Count - 1).Value)
            rotationHistory.Add(timestamp, transform.localRotation);
        if (scaleHistory.Count == 0 || transform.localScale != scaleHistory.ElementAt(scaleHistory.Count - 1).Value)
            scaleHistory.Add(timestamp, transform.localScale);
    }
}
