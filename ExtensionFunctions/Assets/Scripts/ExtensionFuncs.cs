using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class ExtensionFuncs              //static required
{
    //Student requirements
    /*
     * For these exercises. You are not allowed to use ChatGPT or use the ExtensionFunc sheet I gave you
     * My sheet contains the answers
     * ChatGPT will be able to answer these with no problem at all
     * Using stack overflow or other resources is allowed and heavily recommended
     * 
      
    Make an extension function on a Vector2 which returns a random value between x & y
    Make an extension function of rigidbody that given a float arguement, clamps speed at that threshold
    Make an extension function that given a string array & label, returns a single string formated like:
        label: elem1,elem2,elem3
    Make an extension function same as above, but works on an array of any type
    Make an extension function that mimics how .Contains works  
    Make an extension function that runs a predicate on each element and returns if it is true for all elements
    Make an extension function that runs a delegate on a collection of type T, and returns a collection of type G
        Examples:
            Vector3[] velocitiesOfEachRb = arrayOfRigidbodiesIHave.CollectionFrom((rb)=>{ return rb.velocity;});
            Vector3[] positionsOfGameObjects = arrayOfGameObjects.CollectionFrom((go)=>{ return go.position;});
    */

    /*
     Examples shown in class
     An extension function that returns the average of a vector3
     An extension function that absolutes all values in an int array
     An extension function of rigidbody that given a float arguement, returns if speed is above that threshold
     Our own Contains function
     An extension function that runs a delegate on each element in an int array and replaces the int
     An extension function that extends type T array and randomizes the array

     */


    //----------------Class Exemples---------------//
    public static float Average(this Vector3 v3)
    {
        return (v3.x + v3.y + v3.z) / 3;
    }

    public static int[] Absolute(this int[] arr)
    {
        int[] toRet = new int[arr.Length];
        for (int i = 0; i < arr.Length; i++)
        {
            toRet[i] = Mathf.Abs(arr[i]);
        }

        return toRet;
    }

    public static bool SpeedSurpassesTreshold(this Rigidbody rb, float speed)
    {
        return rb.velocity.magnitude > speed;
    }

    public static float[] RunDelegateOnEach(this float[] arr, Func<float, float> delg)
    {
        float[] toRet = new float[arr.Length];
        for (int i = 0; i < arr.Length; i++)
        {
            toRet[i] = delg.Invoke(arr[i]);
        }

        return toRet;
    }

    public static T[] RandomizeArray<T>(this T[] arr)
    {
        //Randomize ...

        return null;
    }

    //----------------Module---------------//

    public static float RandomValueVector2(this Vector2 v2)
    {
        float min, max;

        if (v2.x < v2.y)
        {
            min = v2.x; max = v2.y;
        }
        else
        {
            min = v2.y;
            max = v2.x;
        }

        return Random.Range(min, max);
    }

    public static void LimitSpeedToTreshold(this Rigidbody rb, float maxSpeed)
    {
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, Mathf.Abs(maxSpeed));
    }

    public static String AppendStringsWithLabel(this String[] arr, String label)
    {
        String finalResult = (label + ": ");

        for (int i = 0; i < arr.Length; i++)
            if (i < arr.Length - 1)
                finalResult += (arr[i] + ", ");
            else
                finalResult += arr[i];

        return finalResult;
    }

    public static String AppendArrayWithLabel<T>(this T[] arr, string label, bool withIndex = false)
    {
        String finalResult = (label + ": ");

        for (int i = 0; i < arr.Length; i++)
            if (i == 0)
                finalResult += ((withIndex) ? "[" + (i + 1) + "] = " : "") + arr[i].ToString();
            else
                finalResult += ", " + ((withIndex) ? "[" + (i + 1) + "] = " : "") + arr[i].ToString();

        return finalResult;
    }

    public static bool ContainsValue<T>(this IEnumerable<T> collection, T targetValue)
    {
        foreach (T element in collection)
        {
            if (element.Equals(targetValue))
            {
                return true;
            }
        }

        return false;
    }

    public static bool RunPredicate<T>(this IEnumerable<T> collection, Predicate<T> predicate)
    {
        foreach (T element in collection)
        {
            if (!predicate.Invoke(element))
            {
                return false;
            }
        }

        return true;
    }

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
