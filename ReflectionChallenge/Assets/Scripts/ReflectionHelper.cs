using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;

public static class ReflectionHelper
{
    static BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
    
    //First challenge
    public static void FindWordsInProject(this HashSet<String> wordsToFind)
    {
        Queue<string> projectWordsToCheck = new Queue<string>();

        Type[] allTypes = Assembly.GetExecutingAssembly().GetTypes();

        foreach (var type in allTypes)
        {
            projectWordsToCheck.Enqueue(type + "/" + type.Name);

            PropertyInfo[] allPropertyInfos = type.GetProperties(bindingFlags);
            MethodInfo[] allMethodInfos = type.GetMethods(bindingFlags);
            FieldInfo[] allFieldInfos = type.GetFields(bindingFlags);

            for (int i = 0; i < allPropertyInfos.Length; i++)
            {
                projectWordsToCheck.Enqueue(type + "/" + allPropertyInfos[i].Name);
            }

            for (int i = 0; i < allMethodInfos.Length; i++)
            {
                projectWordsToCheck.Enqueue(type + "/" + allMethodInfos[i].Name);
                var parameters = allMethodInfos[i].GetParameters();
                for (int j = 0; j < parameters.Length; j++)
                {
                    projectWordsToCheck.Enqueue(type + "/" + parameters[j].Name);
                }
            }

            for (int i = 0; i < allFieldInfos.Length; i++)
            {
                projectWordsToCheck.Enqueue(type + "/" + allFieldInfos[i].Name);
            }
        }
        for (int i = 0; i < projectWordsToCheck.Count; i++)
        {
            string[] value = projectWordsToCheck.Dequeue().Split("/");
            string toCheck = value[1];
            string type = value[0];
            foreach (string toFind in wordsToFind)
            {
                if(toFind.Length > 0)
                {
                    if (toCheck.Contains(toFind, StringComparison.OrdinalIgnoreCase))
                    {
                        Debug.Log("word: " + toFind + " was found in type: " + type);
                    }
                }
            }
        }
    }
}
