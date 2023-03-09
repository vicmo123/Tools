using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;
using System.IO;

public static class PrefabGenerator
{

    [MenuItem("Custom Tools/GeneratePrefabs")] //This the function below it as a menu item, which appears in the tool bar
    public static void GeneratePrefabs() //Menu items can call STATIC functions, does not work for non-static since Editor scripts cannot be attached to objects
    {
        spriteList = GetSpritesInAssets(spritePath).ToList();
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

    private static int numberToGenerate = 5;
    private static List<GameObject> GeneratedPrefabs = new List<GameObject>();
    private static List<Sprite> spriteList = new List<Sprite>();
    private static string spritePath = "EnemySprites/";
    private static string pathOldBaseAi = "Scripts/Old Scripts/OldAiBase/";
    private static string searchPattern = "*.cs";
    private static string scriptablePath = "Assets/Resources/ScriptableAssets/";
    private static string saveLocation = "Resources/Prefabs/";


    public static GameObject GeneratePrefab()
    {
        GameObject newPrefab = new GameObject();
        newPrefab.AddComponents(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(SpriteRenderer), GetRandomAI<Old_AIBase>());
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

    public static Sprite[] GetSpritesInAssets(string filePath)
    {
        return Resources.LoadAll<Sprite>(filePath);
    }

    public static Sprite GetRandomSprite()
    {
        return spriteList[Random.Range(0, spriteList.Count)];
    }

    public static System.Type GetRandomAI<T>()
    {
        string folderPath = Path.Combine(Application.dataPath, pathOldBaseAi);

        var baseType = typeof(T);
        var randomScriptType = GetRandomTypeFromFolder(folderPath, searchPattern, baseType);

        if (randomScriptType != null)
        {
            return randomScriptType;
        }
        else
        {
            return null;
        }
    }

    public static System.Type GetRandomTypeFromFolder(string folderPath, string searchPattern, System.Type baseType)
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
            return typeList[UnityEngine.Random.Range(0, typeList.Count)];
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
            string fullPath = Path.Combine(Application.dataPath, assetPath.Replace("\\", "/"));
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
        string folderPath = Path.Combine(Application.dataPath, scriptablePath);
        var scriptables = LoadAllScriptableAssets<AIStats>(scriptablePath, "*.asset");
        return scriptables[Random.Range(0, scriptables.Length)];
    }

    public static string GetRandomName()
    {
        int tries = 50;
        string[] adjectives = { "Happy", "Angry", "Brave", "Clever", "Drunk", "Energetic", "Friendly", "Gentle", "Helpful", "Intelligent", "Joyful", "Kind", "Loyal", "Mighty", "Nimble", "Optimistic", "Proud", "Quiet", "Rugged", "Silly", "Talented", "Unique", "Vibrant", "Wise", "Zealous" };
        string[] nouns = { "Cat", "Dog", "Horse", "Bird", "Fish", "Lion", "Tiger", "Bear", "Wolf", "Fox", "Elephant", "Giraffe", "Monkey", "Snake", "Crocodile", "Spider", "Dragon", "Robot", "Ninja", "Pirate", "Knight", "Wizard", "Alien", "Vampire", "Zombie", "Bob" };

        HashSet<string> prefabNames = new HashSet<string>();
        if(GeneratedPrefabs.Count > 0)
        {
            foreach (GameObject prefab in GeneratedPrefabs)
            {
                prefabNames.Add(prefab.name);
            }
        }
        else
        {
            string adjective = adjectives[Random.Range(0, adjectives.Length)];
            string noun = nouns[Random.Range(0, nouns.Length)];
            string name = adjective + " " + noun;
            return name;
        }
        

        for (int i = 0; i < tries; i++)
        {
            string adjective = adjectives[Random.Range(0, adjectives.Length)];
            string noun = nouns[Random.Range(0, nouns.Length)];
            string name = adjective + " " + noun;

            if (!prefabNames.Contains(name))
            {
                return name;
            }
        }

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
