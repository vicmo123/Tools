using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    GenericWrapper<EnemyTypes, Enemy, EnemyData> enemySystem;
    [SerializeField] private int initialNumberOfEnemies = 10;

    private float screenHeight;
    private float screenWidth;

    private void SetBounds()
    {
        var cam = Camera.main;
        var screenBottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, transform.position.z));
        var screenTopRight = cam.ViewportToWorldPoint(new Vector3(1, 1, transform.position.z));
        screenWidth = screenTopRight.x - screenBottomLeft.x;
        screenHeight = screenTopRight.y - screenBottomLeft.y;
    }

    private void Awake()
    {
        SetBounds();
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
        enemySystem.manager.Init += () => {
            for (int i = 0; i < initialNumberOfEnemies; i++)
            {
                EnemyData data = EnemyData.GenerateData();
                enemySystem.manager.AddObjectToCollection(data.Type, data);
            }
        };
        enemySystem.manager.Update += () => { 
            ManageEnemies(); 
            if(enemySystem.manager.Collection.Count < 10)
            {
                EnemyData data = EnemyData.GenerateData();
                enemySystem.manager.AddObjectToCollection(data.Type, data);
            }
        };
        enemySystem.manager.PhysicsUpdate += () => { };
    }

    private void ManageEnemies()
    {

        List<Enemy> toDelete = new List<Enemy>();

        foreach (var item in enemySystem.manager.Collection)
        {
            if (!CheckBounds(item))
            {
                toDelete.Add(item);
            }
        }

        for (int i = toDelete.Count - 1; i >= 0; i--)
        {
            enemySystem.manager.RemoveObjectFromCollection(toDelete[i].enemyData.Type, toDelete[i]);
            
        }
    }

    private bool CheckBounds<T>(T item) where T : MonoBehaviour
    {
        bool isVisible = true;

        if (item.transform.position.x < -screenWidth / 2)
            isVisible = false;
        else if (item.transform.position.x > screenWidth / 2)
            isVisible = false;
        else if (item.transform.position.y < -screenHeight / 2)
            isVisible = false;
        else if (item.transform.position.y > screenHeight / 2)
            isVisible = false;

        return isVisible;
    }
}

