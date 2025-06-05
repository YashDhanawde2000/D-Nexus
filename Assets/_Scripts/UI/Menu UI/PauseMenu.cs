using Cinemachine;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;

    public StarterAssetsInputs inputs;


    public bool IsPaused {  get; private set; }


    private float originalTimeScale = 1f;
    public List<GameObject> gameObjectsToDisable;
    public CinemachineBrain mainCamCinemachineBrain;


    void Awake()
    {
        
        //if (healthSystem == null || waveSpawner == null)
        //{
        //    Debug.LogWarning("Add  Script Reference to the Pause Menu");
        //}

    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;

        if (instance == null)
        {
            instance = this;
        }

        GameObject.FindGameObjectWithTag("Player").TryGetComponent(out StarterAssetsInputs _inputs);
        inputs = _inputs;

        if (inputs == null || mainCamCinemachineBrain == null)
        {
            Debug.LogWarning("Add  Script Reference to the Pause Menu");
        }
        else
        {
            mainCamCinemachineBrain.enabled = true;
        }

        foreach (GameObject go in gameObjectsToDisable)
        {
            go.SetActive(false);
        }
    }

    private void Update()
    {
        if (inputs.pause)
        {
            if (!IsPaused)
            {
                PauseGame();
                
            }
        }

        else if (inputs.resume)
        {
            if (IsPaused)
            {
                ResumeGame();
                
            }
            
        }
    }

    public void PauseGame()
    {
        mainCamCinemachineBrain.enabled = false;
        ThirdPersonController.playerInput.SwitchCurrentActionMap("UI");

        originalTimeScale = Time.timeScale;
        Time.timeScale = 0;

        foreach (GameObject go in gameObjectsToDisable)
        {
            go.SetActive(true);
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        inputs.pause = false;
        inputs.resume = false;
        IsPaused = true;
    }

    public void ResumeGame()
    {
        mainCamCinemachineBrain.enabled = true;

        ThirdPersonController.playerInput.SwitchCurrentActionMap("Player");
        
        Time.timeScale = originalTimeScale;

        foreach (GameObject go in gameObjectsToDisable)
        {
            go.SetActive(false);
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        inputs.pause = false;
        inputs.resume = false;
        inputs.shoot = false;
        IsPaused = false;
    }

    public void RestartGame()
    {
        ResumeGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("_MainMenu");
    }


    public void QuitGame()
    {
        Application.Quit();
    }

}
