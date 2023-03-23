using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class MainScript : MonoBehaviour
{
    private HashSet<string> BannedWordsHash = new HashSet<string>();
    [SerializeField] private string[] BannedWords;

    [StringDropDown("Bob", "Joe", "David", "Roger", "Jeremy")]
    public string FirstName;
    [StringDropDown("Canada", "France", "Mexico", "China", "Japan")]
    public string Country;

    System.Action GameOverSequence = () => { };

    public int MaxNumberOfParameters = 0;
        
    [CustomRange(10, 100)]
    public int MyIntField1;
    [CustomRange(0, 5)]
    public int MyIntField2;

    [ExposeMethodInEditor()]
    private void CheckForBadWords()
    {
        for (int i = 0; i < BannedWords.Length; i++)
        {
            BannedWordsHash.Add(BannedWords[i]);
        }

        BannedWordsHash.FindWordsInProject();
    }

    [ExposeMethodInEditor()]
    private void CacheGameOverSequence()
    {
        MonoBehaviour[] allObjects = FindObjectsOfType<MonoBehaviour>();

        foreach (MonoBehaviour obj in allObjects)
        {
            GameOverSequence += obj.GetMethodWithAttribute<CallOnGameOverAttribute>();
        }
    }

    [ExposeMethodInEditor()]
    private void GameOverTime()
    {
        GameOverSequence.Invoke();
    }

    [ExposeMethodInEditor()]
    private void ShowcaseCustomRange()
    {
        MonoBehaviour[] allObjects = FindObjectsOfType<MonoBehaviour>();
        allObjects.CheckAndFixCustomRangeValues();
    }

    static string[] choices = ReflectionHelper.GetAllTypeStrings();
    [StringDropDown(true)]
    public string typesToChoseFrom;

    [ExposeMethodInEditor()]
    public void ShowcaseDisplayClassInfo()
    {
        typesToChoseFrom.DisplayClassInfo();
    }

    [ExposeMethodInEditor()]
    public void FindAllMethodsThatHaveOverNParams()
    {
        ReflectionHelper.FindAllFunctionsWithNumberOfParameter(MaxNumberOfParameters);
    }

    [ExposeMethodInEditor()]
    public void LinkAll()
    {
        MonoBehaviour[] allObjects = FindObjectsOfType<MonoBehaviour>();

        foreach (MonoBehaviour obj in allObjects)
        {
            CustomEventSystem.Register(obj.GetMethodWithAttribute<CustomEventAttribute>());
        }
        Debug.Log("Events Linked");
    }

    [ExposeMethodInEditor()]
    public void InvokeEventSystem()
    {
        CustomEventSystem.InvokeAllEvents();
    }
}
