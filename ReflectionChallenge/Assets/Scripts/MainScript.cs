using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScript : MonoBehaviour
{
    private HashSet<string> BannedWordsHash = new HashSet<string>();
    [SerializeField] private string[] BannedWords;

    [StringDropDown("Bob", "Joe", "David", "Roger", "Jeremy")]
    public string FirstName;
    [StringDropDown("Canada", "France", "Mexico", "China", "Japan")]
    public string Country;

    [ExposeMethodInEditor()]
    private void CheckForBadWords()
    {
        for (int i = 0; i < BannedWords.Length; i++)
        {
            BannedWordsHash.Add(BannedWords[i]);
        }

        BannedWordsHash.FindWordsInProject();
    }
}
