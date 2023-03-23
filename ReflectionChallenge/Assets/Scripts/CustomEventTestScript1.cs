using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomEventTestScript1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [CustomEvent()]
    public void MakeCatNoise()
    {
        Debug.Log("Miaou");
    }
}
