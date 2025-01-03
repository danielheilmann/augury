using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicObjectManager : MonoBehaviour
{
    public static DynamicObjectManager Instance
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
            _Instance = this;
        else
        {
            Debug.LogError($"{this} cannot set itself as instance as one has already been set by {_Instance.gameObject}. Deleting self.");
            foreach (var item in this.dynObjects)   //> Transfer all the entries from this manager object onto the already existing singleton instance before deleting self.
                _Instance.dynObjects.TryAdd(item.Key, item.Value);
            Destroy(this);
        }
    }

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
    public void RegisterAllDynamicObjectsInScene()
    {
        DynamicObject[] dynObjectsInScene = FindObjectsOfType<DynamicObject>(true);
        foreach (DynamicObject dynObj in dynObjectsInScene)
        {
            if (dynObj.hasID)
            {
                if (!dynObjects.ContainsKey(dynObj.id)) //< If the object has an ID already but is not yet registered.
                    Register(dynObj);
            }
            else
                dynObj.RequestNewID();
        }
    }
}