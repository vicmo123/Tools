using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;  //Required for MenuItem, means that this is an Editor script, must be placed in an Editor folder, and cannot be compiled!
using System.Linq;  //Used for Select
using Random = UnityEngine.Random;
using System.Text;
using System;

public class CardCreatorWindow : EditorWindow
{
    private CardDataScriptable cardData;
    private Sprite[] sprites;
    private int currentSpriteIndex = 0;

    [MenuItem("Custom Tools/Card Creator Window")] //This the function below it as a menu item, which appears in the tool bar
    public static void CreateShowcase() //Menu items can call STATIC functions, does not work for non-static since Editor scripts cannot be attached to objects
    {
        EditorWindow window = GetWindow<CardCreatorWindow>("Card Creator");
        Vector2 windowSize = new Vector2(600, 700);
        Vector2 windowPos = new Vector2((Screen.currentResolution.width - windowSize.x) / 2f, (Screen.currentResolution.height - windowSize.y) / 2f);
        window.position = new Rect(windowPos, windowSize);
        window.minSize = new Vector2(600, 700);
    }

    Texture colorTexture;
    Vector2 cardSize = new Vector2(300, 400);

    private void OnEnable()
    {
        colorTexture = EditorGUIUtility.whiteTexture;
        sprites = Resources.LoadAll<Sprite>("Sprites/");
        sprites.OrderBy(val => val.name);
    }


    private void OnGUI()
    {
        
        Vector2 editorSize = new Vector2(EditorWindow.focusedWindow.position.width, EditorWindow.focusedWindow.position.height);

        PrintControls();
        cardData = EditorGUILayout.ObjectField("Card Data", cardData, typeof(CardDataScriptable), false) as CardDataScriptable;

        if (cardData != null)
        {
            currentSpriteIndex = Array.IndexOf(sprites, cardData.Image);

            PrintFields();

            PrintCardPreview(editorSize);
        }
    }

    private void SaveCardData()
    {
        if (cardData != null)
        {
            EditorUtility.SetDirty(cardData);
            AssetDatabase.SaveAssets();
        }
    }

    private void PrintFields()
    {
        cardData.name = EditorGUILayout.TextField("File Name : ", cardData.name);
        cardData.CharacterName = EditorGUILayout.TextField("Character Name : ", cardData.CharacterName);
        cardData.ManaCost = EditorGUILayout.IntField("ManaCost", cardData.ManaCost);
        cardData.Power = EditorGUILayout.IntField("Attack", cardData.Power);
        cardData.defense = EditorGUILayout.IntField("Defense", cardData.defense);
        cardData.Description = EditorGUILayout.TextField(
        "Description : ",
        cardData.Description,
        EditorStyles.textArea,
        GUILayout.Height(EditorGUIUtility.singleLineHeight * 2)
        );
        cardData.Color = (Colors)EditorGUILayout.IntSlider("Card Color: ", (int)cardData.Color, 0, Enum.GetValues(typeof(Colors)).Length - 1);
    }

    private void PrintControls()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Load File"))
        {
            Debug.Log("load");
        }
        if (GUILayout.Button("Go To File "))
        {
            Debug.Log("go to...");
        }
        if (GUILayout.Button("Create New Card"))
        {
            Debug.Log("New Card");
        }
        GUILayout.EndHorizontal();
    }

    private void PrintCardPreview(Vector2 editorSize)
    {
        var oldColor = GUI.color;
        GUI.color = cardData.Color.GetUnityEngineColor();
        GUI.Box(new Rect((editorSize.x - cardSize.x) / 2f, (editorSize.y - cardSize.y) * 3f / 4f, cardSize.x, cardSize.y), "", GUI.skin.window);
        GUI.color = oldColor;

        //Center of screen
        GUILayout.BeginArea(new Rect((editorSize.x - cardSize.x) / 2f, (editorSize.y - cardSize.y) * 3f / 4f, cardSize.x, cardSize.y));
        {
            GUILayout.Label(cardData.CharacterName);

            PrintImageSelector();
            PrintCardInfo();
        }
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect((editorSize.x - cardSize.x) / 2f, (editorSize.y - cardSize.y) * 3f / 4f + cardSize.y + 10f, cardSize.x, 50f));
        {
            if (GUILayout.Button("Save Card"))
            {
                SaveCardData();
            }
        }
        GUILayout.EndArea();
    }

    private void PrintImageSelector()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.MiddleCenter;
        style.padding.right += 50;
        style.padding.left += 50;

        GUILayout.BeginHorizontal();
        {
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("<", GUILayout.Height(40), GUILayout.Width(40)))
            {
                currentSpriteIndex--;
                if (currentSpriteIndex < 0) currentSpriteIndex = sprites.Length - 1;
                cardData.Image = sprites[currentSpriteIndex];
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();

            Rect rect = GUILayoutUtility.GetRect(200, 200, style);
            GUI.Box(rect, "", style);
            GUI.DrawTexture(rect, cardData.Image.texture, ScaleMode.ScaleToFit);

            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(">", GUILayout.Height(40), GUILayout.Width(40)))
            {
                currentSpriteIndex++;
                if (currentSpriteIndex >= sprites.Length) currentSpriteIndex = 0;
                cardData.Image = sprites[currentSpriteIndex];
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
    }

    private void PrintCardInfo()
    {
        GUILayout.Label("Card Info");
        Rect previewRect = GUILayoutUtility.GetRect(cardSize.x, cardSize.y);
        //GUILayout.ExpandHeight(false);
        float spacing = 10;
        previewRect.x += spacing; previewRect.width -= spacing * 2; previewRect.height -= spacing * 3;

        if (Event.current.type == EventType.Repaint)
        {
            GUIStyle cardStyle = new GUIStyle(GUI.skin.box);
            cardStyle.normal.background = (Texture2D)colorTexture;
            GUI.Box(previewRect, "", cardStyle);

            float x = previewRect.x + 10;
            float y = previewRect.y + 10;
            GUI.Label(new Rect(x, y, previewRect.width - 20, 20), "Cost: " + cardData.ManaCost.ToString());
            y += 20;
            GUI.Label(new Rect(x, y, previewRect.width - 20, 20), "Attack: " + cardData.Power.ToString());
            y += 20;
            GUI.Label(new Rect(x, y, previewRect.width - 20, 20), "Defense: " + cardData.defense.ToString());
            y += 20;
            string wrappedDescription = ("Description: " + cardData.Description).WrapText(45);
            float descriptionHeight = GUI.skin.label.CalcHeight(new GUIContent(wrappedDescription), previewRect.width - 20);
            GUI.Label(new Rect(x, y, previewRect.width - 20, descriptionHeight), wrappedDescription);
        }
    }
}
