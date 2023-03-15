using UnityEditor;
using UnityEngine;
using propertyDrawerTool;

// IngredientDrawer
[CustomPropertyDrawer(typeof(Ingredient))]
public class IngredientDrawer : PropertyDrawer
{
    int extraLines { get; set; } = 0;
    float width = 100;
    float height = EditorGUIUtility.singleLineHeight;
    float spacing = 5f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        //extraLines = PropertyDrawerLayoutTool.GenerateProperty(label, position, spacing, new PropertyData(property.FindPropertyRelative("amount"), width, height2), new PropertyData(property.FindPropertyRelative("unit"), width, height), new PropertyData(property.FindPropertyRelative("name"), width, height2), new PropertyData(property.FindPropertyRelative("amount"), width, height), new PropertyData(property.FindPropertyRelative("unit"), width, height), new PropertyData(property.FindPropertyRelative("name"), width, height));
        extraLines = PropertyDrawerLayoutTool.GenerateProperty(label, position, spacing, PropertyDrawerLayoutTool.GetPropertyFields(property, "Ingredient", width, height));
    }


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float extraHeight = EditorGUIUtility.singleLineHeight * extraLines + EditorGUIUtility.standardVerticalSpacing * extraLines;
        return base.GetPropertyHeight(property, label) + extraHeight;
    }
}
