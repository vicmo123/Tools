using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

public class FirstExample : MonoBehaviour
{

    Dictionary<string, MethodInfo> myDict = new Dictionary<string, MethodInfo>();
    BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    [ExposeMethodInEditor()]
    private void RunExamples()
    {
        FieldInfoExample();

        MethodInfoExample();

        PropertyInfoExample();

        Debug.Log(HowManyOfType<int>());
    }

    private void FieldInfoExample()
    {
        Dog firstDog = new Dog("Bob", 5, Color.black, "woof", false);

        Type dogType = typeof(Dog);
        FieldInfo dogNameFieldInfo = dogType.GetField("name", bindingFlags);
        string dogName = dogNameFieldInfo.GetValue(firstDog) as string;
        Debug.Log(dogName);

        FieldInfo dogAgeFieldInfo = dogType.GetField("age", bindingFlags);
        int dogAge = (int)dogAgeFieldInfo.GetValue(firstDog);
        Debug.Log(dogAge);

        firstDog.OutputField("name", bindingFlags);
    }

    private void MethodInfoExample()
    {
        Dog secondDog = new Dog("Joe", 7, Color.black, "woooooof", true);
        
        MethodInfo methodInfo = typeof(Dog).GetMethod("MakeNoise", bindingFlags);
        methodInfo.Invoke(secondDog, new object[] { });

        secondDog.OutputField("name", bindingFlags);

        MethodInfo methodInfo2 = typeof(Dog).GetMethod("ChangeName", bindingFlags);
        Action<string> newDelegate = (Action<string>)methodInfo2.CreateDelegate(typeof(Action<string>), secondDog);
        newDelegate.Invoke("Dimitri");

        secondDog.OutputField("name", bindingFlags);
    }

    private void PropertyInfoExample()
    {
        Dog secondDog = new Dog("Jacob", 3, Color.green, "Miaouwww", true);

        PropertyInfo pi = typeof(Dog).GetProperty("isSecretlyACat", bindingFlags);
        MethodInfo mi1 = pi.GetGetMethod();
        MethodInfo mi2 = pi.GetSetMethod();
    }

    private int HowManyOfType<T>()
    {
        int numberOfT = 0;
        Type[] allType = Assembly.GetExecutingAssembly().GetTypes();

        foreach (var t in allType)
        {
            var fis = t.GetFields(bindingFlags);
            foreach (var fi in fis)
            {
                if(fi.FieldType == typeof(T))
                {
                    numberOfT++;
                }
            }
        }

        return numberOfT;
    }
}

[FindMe("Doggo", 4)]
public class Dog
{
    [FindMe("DoggoName", 40)] public string name;
    private int age;
    [SerializeField] Color furColor;
    [HideInInspector] public string noise;
    public bool isSecretlyACat { get; set; }

    public Dog(string name, int age, Color furColor, string noise, bool isSecretlyACat)
    {
        this.name = name;
        this.age = age;
        this.furColor = furColor;
        this.noise = noise;
        this.isSecretlyACat = isSecretlyACat;
    }

    public void MakeNoise()
    {
        Debug.Log(this.noise);
    }

    private void PrintName()
    {
        Debug.Log(this.name);
    }

    protected void ChangeName(string newName)
    {
        name = newName;
    }

    public bool GetIsSecretlyACat()
    {
        return this.isSecretlyACat;
    }

    protected void ChangeFurColor(Color newColor)
    {
        furColor = newColor;
    }
}

public enum FurColor
{
    Red,
    Brown, 
    Blue, 
    Green
}

public static class ExtensionFunctions
{
    public static void OutputField<T>(this T obj, string fieldName, BindingFlags bindingFlags)
    {
        Type type = typeof(T);
        FieldInfo extractedField = type.GetField(fieldName, bindingFlags);
        Debug.Log(extractedField.GetValue(obj).ToString());
    }
}
