using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Cards/Card")]
public class CardDataScriptable : ScriptableObject
{
    public string CharacterName;
    public Colors Color;
    public int ManaCost;
    public Sprite Image;
    public string Description;
    public int Power;
    public int defense;
}
