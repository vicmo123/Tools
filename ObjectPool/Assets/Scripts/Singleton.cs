using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> where T : Singleton<T>, new()
{
    private static T instance;

    protected Singleton() { }

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = System.Activator.CreateInstance(typeof(T), true) as T;
            }
            return instance;
        }
    }
}
