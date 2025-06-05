using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public PauseMenu pauseMenu;
    public PlayerInfoManager pif;
    public WaveSystemInfo waveSystemInfo;

    public GameObject GameWinCanvas, GameLostCanvas, PauseMenuCanvas;


    // Start is called before the first frame update
    void Start()
    {
        pauseMenu = GetComponent<PauseMenu>();
        
    }


    public void GameWin()
    {
        pif.SendWinGameRequest(LevelInfoManager.instance.accountId, waveSystemInfo.score, SceneManager.GetActiveScene().name);

        pauseMenu.inputs.pause = true;
        pauseMenu.PauseGame();
        PauseMenuCanvas.SetActive(false);
        GameWinCanvas.SetActive(true);

    }

    public void GameLost()
    {

        //pif.SendWinGameRequest(LevelInfoManager.instance.accountId, waveSystemInfo.score, SceneManager.GetActiveScene().name);

        pauseMenu.inputs.pause = true;
        pauseMenu.PauseGame();
        PauseMenuCanvas.SetActive(false);
        GameLostCanvas.SetActive(true);

    }

}
