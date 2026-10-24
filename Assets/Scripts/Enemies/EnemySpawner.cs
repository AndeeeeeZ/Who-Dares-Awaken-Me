using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField, Min(0.01f)] private float spawnInterval = 1f;
    [SerializeField] private Transform[] spawnPoints;

    public float SpawnInterval => Mathf.Max(0.01f, spawnInterval);
    public bool IsConfigured => enemyPrefab != null && enemyPrefab.GetComponent<Enemy>() != null &&
                                spawnPoints != null && spawnPoints.Length > 0 &&
                                HasOnlyAssignedSpawnPoints();

    public Enemy SpawnEnemy(GameController gameController)
    {
        if (!IsConfigured || gameController == null)
        {
            Debug.LogError("EnemySpawner needs an Enemy prefab, assigned spawn points, and a GameController.", this);
            return null;
        }

        Tombstone target = gameController.GetEnemyTarget();
        if (target == null)
            return null;

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject instance = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity, transform);
        Enemy enemy = instance.GetComponent<Enemy>();
        enemy.FindTarget(target, gameController);
        return enemy;
    }

    private bool HasOnlyAssignedSpawnPoints()
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint == null)
                return false;
        }

        return true;
    }
}
