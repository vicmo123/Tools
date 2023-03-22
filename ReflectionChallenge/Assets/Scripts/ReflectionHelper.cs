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

        // Print the assembly information
        Assembly assembly = type.Assembly;
        classInfo += "  " + "Assembly Name: " + assembly.FullName + "\n";

        // Print the class information
        classInfo += "  " + "Class Name: " + type.Name + "\n";
        classInfo += "  " + "Is Abstract: " + type.IsAbstract + "\n";
        classInfo += "  " + "Is Sealed: " + type.IsSealed + "\n";
        classInfo += "  " + "Is Generic Type Definition: " + type.IsGenericTypeDefinition + "\n";
        classInfo += "  " + "Is Class: " + type.IsClass + "\n";

        // Print the fields information
        classInfo += "Fields: " + "\n";
        foreach (FieldInfo field in type.GetFields(bindingFlags))
        {
            classInfo += "  " + field.GetAccessLevel() + " " + field.FieldType + " " + field.Name + "\n";
        }

        // Print the properties information
        classInfo += "Properties: " + "\n";
        foreach (PropertyInfo property in type.GetProperties(bindingFlags))
        {
            classInfo += "  " + property.GetAccessLevel() + " " + property.PropertyType + " " + property.Name + "\n";
        }

        //// Print the enums information
        //classInfo += "Enums: " + "\n";
        //foreach (Type enumType in type.GetNestedTypes(BindingFlags.Public))
        //{
        //    if (enumType.IsEnum)
        //    {
        //        classInfo += "  " + "Enum: " + enumType.Name + "\n";
        //        foreach (string enumName in Enum.GetNames(enumType))
        //        {
        //            classInfo += "  " + enumName + "\n";
        //        }
        //    }
        //}

        //// Print the structs information
        //classInfo += "Structs: " + "\n";
        //foreach (Type structType in type.GetNestedTypes(BindingFlags.Public))
        //{
        //    if (structType.IsValueType && !structType.IsEnum)
        //    {
        //        classInfo += "  " + "Struct: " + structType.Name + "\n";
        //        foreach (FieldInfo field in structType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        //        {
        //            classInfo += "  " + field.FieldType + " " + field.Name + "\n";
        //        }
        //    }
        //}

        // Print the methods information
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
}

