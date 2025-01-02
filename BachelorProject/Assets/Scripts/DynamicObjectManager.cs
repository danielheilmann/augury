using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: This has been reduced to a mere ID-assigner.
public static class DynamicObjectManager
{
    public static Dictionary<int, DynamicObject> dynObjects {get; private set;} = new Dictionary<int, DynamicObject>();
    public static void Register(DynamicObject dynObject)
    {
        int dynObjID = dynObject.id;
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