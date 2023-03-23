using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CustomEventSystem 
{
    private static System.Action actionsToPerform = () => { };
     
    public static void Register(System.Action toAdd)
    {
        actionsToPerform += toAdd;
    }

    public static void Unregister(System.Action toRemove)
    {
        actionsToPerform -= toRemove;
    }

    public static void ClearAllEvents()
    {
        actionsToPerform = () => { }; 
    }

    public static void InvokeAllEvents()
    {
        actionsToPerform.Invoke();
    }
}
