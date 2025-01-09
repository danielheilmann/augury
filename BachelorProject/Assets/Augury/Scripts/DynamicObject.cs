using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class DynamicObject : MonoBehaviour
{
    public UnityEvent OnPositionUpdate { get; private set; } = new();
    public UnityEvent OnRotationUpdate { get; private set; } = new();
    public UnityEvent OnScaleUpdate { get; private set; } = new();
    [SerializeField] public string id; //{ get; private set; } //< Currently commented out the set;get; so the variable is visible in the inspector
    public bool hasID => !string.IsNullOrEmpty(id);
    [SerializeField, ReadOnly] public Dictionary<DateTime, Vector3> positionHistory = new();
    [SerializeField, ReadOnly] public Dictionary<DateTime, Quaternion> rotationHistory = new();
    [SerializeField, ReadOnly] public Dictionary<DateTime, Vector3> scaleHistory = new();

    //> Rigidbody Data & References
    Rigidbody rb;
    private bool wasKinematicByDefault = false; //< To store the default state of the Rigidbody to revert to after replaying.

    private void Awake()
    {
        rb = GetComponentInChildren<Rigidbody>();
    }

    private void Start()
    {
        if (hasID)
            DynamicObjectManager.Register(this); //< To re-register in case this object was deleted or generally instantiated after the DynamicObjectManager ran its initial scan on Start().
        else
            Debug.LogError($"{this.gameObject.name} does not have an ID assigned yet. Please stop the application and assign an ID to this object using the \"Request new Unique ID\" button.");
    }

    private void OnEnable()
    {
        Timer.OnTick.AddListener(OnTimerTick);
        SessionManager.OnRecordStart.AddListener(OnRecordSessionStart);
        SessionManager.OnRecordStop.AddListener(OnRecordSessionStop);

        SessionManager.OnReplayStart.AddListener(OnReplaySessionStart);
        SessionManager.OnReplayStop.AddListener(OnReplaySessionStop);

        //> Just in case for if this object had been disabled (or not instantiated yet) when the recording session was started.
        if (SessionManager.currentMode == SessionManager.DataMode.Record)
            OnRecordSessionStart();
    }

    private void OnDisable()
    {
        Timer.OnTick.RemoveListener(OnTimerTick);
        //!> Should still listen to the SessionStart and -Stop events, even if it is inactive, so the following two lines are supposed to be "missing".
        // SessionManager.OnRecordStart.RemoveListener(OnSessionStart);
        // SessionManager.OnRecordStop.RemoveListener(OnSessionStop); 
    }

    private void OnDestroy()
    {
        //> To prevent null exceptions
        SessionManager.OnRecordStart.RemoveListener(OnRecordSessionStart);
        SessionManager.OnRecordStop.RemoveListener(OnRecordSessionStop);

        SessionManager.OnReplayStart.RemoveListener(OnReplaySessionStart);
        SessionManager.OnReplayStop.RemoveListener(OnReplaySessionStop);

        //> If this object is deleted (e.g. as part of the game mechanics) while a recording session is still in progress, save to file immediately
        if (SessionManager.currentMode == SessionManager.DataMode.Record)
            OnRecordSessionStop();
    }

    #region Methods to be called during Recording
    private void OnRecordSessionStart() => Initialize();
    private void OnRecordSessionStop() => FileSystemHandler.SaveDynamicObject(this);

    private void Initialize()
    {
        //> Reset
        positionHistory = new();
        rotationHistory = new();
        scaleHistory = new();

        //> Reinitialize
        DateTime timestamp = DateTime.Now;  //< This part of the code is usually executed before the Timer ticks for the first time, so I have to call DateTime.Now here to prevent the first entry from being at DateTime.MinValue.

        //> For one, these indicate the starting position of the DynamicObject. But there is another benefit: Without these first entries, I would have to check against null on every call of OnTimerTick, just because there would be a null exception the first time OnTimerTick is executed.
        positionHistory.Add(timestamp, transform.localPosition);
        rotationHistory.Add(timestamp, transform.localRotation);
        scaleHistory.Add(timestamp, transform.localScale);
    }

    private void OnTimerTick()
    {
        DateTime timestamp = Timer.latestTimestamp;
        if (transform.localPosition != positionHistory.ElementAt(positionHistory.Count - 1).Value)
        {
            positionHistory.Add(timestamp, transform.localPosition);
            OnPositionUpdate.Invoke();
        }
        if (transform.localRotation != rotationHistory.ElementAt(rotationHistory.Count - 1).Value)
        {
            rotationHistory.Add(timestamp, transform.localRotation);
            OnRotationUpdate.Invoke();
        }
        if (transform.localScale != scaleHistory.ElementAt(scaleHistory.Count - 1).Value)
        {
            scaleHistory.Add(timestamp, transform.localScale);
            OnScaleUpdate.Invoke();
        }
    }
    #endregion

    #region Methods to be called during Replay
    private void OnReplaySessionStart()
    {
        if (rb == null) return; //< Which is the case for dynamic objects without a Rigidbody. (e.g. a Lazy Follower)
        wasKinematicByDefault = rb.isKinematic;
        rb.isKinematic = true; //< To prevent physics from interfering with the replay movement.
    }
    private void OnReplaySessionStop()
    {
        if (rb == null) return;
        rb.isKinematic = wasKinematicByDefault;
    }

    public void OverwritePosition(Vector3 position)
    {
        transform.position = position;
        OnPositionUpdate.Invoke();
    }

    public void OverwriteRotation(Quaternion rotation)
    {
        transform.rotation = rotation;
        OnRotationUpdate.Invoke();
    }

    public void OverwriteScale(Vector3 scale)
    {
        transform.localScale = scale;
        OnScaleUpdate.Invoke();
    }
    #endregion

    [ContextMenu("Request new Unique ID")]
    public void RequestNewID()
    {
        if (hasID)
            DynamicObjectManager.Unregister(this);  //< Unregister old ID before getting a new one.

        id = DynamicObjectManager.GenerateUniqueID();
        DynamicObjectManager.Register(this);
    }

    [ContextMenu("Clear ID")]
    public void Unregister()
    {
        DynamicObjectManager.Unregister(this);
        id = string.Empty;
    }
}
