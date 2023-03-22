using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverTestScript1 : MonoBehaviour
{
    [CallOnGameOver()]
    public void Winner()
    {
        Debug.Log(gameObject.name + " has won the game");
    }
}
