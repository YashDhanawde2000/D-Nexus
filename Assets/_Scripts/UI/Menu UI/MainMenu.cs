using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuPanel, charSelectPanel;


    void Awake()
    {
        Time.timeScale = 1.0f;
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void GoToCharSelectMenu()
    {
        mainMenuPanel.SetActive(false);
        charSelectPanel.SetActive(true);
    }

    public void GoBack()
    {
        mainMenuPanel.SetActive(true);
        charSelectPanel.SetActive(false);
    }
}
