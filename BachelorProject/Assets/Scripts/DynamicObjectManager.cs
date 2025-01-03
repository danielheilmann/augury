using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//TODO: This has been reduced to a mere ID-assigner.
[ExecuteAlways]
public class DynamicObjectManager : MonoBehaviour   //< Only a Monobehaviour for the utility methods at the very bottom
{
    public static Dictionary<string, DynamicObject> dynObjects { get; private set; } = new();

    public static DynamicObject GetByID(string id)
    {
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
        //> We do not need to check for ID overlaps, as the IDs are checked for uniqueness during the initial assignment.
        if (dynObjects.TryAdd(dynObject.id, dynObject))
            return;
        else
            dynObjects[dynObject.id] = dynObject;  //< To relink a dynamic object that had previously been destroyed or was in an inactive scene.
    }

    public static void Unregister(DynamicObject dynObject)
    {
        dynObjects.Remove(dynObject.id);
    }

    public static string GenerateID()
    {
        string id = Guid.NewGuid().ToString();
        while (dynObjects.ContainsKey(id))
            id = Guid.NewGuid().ToString();

        return id;
    }

#if UNITY_EDITOR
    // [ContextMenu("Register all unregistered DynamicObjects in scene")]
    // private void RegisterAllDynamicObjectsInScene()  //< Would require DynamicObject class to also be set to [ExecuteAlways], which would be overkill as the IDs can also be assigned via bulk-select in the editor.
    // {
    //     DynamicObject[] dynObjectsInScene = FindObjectsOfType<DynamicObject>(true);
    //     foreach (DynamicObject dynObj in dynObjectsInScene)
    //     {
    //         if (!dynObj.isRegistered)
    //             dynObj.GetUniqueID();
    //     }
    // }

    [ContextMenu("List all registered DynamicObjects")]
    private void ListAllRegistered()
    {
        Debug.Log($"Now listing all registered DynamicObjects:");
        foreach (var entry in dynObjects)
        {
            DynamicObject dynamicObject = entry.Value;
            Debug.Log($"{dynamicObject.gameObject.name}", dynamicObject);
        }
    }
#endif
}