using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public GameOverMenu gameOverMenu;

    public GameObject waveNumberDisplay;
    public float waveNumPopupDuration = 3f;

    public List<Enemy> enemies = new List<Enemy>();
    public int currWave;
    public int maxSpawnableEnemies = 30;
    private int waveValue;
    private List<GameObject> enemiesToSpawn = new List<GameObject>();

    public Transform[] spawnLocation;
    public int spawnIndex;

    public int waveDuration;
    private float waveTimer;
    private float spawnInterval;
    private float spawnTimer;

    public List<GameObject> spawnedEnemies = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        GenerateWave();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (spawnTimer <= 0)
        {
            //spawn an enemy
            if (enemiesToSpawn.Count > 0)
            {
                GameObject enemy = (GameObject)Instantiate(enemiesToSpawn[0], spawnLocation[spawnIndex].position, Quaternion.identity); // spawn first enemy in our list
                enemiesToSpawn.RemoveAt(0); // and remove it
                spawnedEnemies.Add(enemy);
                spawnTimer = spawnInterval;

                if (spawnIndex + 1 <= spawnLocation.Length - 1)
                {
                    spawnIndex++;
                }
                else
                {
                    spawnIndex = 0;
                }
            }
            else
            {
                waveTimer = 0; // if no enemies remain, end wave
            }
        }
        else
        {
            spawnTimer -= Time.fixedDeltaTime;
            waveTimer -= Time.fixedDeltaTime;
        }

        if (waveTimer <= 0 && spawnedEnemies.Count <= 0)
        {
            currWave++;
            if (currWave > 5)
            {
                //  VICTORY SCREEN!!

                gameOverMenu.GameWin();

            }
            else
            {
                GenerateWave();
            }
            
        }
    }

    public void GenerateWave()
    {
        StartCoroutine(WaveNumPopupCoroutine());

        waveValue = (currWave * 10) ;
        GenerateEnemies();

        spawnInterval = waveDuration / enemiesToSpawn.Count; // gives a fixed time between each enemies
        waveTimer = waveDuration; // wave duration is read only
    }

    public void GenerateEnemies()
    {
        // Create a temporary list of enemies to generate
        // 
        // in a loop grab a random enemy 
        // see if we can afford it
        // if we can, add it to our list, and deduct the cost.

        // repeat... 

        //  -> if we have no points left, leave the loop

        List<GameObject> generatedEnemies = new List<GameObject>();
        while (waveValue > 0 || generatedEnemies.Count < maxSpawnableEnemies)
        {
            int randEnemyId = Random.Range(0, enemies.Count);
            int randEnemyCost = enemies[randEnemyId].cost;

            if (waveValue - randEnemyCost >= 0)
            {
                generatedEnemies.Add(enemies[randEnemyId].enemyPrefab);
                waveValue -= randEnemyCost;
            }
            else if (waveValue <= 0)
            {
                break;
            }
        }
        
        enemiesToSpawn.Clear();
        enemiesToSpawn = generatedEnemies;
    }

    IEnumerator WaveNumPopupCoroutine()
    {
        // Activate the object
        waveNumberDisplay.SetActive(true);

        // Wait for the specified duration
        yield return new WaitForSeconds(waveNumPopupDuration);

        // Deactivate the object after the duration
        waveNumberDisplay.SetActive(false);

    }

}

[System.Serializable]
public class Enemy
{
    public GameObject enemyPrefab;
    public int cost;
}


