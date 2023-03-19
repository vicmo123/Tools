using System;
using UnityEditor;
using UnityEngine;

public enum IngredientUnit { Spoon, Cup, Bowl, Piece }

[Serializable]
public class Ingredient
{
    [PropertyDataSize(width: 120f, height: 60f)] public string name;
    [PropertyDataSize(width: 30f, height: 20f)] public int amount = 1;
    public IngredientUnit unit;
    
}

public class Recipe : MonoBehaviour
{ 
    public Ingredient[] potionIngredients;
}
