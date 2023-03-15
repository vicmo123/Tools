using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace propertyDrawerTool
{
    public static class PropertyDrawerLayoutTool
    {
        public static float GenerateProperty(GUIContent mainLabel, Rect position, float spacing, params PropertyData[] content)
        {
            EditorGUI.BeginProperty(position, mainLabel, content[0].property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), mainLabel);
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            float offsetX = 0f;
            float offsetY = 0f;
            float rowHeight = 0f;
            float totalPropertyHeight = 0f;

            for (int i = 0; i < content.Length; i++)
            {
                if (offsetX + (content[i].width / 2) > position.width)
                {
                    offsetY += rowHeight + spacing;
                    offsetX = 0;
                    rowHeight = 0;
                }

                Rect rect = position.Offset(offsetX, offsetY).ChangeSize(content[i].width, content[i].height);
                
                if (rect.height > rowHeight)
                {
                    rowHeight = rect.height;
                    totalPropertyHeight += rowHeight + spacing;
                }

                EditorGUI.PropertyField(rect, content[i].property, content[i].label, false);
                offsetX += content[i].width + spacing;
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();

            return totalPropertyHeight;
        }

        public static Rect Offset(this Rect rect, float x, float y)
        {
            return new Rect(rect.x + x, rect.y + y, rect.width, rect.height);
        }

        public static Rect ChangeSize(this Rect rect, float width, float height)
        {
            return new Rect(rect.x, rect.y, width, height);
        }

        public static PropertyData[] GetPropertyFields(SerializedProperty serializable, string typeName, float mainWidth, float mainHeight)
        {
            System.Type type = GetSystemTypeWithString(typeName);

            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            PropertyData[] props = new PropertyData[fields.Length];

            for (int i = 0; i < fields.Length; i++)
            {
                props[i] = new PropertyData(serializable.FindPropertyRelative(fields[i].Name), mainWidth, mainHeight);
            }

            return props;
        }

        public static PropertyData[] GetPropertyFields(SerializedProperty serializable, System.Type type, float mainWidth, float mainHeight)
        {
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            PropertyData[] props = new PropertyData[fields.Length];

            for (int i = 0; i < fields.Length; i++)
            {
                props[i] = new PropertyData(serializable.FindPropertyRelative(fields[i].Name), mainWidth, mainHeight/*, new GUIContent(fields[i].Name)*/);
            }

            return props;
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

