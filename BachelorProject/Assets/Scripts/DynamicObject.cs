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
    private void Awake() => UpdatePropertyValues();
    private void OnValidate() => UpdatePropertyValues();

    private void OnEnable()
    {
        Timer.OnTick.AddListener(OnTimerTick);
        SessionManager.OnRecordStart.AddListener(OnSessionStart);
        SessionManager.OnRecordStop.AddListener(OnSessionStop);
    }

    private void OnDisable()
    {
        Timer.OnTick.RemoveListener(OnTimerTick);
        //!> Should still listen to the SessionStart and -Stop events, even if it is inactive, so the following two lines are supposed to be "missing".
        // SessionManager.OnRecordStart.RemoveListener(OnSessionStart);
        // SessionManager.OnRecordStop.RemoveListener(OnSessionStop); 
    }

    private void Start() => DynamicObjectManager.Register(this);
    private void OnDestroy()
    {
        DynamicObjectManager.Unregister(this);

        SessionManager.OnRecordStart.RemoveListener(OnSessionStart);  //< To prevent null exceptions
        SessionManager.OnRecordStop.RemoveListener(OnSessionStop);  //< To prevent null exceptions

        if (SessionManager.currentMode == SessionManager.DataMode.Record)  //< This handles the situation where the DynamicObject is deleted (e.g. as part of the game mechanics) while a recording session is still ongoing. AKA [Case "destroyed during ongoing record session"]
            OnSessionStop();
    }
    #endregion

    private void OnSessionStart() => Initialize();
    private void OnSessionStop() => FileSystemHandler.SaveDynamicObject(this);

    private void Initialize()
    {
        //> Reset
        positionHistory = new();
        rotationHistory = new();
        scaleHistory = new();

        //> Reinitialize
        DateTime timestamp = DateTime.Now;  //< This is executed before the Timer ticks for the first time, so I have to call DateTime.Now here to prevent the first entry from being at DateTime.MinValue.
        
        //> For one, these indicate the starting position of the DynamicObject. But there is another benefit: Without these first entries, I would have to check against null on every call of OnTimerTick, just because there would be a null exception the first time OnTimerTick is executed.
        positionHistory.Add(timestamp, transform.localPosition);
        rotationHistory.Add(timestamp, transform.localRotation);
        scaleHistory.Add(timestamp, transform.localScale);
    }

    private void OnTimerTick()
    {
        DateTime timestamp = Timer.latestTimestamp;
        if (transform.localPosition != positionHistory.ElementAt(positionHistory.Count - 1).Value)
            positionHistory.Add(timestamp, transform.localPosition);
        if (transform.localRotation != rotationHistory.ElementAt(rotationHistory.Count - 1).Value)
            rotationHistory.Add(timestamp, transform.localRotation);
        if (transform.localScale != scaleHistory.ElementAt(scaleHistory.Count - 1).Value)
            scaleHistory.Add(timestamp, transform.localScale);
    }

    private void UpdatePropertyValues()
    {
        string meshName = GetComponent<MeshFilter>().sharedMesh.name;
        id = meshName.GetHashCode();
    }
}
