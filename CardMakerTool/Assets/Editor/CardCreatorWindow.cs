using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using Random = UnityEngine.Random;
using System.Text;
using System;
using System.IO;

public class CardCreatorWindow : EditorWindow
{
    private CardDataScriptable cardClone = null;
    private CardDataScriptable cardData = null;
    private Sprite[] sprites;
    private int currentSpriteIndex = 0;

    [MenuItem("Custom Tools/Card Creator Window")]
    public static void CreateShowcase()
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
        if (!EditorWindow.focusedWindow.titleContent.text.Contains("Card Creator"))
        {
            CardCreatorWindow myWindow = EditorWindow.GetWindow<CardCreatorWindow>();
            myWindow.Focus();
        }

        Vector2 editorSize = new Vector2(EditorWindow.focusedWindow.position.width, EditorWindow.focusedWindow.position.height);

        PrintControls();
        CardDataScriptable previousCardData = cardData;
        cardData = EditorGUILayout.ObjectField("Card Data", cardData, typeof(CardDataScriptable), false) as CardDataScriptable;

        if (cardData != null)
        {
            if (cardData != previousCardData || cardClone == null)
            {
                cardClone = Instantiate(cardData);
            }

            currentSpriteIndex = Array.IndexOf(sprites, cardClone.Image);
            PrintFields();
            PrintCardPreview(editorSize);
        }
    }


    private void PrintFields()
    {
        //cardClone.name = EditorGUILayout.TextField("File Name : ", cardClone.name.Split("(")[0]); 
        cardClone.name = cardClone.name.Split("(")[0];
        cardClone.CharacterName = EditorGUILayout.TextField("Character Name : ", cardClone.CharacterName);
        cardClone.ManaCost = EditorGUILayout.IntField("ManaCost", cardClone.ManaCost);
        cardClone.Power = EditorGUILayout.IntField("Attack", cardClone.Power);
        cardClone.defense = EditorGUILayout.IntField("Defense", cardClone.defense);
        cardClone.Description = EditorGUILayout.TextField(
            "Description : ",
            cardClone.Description,
            EditorStyles.textArea,
            GUILayout.Height(EditorGUIUtility.singleLineHeight * 2)
        );
        cardClone.Color = (Colors)EditorGUILayout.IntSlider("Card Color: ", (int)cardClone.Color, 0, Enum.GetValues(typeof(Colors)).Length - 1);
    }

    private void PrintControls()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Load File"))
        {
            Load();
        }
        if (GUILayout.Button("Go To File "))
        {
            GoToFile();
        }
        if (GUILayout.Button("Create New Card"))
        {
            CreateNewCard();
        }
        GUILayout.EndHorizontal();
    }

    private void PrintCardPreview(Vector2 editorSize)
    {
        var oldColor = GUI.color;
        GUI.color = cardClone.Color.GetUnityEngineColor();
        GUI.Box(new Rect((editorSize.x - cardSize.x) / 2f, (editorSize.y - cardSize.y) * 3f / 4f, cardSize.x, cardSize.y), "", GUI.skin.window);
        GUI.color = oldColor;

        //Center of screen
        GUILayout.BeginArea(new Rect((editorSize.x - cardSize.x) / 2f, (editorSize.y - cardSize.y) * 3f / 4f, cardSize.x, cardSize.y - 10));
        {
            GUIStyle boldCenteredStyle = new GUIStyle(GUI.skin.label);
            boldCenteredStyle.fontStyle = FontStyle.Bold;
            boldCenteredStyle.alignment = TextAnchor.MiddleCenter;
            boldCenteredStyle.fontSize = 17;
            GUILayout.Label(cardClone.CharacterName, boldCenteredStyle);

            PrintImageSelector();
            PrintCardInfo();
        }
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect((editorSize.x - cardSize.x) / 2f, (editorSize.y - cardSize.y) * 3f / 4f + cardSize.y + 10f, cardSize.x, 50f));
        {
            if (GUILayout.Button("Revert Changes"))
            {
                ResetCard();
            }
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
                cardClone.Image = sprites[currentSpriteIndex];
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();

            Rect rect = GUILayoutUtility.GetRect(200, 200, style);
            GUI.Box(rect, "", style);
            GUI.DrawTexture(rect, cardClone.Image.texture, ScaleMode.ScaleToFit);

            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(">", GUILayout.Height(40), GUILayout.Width(40)))
            {
                currentSpriteIndex++;
                if (currentSpriteIndex >= sprites.Length) currentSpriteIndex = 0;
                cardClone.Image = sprites[currentSpriteIndex];
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
        GUIStyle boldCenteredStyle = new GUIStyle(GUI.skin.label);
        boldCenteredStyle.fontStyle = FontStyle.Bold;
        boldCenteredStyle.alignment = TextAnchor.MiddleCenter;
        boldCenteredStyle.fontSize = 13;
        GUILayout.Label("- Card Info -", boldCenteredStyle);

        Rect previewRect = GUILayoutUtility.GetRect(cardSize.x, cardSize.y - 20);
        float spacing = 10;
        previewRect.x += spacing;
        previewRect.width -= spacing * 2;
        previewRect.height += 40;

        if (Event.current.type == EventType.Repaint)
        {
            GUIStyle cardStyle = new GUIStyle(GUI.skin.box);
            cardStyle.normal.background = (Texture2D)colorTexture;
            GUI.Box(previewRect, "", cardStyle);

            float x = previewRect.x + 10;
            float y = previewRect.y + 10;
            GUI.Label(new Rect(x, y, previewRect.width - 20, 20), "Cost: " + cardClone.ManaCost.ToString());
            y += 20;
            GUI.Label(new Rect(x, y, previewRect.width - 20, 20), "Attack: " + cardClone.Power.ToString());
            y += 20;
            GUI.Label(new Rect(x, y, previewRect.width - 20, 20), "Defense: " + cardClone.defense.ToString());
            y += 20;
            string wrappedDescription = ("Description: " + cardClone.Description).WrapText(45);
            float descriptionHeight = GUI.skin.label.CalcHeight(new GUIContent(wrappedDescription), previewRect.width - 20);
            GUI.Label(new Rect(x, y, previewRect.width - 20, descriptionHeight), wrappedDescription);
        }
    }

    private void SaveCardData()
    {
        if (cardClone != null && cardData != null)
        {
            EditorUtility.CopySerialized(cardClone, cardData);
            EditorUtility.SetDirty(cardData);
            AssetDatabase.SaveAssets();
        }
    }

    private void ResetCard()
    {
        if (cardClone != null && cardData != null)
        {
            cardClone = Instantiate(cardData);
        }
    }

    private void Load()
    {
        string folderPath = EditorUtility.OpenFilePanel("Select Card", "Resources", "asset");

        if (string.IsNullOrEmpty(folderPath))
        {
            Debug.Log("File dialog cancelled.");
            return;
        }

        int resourcesIndex = folderPath.IndexOf("Resources");
        string resourcePath = folderPath.Substring(resourcesIndex + "Resources".Length + 1);

        cardData = Resources.Load<CardDataScriptable>(resourcePath.Split(".")[0]);
        cardClone = Instantiate(cardData);
        Debug.Log($"Loaded {resourcePath}");
    }

    private void GoToFile()
    {
        if (cardData != null)
        {
            string cardPath = AssetDatabase.GetAssetPath(cardData);
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(cardPath, typeof(UnityEngine.Object)));
        }
    }

    private void CreateNewCard()
    {
        string defaultName = "NewCard";
        string defaultPath = "Assets/Resources";
        string extension = "asset";
        string fullPath = EditorUtility.SaveFilePanelInProject("Create New Card", defaultName, extension, "Enter a name for the new card.", defaultPath);

        if (!string.IsNullOrEmpty(fullPath))
        {
            CardDataScriptable newCard = ScriptableObject.CreateInstance<CardDataScriptable>();
            AssetDatabase.CreateAsset(newCard, fullPath);
            AssetDatabase.SaveAssets();
            Selection.activeObject = newCard;
            newCard.Image = sprites[0];
            SaveChanges();
            cardData = newCard;
            cardClone = Instantiate(newCard);
        }
    }
}
