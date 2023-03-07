using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IFactoryInitializable<EnemyData>, IPoolable
{
    private readonly float BigMult = 1.5f;
    private readonly float MedMult = 1f;
    private readonly float SmallMult = 0.5f;

    public EnemyData enemyData { get; set; }

    private bool isActive = true;
    public bool IsActive
    {
        get { return isActive; }
        set { isActive = value; }
    }

    private Rigidbody2D rb;
    private Vector2 velocity;
    private float speed;

    public void FixedUpdate()
    {
        MoveEnemy();
    }

    public void Initialize(EnemyData data)
    {
        rb = GetComponent<Rigidbody2D>();
        enemyData = data;
        SetPosition();
        SetScale();
        velocity = enemyData.Direction;
        speed = enemyData.Speed;
    }

    void SetPosition()
    {
        transform.position = enemyData.Position;
    }

    void SetScale()
    {
        switch (enemyData.Type)
        {
            case EnemyTypes.BigEnemy:
                transform.localScale = new Vector3(1,1,1) * BigMult;
                break;
            case EnemyTypes.MediumEnemy:
                transform.localScale = new Vector3(1, 1, 1) * MedMult;
                break;
            case EnemyTypes.SmallEnemy:
                transform.localScale = new Vector3(1, 1, 1) * SmallMult;
                break;
            default:
                break;
        }
    }

    private void MoveEnemy()
    {
        rb.AddForce(velocity * speed, ForceMode2D.Force);
    }

    public void Activate()
    {
        IsActive = true;
        gameObject.SetActive(IsActive);
    }

    public void Deactivate()
    {
        IsActive = false;
        gameObject.SetActive(IsActive);
    }
}
