using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveSystemInfo : MonoBehaviour
{
    [SerializeField]
    private WaveSpawner waveSpawner;

    public TextMeshProUGUI enemiesRemainingText, waveNumberText;
    public TextMeshProUGUI scoreText;

    public int score = 0;

 

    // Update is called once per frame
    void LateUpdate()
    {
        if (waveSpawner != null)
        {
            if (enemiesRemainingText != null)
            {
                enemiesRemainingText.text = ": " + waveSpawner.spawnedEnemies.Count;
            }

            if (waveNumberText != null)
            {
                if (waveNumberText.gameObject.activeSelf)
                {
                    waveNumberText.text = "Wave: " + waveSpawner.currWave;
                }
                
            }

            if (scoreText != null)
            {
                scoreText.text = "Score: " + score;
            }

        }
    }

    public void UpdateScore(int enemyPoints)
    {
        score += enemyPoints;
    }

}
