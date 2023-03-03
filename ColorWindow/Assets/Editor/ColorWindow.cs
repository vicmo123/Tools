using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;  //Required for MenuItem, means that this is an Editor script, must be placed in an Editor folder, and cannot be compiled!
using System.Linq;  //Used for Select
using Random = UnityEngine.Random;

public class ColorWindow : EditorWindow
{ //Now is of type EditorWindow

    [MenuItem("Custom Tools/ Color Window")] //This the function below it as a menu item, which appears in the tool bar
    public static void CreateShowcase() //Menu items can call STATIC functions, does not work for non-static since Editor scripts cannot be attached to objects
    {
        EditorWindow window = GetWindow<ColorWindow>("Color Window");
    }

    private Color[] colors;
    private int width = 8;
    private int height = 8;
    private float randomnessMultiplicator = 0.0f;
    private bool toggleRandom = false;
    Texture colorTexture;
    Renderer textureTarget;

    Color selectedColor = Color.white;
    Color eraseColor = Color.white;

    public void OnEnable()
    {
        colors = new Color[width * height];
        for (int i = 0; i < colors.Length; i++)
            colors[i] = GetRandomColor();
        colorTexture = EditorGUIUtility.whiteTexture;
    }

    private Color GetRandomColor()  //Built a get random color tool
    {
        return new Color(Random.value, Random.value, Random.value, 1f);
    }

    void OnGUI() //Called every frame in Editor window
    {
        GUILayout.BeginHorizontal();        //Have each element below be side by side
        DoControls();
        DoCanvas();
        GUILayout.EndHorizontal();
    }

    void DoControls()
    {
        GUILayout.BeginVertical();                                                      //Start vertical section, all GUI draw code after this will belong to same vertical
        GUILayout.Label("ToolBar", EditorStyles.largeLabel);                            //A label that says "Toolbar"
        selectedColor = EditorGUILayout.ColorField("Paint Color", selectedColor);       //Make a color field with the text "Paint Color" and have it fill the selectedColor var
        eraseColor = EditorGUILayout.ColorField("Erase Color", eraseColor);             //Make a color field with the text "Erase Color"
        if (GUILayout.Button("Fill All"))                                               //A button, if pressed, returns true
            colors = colors.Select(c => c = selectedColor).ToArray();                   //Linq expresion, for every color in the color array, sets it to the selected color

        //-----------------------------Randomness feature-----------------------------//
        GUILayout.Label("Randomness", EditorStyles.largeLabel);
        GUILayout.BeginHorizontal();
        randomnessMultiplicator = EditorGUILayout.FloatField("Random Value", Mathf.Clamp01(randomnessMultiplicator));
        toggleRandom = GUILayout.Toggle(toggleRandom, GUIContent.none);
        GUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();                                                      //Flexible space uses any left over space in the loadout
        textureTarget = EditorGUILayout.ObjectField("Output Renderer", textureTarget, typeof(Renderer), true) as Renderer;  //Build an object field that accepts a renderer

        if (GUILayout.Button("Save to Object"))
        {
            Texture2D t2d = new Texture2D(width, height);                               //Create a new texture
            t2d.filterMode = FilterMode.Point;                                          //Simplest non-blend texture mode
            textureTarget.material = new Material(Shader.Find("Diffuse"));              //Materials require Shaders as an arguement, Diffuse is the most basic type
            textureTarget.sharedMaterial.mainTexture = t2d;                             //sharedMaterial is the MAIN RESOURCE MATERIAL. Changing this will change ALL objects using it, .material will give you the local instance

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int index = j + i * height;
                    t2d.SetPixel(i, height - 1 - j, colors[index]);                     //Color every pixel using our color table, the texture is 8x8 pixels large, but strecthes to fit
                }
            }
            t2d.Apply();                                                                //Apply all changes to texture
        }
        GUILayout.EndVertical();                                                        //end vertical section
    }

    void DoCanvas()
    {
        Event evt = Event.current;                     //Grab the current event

        Color oldColor = GUI.color;                    //GUI color uses a static var, need to save the original to reset it
        GUILayout.BeginHorizontal();                   //All following gui will be on one horizontal line until EndHorizontal is called

        

        for (int i = 0; i < width; i++)
        {
            GUILayout.BeginVertical();                //All following gui will be in a vertical line
            for (int j = 0; j < height; j++)
            {
                int index = j + i * height;           //Rememeber, this is just like a 2D array, but in 1D
                Rect colorRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)); //Reserve a square, which will autofit to the size given
                if (evt.type == EventType.MouseDown && evt.button == 2 && colorRect.Contains(evt.mousePosition)) //--------------------Bucket Fill-----------------------//
                {
                    BucketFill(new Vector2Int(i, j));
                }
                else if ((evt.type == EventType.MouseDown || evt.type == EventType.MouseDrag) && colorRect.Contains(evt.mousePosition)) //Can now paint while dragging update
                {
                    if (evt.button == 0)                //If mouse button pressed is left
                        colors[index] = toggleRandom ? GetRandomColorVariant(selectedColor) : selectedColor; //Set the color of the index
                    else if (evt.button == 1)
                        colors[index] = eraseColor;   //Set the color of the index
                    evt.Use();                        //The event was consumed, if you try to use event after this, it will be non-sensical
                }
                

                GUI.color = colors[index];            //Same as a 2D array
                GUI.DrawTexture(colorRect, colorTexture); //This is colored by GUI.Color!!!
            }
            GUILayout.EndVertical();                  //End Vertical Zone
        }
        GUILayout.EndHorizontal();                    //End horizontal zone
        GUI.color = oldColor;                         //Restore the old color
    }

    private Color GetRandomColorVariant(Color currentColor)
    {
        return new Color(Mathf.Clamp01(currentColor.r + GetRandomValue()), Mathf.Clamp01(currentColor.g + GetRandomValue()), Mathf.Clamp01(currentColor.b + GetRandomValue()));
    }

    private float GetRandomValue()
    {
        return Random.Range(-randomnessMultiplicator, randomnessMultiplicator);
    }

    private void BucketFill(Vector2Int clickedPosition)
    {
        Color oldColor = colors[clickedPosition.x * height + clickedPosition.y];
        QueueFloodFill(clickedPosition, oldColor, selectedColor);
    }

    private void QueueFloodFill(Vector2Int clickedTarget, Color targetColor, Color replacementColor)
    {
        Queue<Vector2Int> pixels = new Queue<Vector2Int>();
        HashSet<Vector2Int> closedList = new HashSet<Vector2Int>();

        pixels.Enqueue(clickedTarget);
        Vector2Int newPoint;
       
        while (pixels.Count > 0)
        {
            newPoint = pixels.Dequeue();
            closedList.Add(newPoint);

            if (newPoint.x < width && newPoint.x >= 0 && newPoint.y < height && newPoint.y >= 0)
            {
                if (colors[newPoint.x * height + newPoint.y] == targetColor)
                {
                    colors[newPoint.x * height + newPoint.y] = replacementColor;
                    if(!closedList.Contains(new Vector2Int(newPoint.x - 1, newPoint.y)))
                        pixels.Enqueue(new Vector2Int(newPoint.x - 1, newPoint.y));
                    if (!closedList.Contains(new Vector2Int(newPoint.x + 1, newPoint.y)))
                        pixels.Enqueue(new Vector2Int(newPoint.x + 1, newPoint.y));
                    if (!closedList.Contains(new Vector2Int(newPoint.x, newPoint.y - 1)))
                        pixels.Enqueue(new Vector2Int(newPoint.x, newPoint.y - 1));
                    if (!closedList.Contains(new Vector2Int(newPoint.x, newPoint.y + 1)))
                        pixels.Enqueue(new Vector2Int(newPoint.x, newPoint.y + 1));
                }
            }
        }
        return;
    }
}
