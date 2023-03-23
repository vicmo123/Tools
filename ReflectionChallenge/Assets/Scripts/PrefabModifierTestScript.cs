using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabModifierTestScript : MonoBehaviour
{
    [PrefabModifiable()]
    public string characterName;
    [PrefabModifiable()]
    public int age;
    [PrefabModifiable()]
    public float speed;
    [PrefabModifiable()]
    public Vector2 startPosition;

    //Should not load
    public float someFloat;

    [PrefabModifiable(), SerializeField]
    private int numberOfEnemies;

    // Start is called before the first frame update
    void Start()
    {
        this.Randomizer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
