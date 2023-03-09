using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIBase : MonoBehaviour
{
    public EnumReferences.EnemyType enemyType;
    public Rigidbody2D rb;
    public Collider2D coli;
    public SpriteRenderer sr;
    public AIStats aiStats;
    
    public void Start()
    {
        aiStats = Instantiate(aiStats);
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            aiStats.hp = aiStats.hp - 1f;
            if (aiStats.hp < 0)
            {
                GameObject.Destroy(gameObject);
            }
        }
    }
}
