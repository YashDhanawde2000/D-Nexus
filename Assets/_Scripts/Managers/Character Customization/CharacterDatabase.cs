using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CharacterDatabase : ScriptableObject
{

    public List<Character> characterList;

    public int characterCount
    {
        get {  return characterList.Count; }
    }

    public Character GetCharacter (int index)
    {
        return characterList[index];
    }

}
