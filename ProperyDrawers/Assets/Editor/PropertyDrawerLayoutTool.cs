using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace propertyDrawerTool
{
    public static class PropertyDrawerLayoutTool
    {
        public static int GenerateProperty(GUIContent mainLabel, Rect position, float spacing, params PropertyData[] content)
        {
            EditorGUI.BeginProperty(position, mainLabel, content[0].property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), mainLabel);
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            float offsetX = 0f;
            float offsetY = 0f;
            float rowHeight = 0f;
            int extraLines = 0;

            for (int i = 0; i < content.Length; i++)
            {
                Rect rect = position.Offset(offsetX, offsetY).ChangeSize(content[i].width, content[i].height);

                EditorGUI.PropertyField(rect, content[i].property, content[i].label, false);

                if (rect.height > rowHeight)
                {
                    rowHeight = rect.height;
                }

                offsetX += content[i].width + spacing;
                if (offsetX > position.width)
                {
                    offsetY += rowHeight + spacing;
                    offsetX = 0;
                    rowHeight = 0;
                    extraLines++;
                }
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();

            return extraLines;
        }

        public static Rect Offset(this Rect rect, float x = 0, float y = 0)
        {
            return new Rect(rect.x + x, rect.y + y, rect.width, rect.height);
        }

        public static Rect ChangeSize(this Rect rect, float width = 0, float height = 0)
        {
            return new Rect(rect.x, rect.y, width, height);
        }

        public static PropertyData[] GetPropertyFields(SerializedProperty serializable, string typeName, float mainWidth, float mainHeight)
        {
            List<PropertyData> props = new List<PropertyData>();

            System.Type type = GetSystemTypeWithString(typeName);
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (var field in fields)
            {
                props.Add(new PropertyData(serializable.FindPropertyRelative(field.Name), mainWidth, mainHeight));
            }

            return props.ToArray();
        }

        public static System.Type GetSystemTypeWithString(this string toType)
        {
            try
            {
                Assembly asm = typeof(Recipe).Assembly;
                System.Type newType = asm.GetType(toType);

                return newType;
            }
            catch (System.Exception)
            {
                Debug.LogError("Cannot convert " + toType + "into System.Type");
            }

            return null;
        }
    }

    

    public struct PropertyData
    {
        public SerializedProperty property;
        public SerializedPropertyType type;
        public float width, height;
        public GUIContent label;

        public PropertyData(SerializedProperty property, float width, float height, GUIContent label = null)
        {
            this.property = property;
            this.type = property.propertyType;
            this.width = width;
            this.height = height;
            this.label = label;

            if(this.label == null)
            {
                this.label = GUIContent.none;
            }
        }
    }
}

