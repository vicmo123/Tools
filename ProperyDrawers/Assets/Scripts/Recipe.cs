using System;
using UnityEngine;

public enum IngredientUnit { Spoon, Cup, Bowl, Piece }

// Custom serializable class
[Serializable]
public class Ingredient
{
    public AnimationCurve curve;
    public string name;
    public int amount = 1;
    public IngredientUnit unit;
    public float size;
    public Vector2 vect2;
}

public class Recipe : MonoBehaviour
{
    public Ingredient potionResult;
    public Ingredient[] potionIngredients;
}
