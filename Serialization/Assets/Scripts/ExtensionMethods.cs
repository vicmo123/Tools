using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class ExtensionMethods
{
    public static G[] FromTtoG<T, G>(this IEnumerable<T> collection, Func<T, G> func)
    {
        List<G> resultList = new List<G>();
        foreach (T item in collection)
        {
            resultList.Add(func.Invoke(item));
        }
        return resultList.ToArray();
    }
}
