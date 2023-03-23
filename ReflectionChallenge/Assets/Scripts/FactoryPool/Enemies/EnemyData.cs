using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class EnemyData
{
    public EnemyTypes Type { get; set; }
    //[Resetable(new Vector3(0,0,0))] doesn not work
    public Vector3 Position { get; set; }
    public Vector2 Direction { get; set; }
    [Resetable(8f)]public float Speed { get; set; }

    public EnemyData(EnemyTypes type, Vector3 position, Vector3 direction, float speed)
    {
        Type = type;
        Position = position;
        Direction = direction;
        Speed = speed;
    }

    private static float minSpeed = 3f;
    private static float maxSpeed = 8f;

    public static EnemyData GenerateData()
    {
        return new EnemyData(GenerateType(), GeneratePosition(), GenerateDirection(), GenerateSpeed());
    }

    private static EnemyTypes GenerateType()
    {
        int numTypes = System.Enum.GetValues(typeof(EnemyTypes)).Length;
        return (EnemyTypes)Random.Range(0, numTypes);
    }

    private static Vector3 GeneratePosition()
    {
        return new Vector3(0, 0, 0);
    }

    private static float GenerateSpeed()
    {
        return Random.Range(minSpeed, maxSpeed);
    }

    private static Vector2 GenerateDirection()
    {
        return Random.insideUnitCircle.normalized;
    }

    public static EnemyData ResetWithAttribute()
    {
        EnemyData data = GenerateData();
        var props = data.GetType().GetProperties();
        foreach (var p in props)
        {
            var attr = p.GetCustomAttribute<ResetableAttribute>();
            if(attr != null)
            {
                p.SetValue(data, attr.resetVal);
            }
        }

        return data;
    }
}
