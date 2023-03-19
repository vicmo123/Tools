using UnityEditor;
using UnityEngine;
using propertyDrawerTool;

[CustomPropertyDrawer(typeof(CharacterData))]
public class CharacterDataDrawer : PropertyDrawer
{
    float totalPropertyHeight { get; set; } = 0;
    float width = 100;
    float height = EditorGUIUtility.singleLineHeight;
    float spacing = 5f;
    const float arrElementHeight = 16f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        totalPropertyHeight = PropertyDrawerLayoutTool.GenerateProperty(label, position, spacing, PropertyDrawerLayoutTool.GetPropertyFields(property, typeof(CharacterData), width, height));
    }


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return totalPropertyHeight;
    }
}
