using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using Random = UnityEngine.Random;
using System.Text;
using System;
using System.IO;

public class PrefabEditorWindow : EditorWindow
{
    public GameObject prefab;

    [MenuItem("Custom Tools/Prefab Modifier Tool")]
    public static void CreateShowcase()
    {
        EditorWindow window = GetWindow<PrefabEditorWindow>("Prefab Modifier");
        Vector2 windowSize = new Vector2(600, 500);
        Vector2 windowPos = new Vector2((Screen.currentResolution.width - windowSize.x) / 2f, (Screen.currentResolution.height - windowSize.y) / 2f);
        window.position = new Rect(windowPos, windowSize);
        window.minSize = new Vector2(600, 500);
    }

    private void OnGUI()
    {
        Vector2 editorSize = new Vector2(EditorWindow.focusedWindow.position.width, EditorWindow.focusedWindow.position.height);

        PrintControls();
        if(prefab != null)
        {
            PrintFields();
        }

        if (GUI.changed)
        {
            SavePrefabData();
        }
    }

    private void PrintFields()
    {
        var components = prefab.GetAllComponents();

        foreach (MonoBehaviour c in components)
        {
            var fields = c.GetAllFieldWithAttribute<PrefabModifiableAttribute>();
            
            foreach (var field in fields)
            {
                Type compType = c.GetType();
                SerializedObject serializedObject = new SerializedObject(prefab.GetComponent(compType));
                SerializedProperty serializedProperty = serializedObject.FindProperty(field.Name);
                EditorGUILayout.PropertyField(serializedProperty);
                serializedObject.ApplyModifiedProperties();
            }
        }
    }

    private void PrintControls()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Load File"))
        {
            Load();
        }
        if (GUILayout.Button("Go To File "))
        {
            GoToFile();
        }
        GUILayout.EndHorizontal();

        prefab = EditorGUILayout.ObjectField("Prefab: ", prefab, typeof(GameObject), false) as GameObject;
    }

    private void SavePrefabData()
    {
        EditorUtility.SetDirty(prefab);
        AssetDatabase.SaveAssets();
    }

    private void Load()
    {
        string folderPath = EditorUtility.OpenFilePanel("Select Prefab", "Resources", "prefab");

        if (string.IsNullOrEmpty(folderPath))
        {
            Debug.Log("File dialog cancelled.");
            return;
        }

        int resourcesIndex = folderPath.IndexOf("Resources");
        string resourcePath = folderPath.Substring(resourcesIndex + "Resources".Length + 1);

        prefab = Resources.Load<GameObject>(resourcePath.Split(".")[0]);
        Debug.Log($"Loaded {resourcePath}");
    }

    private void GoToFile()
    {
        if (prefab != null)
        {
            string cardPath = AssetDatabase.GetAssetPath(prefab);
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(cardPath, typeof(UnityEngine.Object)));
        }
    }
}
