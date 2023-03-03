using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    // Start is called before the first frame update

    private void Awake()
    {
        //ClassTesting();
    }

    void Start()
    {
        TestingNo1();
        TestingNo2();
        TestingNo3();
        TestingNo4();
        TestingNo5();
        TestingNo6();
        TestingNo7();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ClassTesting()
    {
        Vector3 v3 = new Vector3(3, 4, 5);
        float average = v3.Average();
        Debug.Log(average);

        int[] arr = new int[] { -5, 5, 2, -10 };
        int[] abs = arr.Absolute();
        foreach (var item in abs)
        {
            Debug.Log(item);
        }

        float[] arr2 = new float[] { 0.5f, -0.2f, 1.5f };
        float[] arr3 = arr2.RunDelegateOnEach((val) => { return (float)Mathf.CeilToInt(val); });
    }

    private void TestingNo1()
    {
        float randomValue = ExtensionFuncs.RandomValueVector2(new Vector2(4, 5));
        float randomValue1 = ExtensionFuncs.RandomValueVector2(new Vector2(5, 3));

        Debug.Log("Good order = " + randomValue + " - Bad order = " + randomValue1);
    }

    private void TestingNo2()
    {
        float maxSpeed = 5f;
        Vector3 currentVelocity = Vector3.forward * 3f;

        Rigidbody rb = GetComponent<Rigidbody>();

        //Under max speed
        rb.velocity = currentVelocity;
        Debug.Log("Before : " + rb.velocity + " - magnitude : " + rb.velocity.magnitude);
        rb.LimitSpeedToTreshold(maxSpeed);
        Debug.Log("After : " + rb.velocity + " - magnitude : " + rb.velocity.magnitude);

        //over max speed
        rb.velocity = currentVelocity * 15f;
        Debug.Log("Before : " + rb.velocity + " - magnitude : " + rb.velocity.magnitude);
        rb.LimitSpeedToTreshold(maxSpeed);
        Debug.Log("After : " + rb.velocity + " - magnitude : " + rb.velocity.magnitude);
    }

    private void TestingNo3()
    {
        string label = "Les fruits";
        string[] arr = new string[] { "Pomme", "Raisin", "Poire", "Banane" };
        Debug.Log(arr.AppendStringsWithLabel(label));
    }

    private void TestingNo4()
    {
        string label = "The Vector3s";
        Vector3[] arr = new Vector3[] { new Vector3(0,1,0), new Vector3(5, 5, 5) , new Vector3(-2, 0, 0) , new Vector3(0, 0.75f, 0) };
        GameObject[] arr1 = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log(arr.AppendArrayWithLabel(label));
        Debug.Log(arr1.AppendArrayWithLabel(label, true));
    }

    private void TestingNo5()
    {
        List<Vector2> arr = new List<Vector2> { new Vector2(2, 1), new Vector2(5, 5), new Vector2(0, 0) };
        HashSet<string> arr1 = new HashSet<string> { "a", "b", "c", "d", "e" };
        Debug.Log(arr.ContainsValue(new Vector2(5,5)).ToString());
        Debug.Log(arr1.ContainsValue("d").ToString() + " - " + arr1.ContainsValue("D").ToString());
    }

    private void TestingNo6()
    {
        float maxLength = 3f;
        List<Vector2> arr = new List<Vector2> { new Vector2(2, 1), new Vector2(5, 5), new Vector2(1, 0) };
        bool result = arr.RunPredicate((val) => { return val.magnitude < maxLength; });
        bool result1 = arr.RunPredicate((val) => { return val.magnitude > Vector2.zero.magnitude; });

        Debug.Log(result + " - " + result1);
    }

    private void TestingNo7()
    {
        GameObject[] arrayOfGameObjects = GameObject.FindGameObjectsWithTag("Player");
        List<Rigidbody> arrayOfRigidbodiesIHave = new List<Rigidbody>();

        foreach (var item in arrayOfGameObjects)
        {
            arrayOfRigidbodiesIHave.Add(item.GetComponent<Rigidbody>());
        }

        Vector3[] velocitiesOfEachRb = arrayOfRigidbodiesIHave.FromTtoG((rb) => { return rb.velocity; });
        Vector3[] positionsOfGameObjects = arrayOfGameObjects.FromTtoG((go) => { return go.transform.position; });

        Debug.Log(velocitiesOfEachRb.AppendArrayWithLabel("Rigid body velocities",true));
        Debug.Log(positionsOfGameObjects.AppendArrayWithLabel("Positions", true));
    }
}
