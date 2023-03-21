using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;
using System;

[CustomPropertyDrawer(typeof(StringDropDownAttribute))]
public class StringDropDownAttributeEditorScript : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        StringDropDownAttribute customAttribute = attribute as StringDropDownAttribute;
        if (customAttribute != null)
        {
            int selected = 0;
            if (property.stringValue != "")
            {
                selected = Array.IndexOf(customAttribute.stringOptions, property.stringValue);
            }

            selected = EditorGUI.Popup(position, property.name, selected, customAttribute.stringOptions);

            property.stringValue = customAttribute.stringOptions[selected];
            property.serializedObject.ApplyModifiedProperties();
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label);
    }
}
