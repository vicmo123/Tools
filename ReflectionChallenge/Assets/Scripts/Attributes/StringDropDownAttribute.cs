using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class StringDropDownAttribute : PropertyAttribute
{
    public string[] stringOptions;

    public StringDropDownAttribute(params string[] stringsToAdd)
    {
        stringOptions = new string[stringsToAdd.Length];
        for (int i = 0; i < stringOptions.Length; i++)
        {
            stringOptions[i] = stringsToAdd[i];
        }
    }

    public StringDropDownAttribute(bool showAllTypes)
    {
        if (showAllTypes)
        {
            var typeStrings = ReflectionHelper.GetAllTypeStrings();
            stringOptions = new string[typeStrings.Length];
            for (int i = 0; i < typeStrings.Length; i++)
            {
                stringOptions[i] = typeStrings[i];
            }
        }
    }
}
