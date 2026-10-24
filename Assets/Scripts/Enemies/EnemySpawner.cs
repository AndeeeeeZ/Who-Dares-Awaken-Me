using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab, target;

    [SerializeField]
    private int maxEnemyCount;

    [SerializeField]
    private float spawnInterval;

    [SerializeField]
    private Transform[] spawnPoints;

    private int currentEnemyCount;
    private float timer;

    private void Start()
    {
        currentEnemyCount = 0;
        timer = 0f;
    }

    private void Update()
    {
        Debug.Log(currentEnemyCount); 
        if (currentEnemyCount < maxEnemyCount)
        {
            timer += Time.deltaTime;
            if (timer >= spawnInterval)
            {
                timer %= spawnInterval;
                SpawnEnemy();
            }
        }
    }

    public void SpawnEnemy()
    {
        currentEnemyCount++;
        int i = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[i];
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity, transform);
        enemy.GetComponent<Enemy>().FindTarget(target);
        enemy.GetComponent<Enemy>().SetSpawner(this);
    }

    public void CloneDestroyed()
    { currentEnemyCount--; }
}
