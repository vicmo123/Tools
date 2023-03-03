using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CustomCollection<T> where T : IComparable
{
    T[] arr = new T[10];
    int currentIndex = 0;

    public void Add(T toAdd)
    {
        arr[currentIndex] = toAdd;
        currentIndex++;
    }

    public void Remove(int removeAtIndex)
    {
        for (int i = removeAtIndex; i < currentIndex - 1; i++)
        {
            arr[i] = arr[i + 1];
        }
        currentIndex--;
    }

    public int Count()
    {
        return currentIndex;
    }
}
