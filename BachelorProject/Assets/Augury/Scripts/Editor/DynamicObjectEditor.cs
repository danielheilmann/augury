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
                if (GUILayout.Button("Request new Unique ID"))
                    confirmAmount += 1;

            if (confirmAmount == 1)
                if (GUILayout.Button("You sure?"))
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
