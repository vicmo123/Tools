using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using UnityEngine.UIElements;

namespace propertyDrawerTool
{
    public static class PropertyDrawerLayoutTool
    {
        public static float GenerateProperty(GUIContent mainLabel, Rect position, float spacing, params PropertyData[] content)
        {
            EditorGUI.BeginProperty(position, mainLabel, content[0].property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), mainLabel);
            EditorGUI.DrawRect(position, Color.gray);
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            float originalLabelWidth = EditorGUIUtility.labelWidth;

            float offsetX = 0f;
            float offsetY = 0f;
            float rowHeight = 0f;
            float totalPropertyHeight = 0f;

            for (int i = 0; i < content.Length; i++)
            {
                float intialWidth = content[i].width;
                EditorGUIUtility.labelWidth = content[i].label.CalculateLabelLenght(spacing);
                content[i].width += EditorGUIUtility.labelWidth;

                if (offsetX + content[i].width > position.width)
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

                content[i].width = intialWidth;
            }

            EditorGUI.indentLevel = indent;
            EditorGUIUtility.labelWidth = originalLabelWidth;
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

        public static Rect ChangeWidth(this Rect rect, float width)
        {
            return new Rect(rect.x, rect.y, width, rect.height);
        }

        public static PropertyData[] GetPropertyFields(SerializedProperty serializable, System.Type type, float defaultWidth, float defaultHeight)
        {
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            PropertyData[] props = new PropertyData[fields.Length];

            for (int i = 0; i < fields.Length; i++)
            {
                PropertyDataSizeAttribute sizeAttr = fields[i].GetCustomAttribute<PropertyDataSizeAttribute>();
                float width = defaultWidth;
                float height = defaultHeight;
                if (sizeAttr != null)
                {
                    width = sizeAttr.width;
                    height = sizeAttr.height;
                }

                props[i] = new PropertyData(serializable.FindPropertyRelative(fields[i].Name), width, height, new GUIContent(fields[i].Name + ":"));
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

        public static int CountCondition<T>(this IEnumerable<T> collection, System.Predicate<T> condition)
        {
            int count = 0;

            foreach (var item in collection)
            {
                if(condition.Invoke(item) == true)
                {
                    count++;
                }
            }

            return count;
        }

        public static float CalculateLabelLenght(this GUIContent label, float spacing = 0f)
        {
            float labelWidth = EditorStyles.label.CalcSize(label).x;
            float prefixLabelWidth = labelWidth + spacing;

            return prefixLabelWidth;
        }

        public static Rect shrinkFromCenter(this Rect rect, float pourcentage)
        {
            pourcentage = Mathf.Clamp01(pourcentage);
            Vector2 newPosition = new Vector2((rect.center.x - rect.width / 2) * pourcentage, (rect.center.y - rect.height / 2) * pourcentage);
            return new Rect(newPosition.x, newPosition.y, rect.width * pourcentage, rect.height * pourcentage);
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

