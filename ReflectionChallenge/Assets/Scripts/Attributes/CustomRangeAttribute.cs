using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Field)]
public class CustomRangeAttribute : Attribute
{
    public int MinValue { get; set; }
    public int MaxValue { get; set; }

    public CustomRangeAttribute(int minValue, int maxValue)
    {
        if (minValue > maxValue)
            Debug.LogError("Min value cannot be greater than max value");

        this.MinValue = minValue;
        this.MaxValue = maxValue;
    }

    public bool CheckIfValid(int value)
    {
        Debug.Log(value);
        return value >= MinValue && value <= MaxValue;
    }

    public int ClampValue(int value)
    {
        return Mathf.Clamp(value, MinValue, MaxValue);
    }
}
