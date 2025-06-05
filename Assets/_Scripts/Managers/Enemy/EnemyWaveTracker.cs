using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveTracker : MonoBehaviour
{
    public int enemyScorePoints = 1;

    // put this on your enemy prefabs. You could just copy the on destroy onto a pre-existing script if you want.
    void OnDestroy()
    {
        if (GameObject.FindGameObjectWithTag("WaveSpawner") != null)
        {
            GameObject[] hudObjects = GameObject.FindGameObjectsWithTag("HUD");
            foreach (GameObject hudObject in hudObjects)
            {
                
                WaveSystemInfo waveSystemInfoComponent;
                if (hudObject.TryGetComponent<WaveSystemInfo>(out waveSystemInfoComponent))
                {
                    waveSystemInfoComponent.UpdateScore(enemyScorePoints);
                }
            }

            
            GameObject.FindGameObjectWithTag("WaveSpawner").GetComponent<WaveSpawner>().spawnedEnemies.Remove(gameObject);
        }

    }
}