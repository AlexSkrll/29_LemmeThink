using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    private bool isSpawning = true;
    [SerializeField] private Vector2 spawnAreaSize;
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
            Vector2 randomPos = new Vector2(transform.position.x + Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f),
                                            transform.position.y + Random.Range(-spawnAreaSize.y / 2f, spawnAreaSize.y / 2f));

            GameObject newEnemy = Instantiate(enemyPrefab, randomPos, Quaternion.identity, transform);
            
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

    private void OnDrawGizmosSelected()
    {
        Color spawnAreaColor = Color.red;

        Gizmos.color = spawnAreaColor;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnAreaSize.x, spawnAreaSize.y, 1f));
    }
}
