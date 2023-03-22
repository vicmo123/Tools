using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverTestScript2 : MonoBehaviour
{
    [CallOnGameOver()]
    public void DestroyGameObject()
    {
        Debug.Log("Destroy Object" + gameObject.name);
    }

    [CallOnGameOver()]
    public void Save()
    {
        Debug.Log("Save");
    }
}
