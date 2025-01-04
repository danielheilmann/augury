using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DynamicObjectManager))]
public class DynamicObjectManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var handler = target as DynamicObjectManager;
        if (handler != null)
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Registered Dynamic Objects:", EditorStyles.boldLabel);
            if (handler.dynObjects != null && handler.dynObjects.Count > 0)
            {
                foreach (var obj in handler.dynObjects)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUI.enabled = false;
                    EditorGUILayout.ObjectField(obj.Value.gameObject, typeof(GameObject), true);
                    GUI.enabled = true;
                    EditorGUILayout.LabelField(obj.Key);
                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                EditorGUILayout.LabelField("No objects registered");
            }

            EditorGUILayout.Space(10);
            GUIContent buttonContent = new GUIContent("Register all DynamicObjects in this scene", "Automatically finds and registers all DynamicObjects in the current scene. Only assigns new IDs to DynamicObjects without IDs. Respects already existing IDs.");
            if (GUILayout.Button(buttonContent))
                handler.RegisterAllDynamicObjectsInScene();
        }

    }
}
