using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{

    public CharacterDatabase CharacterDB;

    public List<GameObject> characterPrefabList;

    public int characterID = 0;


    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("CharacterID"))
        {
            LoadCharacter();
        }
        else
        {
            characterID = 0;
        }

        UpdateCharacter(characterID);
    }

    public void NextCharacter()
    {
        characterID++;

        if (characterID >= CharacterDB.characterCount)
        {
            characterID = 0;
        }
        UpdateCharacter(characterID);
        SaveCharacter();
    }

    public void PreviousCharacter()
    {
        characterID--;
        if (characterID < 0)
        {
            characterID = CharacterDB.characterCount - 1;
        }
        UpdateCharacter(characterID);
        SaveCharacter();
    }

    public void UpdateCharacter(int characterID)
    {
        foreach (GameObject character in characterPrefabList)
        {
            character.SetActive(false);
        }

        characterPrefabList[characterID].SetActive(true);
    }

    private void LoadCharacter()
    {
        characterID = PlayerPrefs.GetInt("CharacterID");
    }

    private void SaveCharacter()
    {
        PlayerPrefs.SetInt("CharacterID", characterID);
    }

}
