using UnityEditor;
using UnityEngine;
using propertyDrawerTool;

// IngredientDrawer
[CustomPropertyDrawer(typeof(Ingredient))]
public class IngredientDrawer : PropertyDrawer
{
    float totalPropertyHeight { get; set; } = 0;
    float width = 100;
    float height = EditorGUIUtility.singleLineHeight;
    float spacing = 5f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //lineSpacing = PropertyDrawerLayoutTool.GenerateProperty(label, position, spacing, PropertyDrawerLayoutTool.GetPropertyFields(property, "Ingredient", width, height));
        totalPropertyHeight = PropertyDrawerLayoutTool.GenerateProperty(label, position, spacing, PropertyDrawerLayoutTool.GetPropertyFields(property, typeof(Ingredient), width, height));
    }


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return totalPropertyHeight;
        //return (base.GetPropertyHeight(property, label) + spacing) * numLines;
    }
}
