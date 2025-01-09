using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DynamicObject))]
public class DynamicObjectEditor : Editor
{
    int confirmAmount = 0;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var handler = target as DynamicObject;
        if (handler != null)
        {
            if (confirmAmount == 0)
                if (GUILayout.Button("Request Unique ID"))
                {
                    if (handler.hasID)
                        confirmAmount += 1;
                    else
                        handler.RequestNewID(); //< If the object does not have an ID, it can be requested immediately without confirmation.
                }

            if (confirmAmount == 1)
                if (GUILayout.Button("Do you really want to overwrite the existing ID?"))
                    confirmAmount += 1;

            if (confirmAmount == 2)
            {
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Yes"))
                {
                    handler.RequestNewID();
                    confirmAmount = 0;
                }
                if (GUILayout.Button("No"))
                    confirmAmount = 0;

                GUILayout.EndHorizontal();
            }

        }

    }
}
