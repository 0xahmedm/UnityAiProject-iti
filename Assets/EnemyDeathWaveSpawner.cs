using UnityEngine;
using System.Collections;

public class EnemyDeathWaveSpawner : MonoBehaviour
{
    [Header("Health Reference")]
    public MonoBehaviour healthScript;  

    [Header("Enemy To Spawn")]
    public GameObject enemyPrefab;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Wave Settings")]
    public int numberOfWaves = 2;
    public int enemiesPerWave = 3;
    public float timeBetweenWaves = 3f;

    private bool wavesStarted = false;

    void Update()
    {
        if (wavesStarted) return;

        if (healthScript != null)
        {
            // Check if the health script has "isDead"
            var field = healthScript.GetType().GetField("isDead");

            if (field != null)
            {
                bool isDead = (bool)field.GetValue(healthScript);

                if (isDead)
                {
                    wavesStarted = true;
                    StartCoroutine(SpawnWaves());
                }
            }
        }
    }

    IEnumerator SpawnWaves()
    {
        for (int w = 0; w < numberOfWaves; w++)
        {
            for (int i = 0; i < enemiesPerWave; i++)
            {
                SpawnEnemy();
            }

            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    void SpawnEnemy()
    {
        if (spawnPoints.Length == 0 || enemyPrefab == null)
            return;

        int rand = Random.Range(0, spawnPoints.Length);

        Instantiate(
            enemyPrefab,
            spawnPoints[rand].position,
            spawnPoints[rand].rotation
        );
    }
}