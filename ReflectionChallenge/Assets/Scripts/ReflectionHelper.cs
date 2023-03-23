using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;
using Random = UnityEngine.Random;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
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
                if (toFind.Length > 0)
                {
                    if (toCheck.Contains(toFind, StringComparison.OrdinalIgnoreCase))
                    {
                        Debug.Log("word: " + toFind + " was found in type: " + type);
                    }
                }
            }
        }
    }

    public static Action GetMethodWithAttribute<Attribute>(this MonoBehaviour script) where Attribute : System.Attribute
    {
        var methodInfos = script.GetType().GetMethods();

        return () =>
        {
            foreach (var methodInfo in methodInfos)
            {
                var customAttr = methodInfo.GetCustomAttribute(typeof(Attribute), false);
                if (customAttr != null)
                {
                    Action method = methodInfo.CreateDelegate(typeof(Action), script) as Action;
                    method();
                }
            }
        };
    }

    public static void CheckAndFixCustomRangeValues(this MonoBehaviour[] objects)
    {
        foreach (var obj in objects)
        {
            var fields = obj.GetType().GetFields();
            foreach (var field in fields)
            {
                CustomRangeAttribute attr = (CustomRangeAttribute)field.GetCustomAttribute(typeof(CustomRangeAttribute), false);
                if (attr != null)
                {
                    int fieldValue = (int)field.GetValue(obj);
                    if (!attr.CheckIfValid(fieldValue))
                    {
                        field.SetValue(obj, attr.ClampValue(fieldValue));
                    }
                }
            }
        }
    }

    public static void DisplayClassInfo(this string classType)
    {
        Type type = Assembly.GetExecutingAssembly().GetType(classType);
        string classInfo = "";
        classInfo += "Class Information for type: " + type.FullName + "\n";

        //Print the assembly information
        Assembly assembly = type.Assembly;
        classInfo += "  " + "Assembly Name: " + assembly.FullName + "\n";

        //Print the class information
        classInfo += "  " + "Class Name: " + type.Name + "\n";
        classInfo += "  " + "Is Abstract: " + type.IsAbstract + "\n";
        classInfo += "  " + "Is Sealed: " + type.IsSealed + "\n";
        classInfo += "  " + "Is Generic Type Definition: " + type.IsGenericTypeDefinition + "\n";
        classInfo += "  " + "Is Class: " + type.IsClass + "\n";

        //Print the fields information
        classInfo += "Fields: " + "\n";
        foreach (FieldInfo field in type.GetFields(bindingFlags))
        {
            classInfo += "  " + field.GetAccessLevel() + " " + field.FieldType + " " + field.Name + "\n";
        }

        //Print the properties information
        classInfo += "Properties: " + "\n";
        foreach (PropertyInfo property in type.GetProperties(bindingFlags))
        {
            classInfo += "  " + property.GetAccessLevel() + " " + property.PropertyType + " " + property.Name + "\n";
        }

        //Print the methods information
        classInfo += "Methods:" + "\n";
        foreach (MethodInfo method in type.GetMethods(bindingFlags))
        {
            classInfo += "  " + method.GetAccessLevel() + " " + method.ReturnType + " " + method.Name + "(";
            ParameterInfo[] parameters = method.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                classInfo += parameters[i].ParameterType + " " + parameters[i].Name;
                if (i < parameters.Length - 1) classInfo += ", ";
            }
            classInfo += ")" + "\n";
        }

        Debug.Log(classInfo);
    }

    public static string[] GetAllTypeStrings()
    {
        Type[] types = Assembly.GetExecutingAssembly().GetTypes();
        List<string> typeStrings = new List<string>();

        foreach (var t in types)
        {
            typeStrings.Add(t.Name);
        }

        return typeStrings.ToArray();
    }

    public static Type GetSystemTypeWithString(this string typeName)
    {
        return Assembly.GetExecutingAssembly().GetType(typeName);
    }

    public static string GetAccessLevel(this MemberInfo member)
    {
        if (member is FieldInfo field)
        {
            if (field.IsPublic)
            {
                return "public";
            }
            else if (field.IsPrivate)
            {
                return "private";
            }
            else if (field.IsFamily)
            {
                return "protected";
            }
            else if (field.IsAssembly)
            {
                return "internal";
            }
        }
        else if (member is PropertyInfo property)
        {
            MethodInfo getter = property.GetGetMethod(true);
            MethodInfo setter = property.GetSetMethod(true);

            if (getter != null && getter.IsPublic || setter != null && setter.IsPublic)
            {
                return "public";
            }
            else if (getter != null && getter.IsPrivate || setter != null && setter.IsPrivate)
            {
                return "private";
            }
            else if (getter != null && getter.IsFamily || setter != null && setter.IsFamily)
            {
                return "protected";
            }
            else if (getter != null && getter.IsAssembly || setter != null && setter.IsAssembly)
            {
                return "internal";
            }
        }
        else if (member is MethodInfo method)
        {
            if (method.IsPublic)
            {
                return "public";
            }
            else if (method.IsPrivate)
            {
                return "private";
            }
            else if (method.IsFamily)
            {
                return "protected";
            }
            else if (method.IsAssembly)
            {
                return "internal";
            }
        }
        //default value if member is neither a field nor a method
        return "unknown";
    }

    public static void FindAllFunctionsWithNumberOfParameter(int maxNumberOfParams)
    {
        var allTypes = Assembly.GetExecutingAssembly().GetTypes();
        bool noMethodFound = true;

        foreach (var t in allTypes)
        {
            var mInfos = t.GetMethods(bindingFlags);

            foreach (var method in mInfos)
            {
                ParameterInfo[] parameters = method.GetParameters();
                int numberOfParams = parameters.Length;
                string methodInfoString = "";

                if (numberOfParams >= maxNumberOfParams)
                {
                    noMethodFound = false;

                    methodInfoString += "Invalid Method: {" + method.GetAccessLevel() + " " + method.ReturnType + " " + method.Name + "(";
                    
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        methodInfoString += parameters[i].ParameterType + " " + parameters[i].Name;
                        if (i < parameters.Length - 1) methodInfoString += ", ";
                    }
                    methodInfoString += ")}";

                    Debug.Log(methodInfoString + " in type " + t.Name + " has " + maxNumberOfParams + " or more parameters. ");
                }
            }
        }

        if (noMethodFound)
        {
            Debug.Log("No methods have " + maxNumberOfParams + " or more parameters");
        }
    }

    public static MonoBehaviour[] GetAllComponents(this GameObject go)
    {
        MonoBehaviour[] components = go.GetComponents<MonoBehaviour>();
        return components;
    }

    public static FieldInfo[] GetAllFieldWithAttribute<T>(this MonoBehaviour component) where T : Attribute
    {
        List<FieldInfo> listFieldInfo= new List<FieldInfo>();

        Type type = component.GetType();
        var fields = type.GetFields(bindingFlags);

        foreach (var field in fields)
        {
            T attr = (T)field.GetCustomAttribute(typeof(T), false);
            if(attr != null)
            {
                listFieldInfo.Add(field);
            }
        }

        return listFieldInfo.ToArray();
    }

    public static void Randomizer(this MonoBehaviour mono)
    {
        Type[] typesToRandomize = { typeof(int), typeof(float), typeof(Vector2) };

        StackFrame callingFrame = new StackFrame(1, true);
        MethodBase callingMethod = callingFrame.GetMethod();
        object callingInstance = callingFrame.GetMethod().IsStatic ? null : callingFrame.GetMethod().ReflectedType;

        if (callingInstance != null)
        {
            Type type = Type.GetType(callingInstance.ToString());
            var fields = type.GetFields(bindingFlags);
            var properties = type.GetProperties(bindingFlags);

            foreach (var f in fields)
            {
                foreach (var t in typesToRandomize)
                {
                    if(f.FieldType == t)
                    {
                        Debug.Log(f.Name + ": " + f.GetValue(mono));
                        Debug.Log("Randomize...");
                        if (t == typeof(int))
                        {
                            f.SetValue(mono, (int)Random.Range(0, 100));
                        }
                        else if (t == typeof(float))
                        {
                            f.SetValue(mono, (float)Random.Range(0f, 100f));
                        }
                        else if (t == typeof(Vector2))
                        {
                            f.SetValue(mono, new Vector2((float)Random.Range(-100f, 100f), (float)Random.Range(-100f, 100f)));
                        }
                        Debug.Log(f.Name + ": " + f.GetValue(mono));
                    }
                } 
            }

            foreach (var p in properties)
            {
                foreach (var t in typesToRandomize)
                {
                    if (p.PropertyType == t)
                    {
                        Debug.Log(p.Name + ": " + p.GetValue(mono));
                        Debug.Log("Randomize...");
                        if (t == typeof(int))
                        {
                            p.SetValue(mono, (int)Random.Range(0, 100));
                        }
                        else if (t == typeof(float))
                        {
                            p.SetValue(mono, (float)Random.Range(0f, 100f));
                        }
                        else if (t == typeof(Vector2))
                        {
                            p.SetValue(mono, new Vector2((float)Random.Range(-100f, 100f), (float)Random.Range(-100f, 100f)));
                        }
                        Debug.Log(p.Name + ": " + p.GetValue(mono));
                    }
                }
            }
        }
    }
}

