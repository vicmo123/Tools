using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public static class ExtensionMethods
{
    public static IEnumerable<G> FromTtoG<T, G>(this IEnumerable<T> collection, Func<T, G> func)
    {
        List<G> resultList = new List<G>();
        foreach (T item in collection)
        {
            resultList.Add(func.Invoke(item));
        }
        return resultList;
    }

    public static void ClearListMonobehaviours<T>(this List<T> collection) where T : MonoBehaviour
    {
        for (int i = 0; i < collection.Count; i++)
        {
            GameObject.Destroy(collection[i].gameObject);
        }

        collection.Clear();
    }

    public static GameManager.ListWrapper GetWrappedList(this List<ObjectDataManager> objectList)
    {
        return new GameManager.ListWrapper { dataList = (objectList.FromTtoG((val) => { return val.objData; })).ToList() };
    }
}
