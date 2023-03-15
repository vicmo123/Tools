using System;
using UnityEditor;
using UnityEngine;

public enum IngredientUnit { Spoon, Cup, Bowl, Piece }
// Custom serializable class
[Serializable]
public class Ingredient
{
    [PropertyDataSize(width: 150f, height: 60f)] public AnimationCurve curve;
    [PropertyDataSize(width: 120f, height: 60f)] public string name;
    [PropertyDataSize(width: 30f, height: 20f)] public int amount = 1;
    public IngredientUnit unit;
    [PropertyDataSize(width: 50f, height: 60f)] public float size;
    
}

public class Recipe : MonoBehaviour
{ 
    public Ingredient[] potionIngredients;
}
