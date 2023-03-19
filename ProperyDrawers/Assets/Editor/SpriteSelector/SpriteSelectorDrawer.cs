using UnityEditor;
using UnityEngine;
using propertyDrawerTool;
using System.Linq;
using System;

[CustomPropertyDrawer(typeof(SpriteData))]
public class SpriteSelectorDrawer : PropertyDrawer
{
    Sprite[] sprites;
    int currentSpriteIndex = 0;
    float extraSize = 0;

    float totalPropertyHeight { get; set; } = 0;
    float width = 100;
    float height = EditorGUIUtility.singleLineHeight;
    float spacing = 5f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (sprites == null)
        {
            sprites = Resources.LoadAll<Sprite>("Sprites/");
            sprites.OrderBy(val => val.name);
        }

        var spriteProp = property.FindPropertyRelative("Sprite");

        if (spriteProp.objectReferenceValue == null)
        {
            currentSpriteIndex = 0;
            spriteProp.objectReferenceValue = sprites[currentSpriteIndex];
        }
        else
        {
            currentSpriteIndex = Array.IndexOf(sprites, spriteProp.objectReferenceValue);
        }

        totalPropertyHeight = PropertyDrawerLayoutTool.GenerateProperty(label, position, spacing, PropertyDrawerLayoutTool.GetPropertyFields(property, typeof(SpriteData), width, height));

        if (spriteProp.propertyType == SerializedPropertyType.ObjectReference && spriteProp.objectReferenceValue != null && spriteProp.objectReferenceValue.GetType() == typeof(Sprite))
        {
            // Get the preview texture for the sprite
            Texture2D previewTexture = AssetPreview.GetAssetPreview(spriteProp.objectReferenceValue);

            extraSize = 0;
            // If the preview texture exists, draw it
            if (previewTexture != null)
            {
                float previewSize = 128f * 1.5f; // size of the preview texture
                float previewX = position.x + EditorGUIUtility.labelWidth + spacing; // x position of the preview texture
                float previewY = position.y + EditorGUIUtility.singleLineHeight + spacing; // y position of the preview texture

                Rect previewRect = new Rect(previewX, previewY, previewSize, previewSize);
                EditorGUI.DrawPreviewTexture(previewRect, previewTexture);

                // Add buttons to select the previous and next sprites
                Rect previousButtonRect = new Rect(previewX, previewY + previewSize + spacing, (previewSize - spacing) / 2, EditorGUIUtility.singleLineHeight);
                if (GUI.Button(previousButtonRect, "<"))
                {
                    currentSpriteIndex--;
                    if (currentSpriteIndex < 0) currentSpriteIndex = sprites.Length - 1;
                    spriteProp.objectReferenceValue = sprites[currentSpriteIndex];
                }

                Rect nextButtonRect = new Rect(previousButtonRect.x + previousButtonRect.width + spacing, previewY + previewSize + spacing, (previewSize - spacing) / 2, EditorGUIUtility.singleLineHeight);
                if (GUI.Button(nextButtonRect, ">"))
                {
                    currentSpriteIndex++;
                    if (currentSpriteIndex >= sprites.Length) currentSpriteIndex = 0;
                    spriteProp.objectReferenceValue = sprites[currentSpriteIndex];
                }

                extraSize += previewSize + previousButtonRect.height + nextButtonRect.height;
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // return totalPropertyHeight;
        return (base.GetPropertyHeight(property, label) + spacing) + extraSize;
    }
}
