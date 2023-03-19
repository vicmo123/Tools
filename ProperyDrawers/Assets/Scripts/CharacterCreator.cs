using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterCreator : MonoBehaviour
{
    public CharacterData Data;
}

[Serializable]
public class CharacterData
{
    public Sprite Image;
    public float ScaleMultiplicator;
    public int HealthPoints;
    public float MovementSpeed;
    public float AttackPower;
    public float AttackRange;
    public BehaviourType Behaviour;
    public GameObject Loot;
    public SpecialAbility SpecialAbility;
    //Not able to make it work for now
    //public KeyValuePair<Elements, DamageType> Damage;
    //Temporary solution
    public Elements Weakness;
    public Elements Resistance;
}

[System.Serializable]
public class DamageStats
{
    public Elements Element;
    public DamageType Damage;
}

public enum SpecialAbility
{
    StunAttack,
    Teleportation,
    Shield,
    Summoning,
    Healing,
    Explosion
}

public enum BehaviourType
{
    ChasePlayer,
    RetreatWhenInjured,
    SpecialAttack
}

public enum Elements
{
    Fire,
    Ice,
    Electricity,
    Metal,
    Water,
    None // for temporary solution
}

public enum DamageType{
    Resistance,
    Weakness,
    Normal
}