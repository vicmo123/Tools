using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ResetableAttribute : Attribute
{
    public object resetVal;
    public ResetableAttribute(object resetVal)
    {
        this.resetVal = resetVal;
    }
}
