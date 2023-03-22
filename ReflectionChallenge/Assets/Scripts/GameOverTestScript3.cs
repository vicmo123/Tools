using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverTestScript3 : MonoBehaviour
{
    [CallOnGameOver()]
    public void CleanUp()
    {
        Debug.Log("Clean Up");
    }
}
