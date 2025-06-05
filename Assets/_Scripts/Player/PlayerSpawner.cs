using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public CharacterDatabase CharacterDB;

    public List<GameObject> playerPrefabList;

    public int characterID = 0;

    // Start is called before the first frame update
    void Awake()
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

    public void UpdateCharacter(int characterID)
    {
        foreach (GameObject playerPrefab in playerPrefabList)
        {
            playerPrefab.SetActive(false);
        }
        playerPrefabList[characterID].SetActive(true);
    }

    private void LoadCharacter()
    {
        characterID = PlayerPrefs.GetInt("CharacterID");
    }
}
