using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(
   AttributeTargets.Field |
   AttributeTargets.Property)]
public class PropertyDataSizeAttribute : Attribute
{
    public float width;
    public float height;

    public PropertyDataSizeAttribute(float width, float height)
    {
        this.width = width;
        this.height = height;
    }
}
