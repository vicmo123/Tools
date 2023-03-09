using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;
using System.IO;
using Unity.VisualScripting;

public static class PrefabGenerator
{
    private static int numberToGenerate = 5;
    private static HashSet<string> usedNames = new HashSet<string>();
    private static List<GameObject> GeneratedPrefabs = new List<GameObject>();
    private static List<Sprite> spriteList = new List<Sprite>();
    private static string spritePath = "EnemySprites/";
    private static string pathOldBaseAi = "Scripts/Old Scripts/OldAiBase/";
    private static string pathNewBaseAi = "Scripts/New Scripts/AITypes/";
    private static string searchPattern = "*.cs";
    private static string scriptablePath = "Assets/Resources/ScriptableAssets/";

    private static string saveLocation = "Resources/Prefabs/";
    [MenuItem("Custom Tools/GeneratePrefabs")]
    public static void GeneratePrefabs()
    {
        usedNames = GetAlreadyUsedNameInFileSystem("Prefabs/");
        spriteList = LoadAllResourcesOfType<Sprite>(spritePath).ToList();
        for (int i = 0; i < numberToGenerate; i++)
        {
            GeneratedPrefabs.Add(GeneratePrefab());
        }

        for (int i = GeneratedPrefabs.Count -1; i >= 0; i--)
        {
            GeneratedPrefabs[i].SaveToFileSystem(saveLocation);
            GeneratedPrefabs[i] = null;
        }

        GeneratedPrefabs.Clear();
    }

    [MenuItem("Custom Tools/RepairPrefab")]
    public static void RepairPrefabs()
    {
        string folderPath = EditorUtility.OpenFolderPanel("Select Folder", "", "");
        string assetPath = folderPath.Replace(Application.dataPath, "Assets").Replace("\\", "/");
        string[] files = Directory.GetFiles(assetPath, "*.prefab", SearchOption.AllDirectories);

        foreach (string file in files)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(file);

            if (prefab != null && prefab.GetComponent<Old_AIBase>() != null)
            {
                try
                {
                    //Instantiate a copy of the Prefab
                    GameObject prefabInstance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                    var oldAiBase = prefabInstance.GetComponent<Old_AIBase>();
                    AIStats stats = oldAiBase.aiStats;
                    EnumReferences.EnemyType enumType = oldAiBase.enemyType;

                    //Replace the component with a new one
                    string newAiString = (oldAiBase.GetType().ToString().Split("_"))[1];
                    prefabInstance.ReplaceComponent(typeof(Old_AIBase), newAiString.GetSystemTypeWithString());
                    var newAiBase = prefabInstance.GetComponent<AIBase>();
                    newAiBase.LinkAiBase(new List<Component> { prefabInstance.GetComponent<Rigidbody2D>(), prefabInstance.GetComponent<BoxCollider2D>(), prefabInstance.GetComponent<SpriteRenderer>() });
                    newAiBase.aiStats = stats;
                    newAiBase.enemyType = enumType;

                    //Save the Prefab
                    PrefabUtility.SaveAsPrefabAsset(prefabInstance, file);
                    AssetDatabase.SaveAssets();
                    Debug.Log($"{prefabInstance.name} was fixed at: {file}");

                    //Destroy the instantiated copy
                    GameObject.DestroyImmediate(prefabInstance, true);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Failed to save prefab {prefab.name}: {ex.Message} : {ex.StackTrace}");
                }
            }
        }
    }

    private static System.Type GetSystemTypeWithString(this string toType)
    {
        Assembly asm = typeof(AIBase).Assembly;
        System.Type newType = asm.GetType(toType);

        return newType;
    }

    private static void ReplaceComponent(this GameObject obj, System.Type oldComponent, System.Type newComponent)
    {
        if (!typeof(Component).IsAssignableFrom(oldComponent))
        {
            Debug.LogError("Error: oldComponent is not a subclass of Component.");
            return;
        }

        if (!typeof(Component).IsAssignableFrom(newComponent))
        {
            Debug.Log(newComponent);
            Debug.LogError("Error: newComponent is not a subclass of Component.");
            return;
        }

        var toDestroy = obj.GetComponent(oldComponent);
        if (toDestroy != null)
        {
            GameObject.DestroyImmediate(toDestroy, true);
            obj.AddComponent(newComponent);
        }
    }

    public static GameObject GeneratePrefab()
    {
        GameObject newPrefab = new GameObject();
        newPrefab.AddComponents(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(SpriteRenderer), GetRandomAI<Old_AIBase>(pathOldBaseAi));
        newPrefab.name = GetRandomName();

        return newPrefab;
    }

    public static void AddComponents(this GameObject obj, params System.Type[] toAddTypes)
    {
        List<Component> toAddList = new List<Component>();

        foreach (var component in toAddTypes)
        {
            var toLink = obj.AddComponent(component);
            //Debug.Log("Added " + toLink.GetType() + " to " + obj.name);
            toAddList.Add(toLink);
        }

        var aiBase = obj.GetComponent<Old_AIBase>();
        aiBase.LinkAiBase(toAddList);
        aiBase.enemyType = GetRandomInEnum<EnumReferences.EnemyType>();
        aiBase.aiStats = GetRandomAiStats();
        obj.GetComponent<SpriteRenderer>().sprite = GetRandomSprite();
    }

    public static void LinkAiBase<T>(this T linkable, List<Component> toLink)
    {
        FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public);
        foreach (FieldInfo field in fields)
        {
            foreach (var item in toLink)
            {
                if (field.FieldType.IsAssignableFrom(item.GetType()))
                {
                    field.SetValue(linkable, item);
                }
            }
        }
    }

    public static T[] LoadAllResourcesOfType<T>(string filePath) where T : UnityEngine.Object
    {
        return Resources.LoadAll<T>(filePath);
    }

    public static Sprite GetRandomSprite()
    {
        return spriteList[Random.Range(0, spriteList.Count)];
    }

    public static System.Type GetRandomAI<T>(string filePath)
    {
        string folderPath = Path.Combine(Application.dataPath, filePath);

        var baseType = typeof(T);
        var randomScriptTypes = GetAllOfTypeFromFolder(folderPath, searchPattern, baseType);

        if (randomScriptTypes != null)
        {
            return randomScriptTypes[Random.Range(0, randomScriptTypes.Count)];
        }
        else
        {
            return null;
        }
    }

    public static List<System.Type> GetAllOfTypeFromFolder(string folderPath, string searchPattern, System.Type baseType)
    {
        List<System.Type> typeList = new List<System.Type>();

        foreach (string filePath in Directory.GetFiles(folderPath, searchPattern))
        {
            string assetPath = filePath.Replace(Application.dataPath, "Assets/").Replace("\\", "/");
            MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);
            if (script != null && baseType.IsAssignableFrom(script.GetClass()))
            {
                typeList.Add(script.GetClass());
            }
        }

        if (typeList.Count > 0)
        {
            return typeList;
        }
        else
        {
            return null;
        }
    }

    public static T GetRandomInEnum<T>() where T : System.Enum
    {
        List<T> enums = System.Enum.GetValues(typeof(T)).Cast<T>().ToList();
        return enums[UnityEngine.Random.Range(0, enums.Count)];
    }

    public static T[] LoadAllScriptableAssets<T>(string folderPath, string searchPattern) where T : UnityEngine.Object
    {
        string[] assetPaths = Directory.GetFiles(folderPath, searchPattern);
        List<T> scriptableList = new List<T>();

        foreach (string assetPath in assetPaths)
        {
            System.Type assetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
            if (assetType == typeof(T))
            {
                T instance = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (instance != null)
                {
                    scriptableList.Add(instance);
                }
            }
        }
        return scriptableList.ToArray();
    }

    public static AIStats GetRandomAiStats()
    {
        var scriptables = LoadAllScriptableAssets<AIStats>(scriptablePath, "*.asset");
        return scriptables[Random.Range(0, scriptables.Length)];
    }

    private static HashSet<string> GetAlreadyUsedNameInFileSystem(string saveLocationResources)
    {
        GameObject[] prefabs = Resources.LoadAll<GameObject>(saveLocationResources);
        HashSet<string> usedNames = new HashSet<string>();

        if(prefabs != null || prefabs.Length > 0)
        {
            for (int i = 0; i < prefabs.Length; i++)
            {
                usedNames.Add(prefabs[i].name);
            }
        }

        return usedNames;
    }

    public static string GetRandomName()
    {
        int numAdj = System.Enum.GetValues(typeof(Adjectives)).Cast<Adjectives>().ToArray().Length;
        int numNoun = System.Enum.GetValues(typeof(Nouns)).Cast<Nouns>().ToArray().Length;
        int tries = numAdj * numNoun;

        HashSet<string> prefabNames = new HashSet<string>();
        if (usedNames != null)
            prefabNames.UnionWith(usedNames);

        if(GeneratedPrefabs.Count > 0 || GeneratedPrefabs != null)
        {
            foreach (GameObject prefab in GeneratedPrefabs)
            {
                prefabNames.Add(prefab.name);
            }
        }
        
        for (int i = 0; i < tries; i++)
        {
            Adjectives adjective = (Adjectives)Random.Range(0, System.Enum.GetValues(typeof(Adjectives)).Length);
            Nouns noun = (Nouns)Random.Range(0, System.Enum.GetValues(typeof(Nouns)).Length);
            string name = adjective.ToString() + " " + noun.ToString();

            if (!prefabNames.Contains(name) || prefabNames.Count <= 0)
            {
                return name;
            }
        }

        Debug.LogError("No valid name was found, please add more words in : (NameWordsCollection.cs)");
        return "No Name";
    }

    private static void SaveToFileSystem(this GameObject objToSave, string saveLocation)
    {
        if (objToSave == null)
        {
            Debug.LogError("The GameObject you are trying to save is null.");
            return;
        }

        if (string.IsNullOrEmpty(saveLocation))
        {
            Debug.LogError("The save location is null or empty.");
            return;
        }

        string directoryPath = Path.Combine(Application.dataPath, saveLocation.Replace("\\", "/"));
        if (!Directory.Exists(directoryPath))
        {
            Debug.LogError($"The directory {directoryPath} does not exist.");
            return;
        }

        string prefabPath = Path.Combine(directoryPath, objToSave.name + ".prefab");
        if (File.Exists(prefabPath))
        {
            if (!EditorUtility.DisplayDialog("Overwrite prefab?", $"A prefab with the name {objToSave.name} already exists in the directory {directoryPath}. Do you want to overwrite it?", "Yes", "No"))
            {
                return;
            }
        }
        try
        {
            EditorUtility.SetDirty(objToSave);
            PrefabUtility.SaveAsPrefabAsset(objToSave, prefabPath);
            AssetDatabase.SaveAssets();
            Debug.Log($"{objToSave.name} was saved at: {prefabPath}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to save prefab {objToSave.name}: {ex.Message}");
        }

        GameObject.DestroyImmediate(objToSave);
    }
}
