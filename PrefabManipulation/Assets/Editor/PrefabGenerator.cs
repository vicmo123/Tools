using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;

public static class PrefabGenerator
{

    [MenuItem("Custom Tools/GeneratePrefabs")] //This the function below it as a menu item, which appears in the tool bar
    public static void GeneratePrefabs() //Menu items can call STATIC functions, does not work for non-static since Editor scripts cannot be attached to objects
    {
        for (int i = 0; i < numberToGenerate; i++)
        {
            GeneratePrefab();
        }
    }

    private static int numberToGenerate = 1;

    /*
    1) Create a menu item that generates 20 enemy prefabs and saves them to the project
            a) The enemy prefab contains the following script: BoxCollider, Rigidbody2D, SpriteRenderer, Transform and one randomly selected Old_AIBase child
            b) Set the enum in the Old_AIBase to a random enum
            c) Link one random scriptable object to it
            d) The enemy AI script should have all elements properly linked
            e) Have a collection of words, and randomly generate a name by mixing 2 of the words
            f) Save the prefab to the project with a randomy generated and refresh the asset database
    */

    public static GameObject GeneratePrefab()
    {
        GameObject newPrefab = new GameObject();
        newPrefab.AddComponents(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(SpriteRenderer), typeof(Old_AIBase));
        return null;
    }

    public static void AddComponents(this GameObject obj, params System.Type[] toAdd)
    {
        foreach (var component in toAdd)
        {
            var toLink = obj.AddComponent(component);
            Debug.Log("Added " + toLink.GetType() + " to " + obj.name);
        }
    }

    public static void LinkOldAiBase(this GameObject linkable, Component toLink)
    {
        var aiBase = linkable.GetComponent<Old_AIBase>();
    }

    public static Sprite GetRandomSprite()
    {
        return null;
    }

    public static Old_AIBase GetRandomOldAiBase()
    {
        return null;
    }

    public static EnumReferences.EnemyType GetRandomEnemyType()
    {
        return 0;
    }

    public static AIStats GetRandomAiStats()
    {
        return null;
    }

    public static string GetRandomName()
    {
        return "";
    }
}
