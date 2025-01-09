using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicObjectManager : MonoBehaviour
{
    public static DynamicObjectManager Instance //< To allow for DynamicObjects to generate unique IDs (in the editor) without having to start the application.
    {
        get
        {
            if (_Instance == null)
                _Instance = FindObjectOfType<DynamicObjectManager>();
            return _Instance;
        }
    }
    private static DynamicObjectManager _Instance;
    public Dictionary<string, DynamicObject> dynObjects { get; private set; } = new();  //< For some reason, this is not session-persistent and gets cleared on every reload

    private void Awake()
    {
        if (_Instance == null)
        {
            DontDestroyOnLoad(this);
            _Instance = this;
        }
        else
        {
            Debug.LogError($"{this} cannot set itself as instance as one has already been set by {_Instance.gameObject}. Deleting self.");
            _Instance.RegisterAllDynamicObjectsInScene();
            Destroy(this);
        }
    }

    private void Start() => RegisterAllDynamicObjectsInScene(); //< To make sure that all dynamic objects in the scene are registered, even ones that are currently inactive. While this cannot account for not-yet-instantiated DOs, these register themselves upon instantiation.

    public static DynamicObject GetByID(string id)
    {
        Dictionary<string, DynamicObject> dynObjects = Instance.dynObjects;

        if (dynObjects.ContainsKey(id))
        {
            DynamicObject dynObject = dynObjects[id];
            if (dynObject == null)
                Debug.LogWarning($"Dynamic object with the ID \"{id}\" is null. This most likely means that it has been destroyed or only exists in another scene.");
            return dynObject;
        }
        else
        {
            Debug.LogWarning($"No dynamic object with the ID \"{id}\" found.");
            return null;
        }
    }
    public static void Register(DynamicObject dynObject)
    {
        Dictionary<string, DynamicObject> dynObjects = Instance.dynObjects;

        //> We do not need to check for ID overlaps, as the IDs are checked for uniqueness during the initial assignment.
        if (dynObjects.TryAdd(dynObject.id, dynObject))
            return;
        else
            dynObjects[dynObject.id] = dynObject;  //< To relink a dynamic object that had previously been destroyed or was in an inactive scene.
    }

    public static void Unregister(DynamicObject dynObject)
    {
        Dictionary<string, DynamicObject> dynObjects = Instance.dynObjects;

        dynObjects.Remove(dynObject.id);
    }

    public static string GenerateUniqueID()
    {
        Dictionary<string, DynamicObject> dynObjects = Instance.dynObjects;

        string id = Guid.NewGuid().ToString();
        while (dynObjects.ContainsKey(id))
            id = Guid.NewGuid().ToString();

        return id;
    }

    [ContextMenu("Register all unregistered DynamicObjects in scene")]
    public void RegisterAllDynamicObjectsInScene()   //< Needs to be public so the CustomInspector can call it.
    {
        DynamicObject[] dynObjectsInScene = FindObjectsOfType<DynamicObject>(true); //< To also find inactive objects in the current scene.

        foreach (DynamicObject dynObj in dynObjectsInScene)
        {
            if (dynObj.hasID)
            {
                if (!dynObjects.ContainsKey(dynObj.id)) //< If the object has an ID already but is not yet registered.
                    Register(dynObj);
                else
                {
                    if (dynObjects[dynObj.id] != dynObj)
                        Debug.LogError($"Another dynamic object with the ID \"{dynObj.id}\" is already registered. Please request a new ID for \"{dynObj.gameObject.name}\".", dynObj.gameObject);
                    // else
                    //     If this object is already registered under this ID, there is no need to do anything.
                }
            }
            else
                Debug.LogWarning($"{dynObj.gameObject.name} does not have an ID assigned yet. Please stop the application and assign an ID to this object using the \"Request new Unique ID\" button.");
        }
    }
}