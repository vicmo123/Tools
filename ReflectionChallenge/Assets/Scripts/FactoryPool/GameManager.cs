using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    GenericWrapper<EnemyTypes, Enemy, EnemyData> enemySystem;
    [SerializeField] private int prefillAmount = 10;

    private void Awake()
    {
        enemySystem = GenericWrapper<EnemyTypes, Enemy, EnemyData>.Instance;
        enemySystem.PreInitialize();
    }

    private void Start()
    {
        SetEnemySystemDelegates();
        enemySystem.Initialize();
    }

    private void Update()
    {
        enemySystem.Refresh();
    }

    private void FixedUpdate()
    {
        enemySystem.PhysicsRefresh();
    }

    private void SetEnemySystemDelegates()
    {
        enemySystem.manager.Init += () =>
        {
            PrefillPool();
        };
        enemySystem.manager.Update += () =>
        {

            EnemyData data = EnemyData.GenerateData();
            enemySystem.manager.AddObjectToCollection(data.Type, data);

            enemySystem.manager.ManageCollectionWithConditions((val) => { return val.CheckBounds(); }, (val) => { return false; });
        };
        enemySystem.manager.PhysicsUpdate += () => { };
    }

    private void PrefillPool()
    {
        for (int i = 0; i < prefillAmount; i++)
        {
            EnemyData data = EnemyData.GenerateData();
            Enemy newEnemy = enemySystem.objectFactory.CreateObject(data.Type, data);
            enemySystem.objectPool.Pool(newEnemy.GetEnumType(), newEnemy);
        }
    }
}

