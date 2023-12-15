using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    private bool isSpawning = true;
    [SerializeField] private float spawnInterval;
    [SerializeField] private int maxActiveEnemies;
    private int currentActiveEnemies = 0;

    void Start()
    {
        InvokeRepeating("SpawnEnemy", Random.Range(1f, spawnInterval), spawnInterval);
    }

        
    void SpawnEnemy()
    {
        if (currentActiveEnemies < maxActiveEnemies && isSpawning == true)
        {
            GameObject newEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity, transform);
            GameManager.instance.IncrementTotalSpawnedEnemies();
            currentActiveEnemies++;
        }
    }

    public void EnemyDestroyed()
    {
        currentActiveEnemies--;
    }
    
    public void StopSpawners()
    {
        isSpawning = false;
        Debug.Log(isSpawning);
    }

    public void StartSpawners()
    {
        isSpawning = true;
        Debug.Log(isSpawning);
    }
}
