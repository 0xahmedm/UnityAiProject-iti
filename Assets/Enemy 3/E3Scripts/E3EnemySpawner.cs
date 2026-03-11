using UnityEngine;
using UnityEngine.SceneManagement;

public class E3EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public GameObject player;

    public int minSpawn = 1;
    public int maxSpawn = 3;

    public int maxKills = 5;
    private int killCount = 0;
    private int activeEnemies = 0;

    void Start()
    {
        enemyPrefab.SetActive(false); // hide the original in scene
        SpawnEnemy();
    }

    public void RegisterKill()
    {
        killCount++;
        activeEnemies--;

        if (killCount >= maxKills)
        {
            if (activeEnemies <= 0)
            {
                SceneManager.LoadScene("Zombie4Final");
            }
            return;
        }

        SpawnEnemy();
    }

    public void SpawnEnemy()
    {
        if (killCount >= maxKills) return;

        int amount = Random.Range(minSpawn, maxSpawn + 1);

        for (int i = 0; i < amount; i++)
        {
            int rand = Random.Range(0, spawnPoints.Length);
            GameObject clone = Instantiate(enemyPrefab, spawnPoints[rand].position, spawnPoints[rand].rotation);
            clone.SetActive(true); // activate the clone

            var health = clone.GetComponent<E3EnemyHealth>();
            if (health != null)
            {
                health.health = 100;
                health.isDead = false;
                health.spawner = this;
                health.isClone = true; // mark as clone so it can be destroyed
            }

            activeEnemies++;
        }
    }
}