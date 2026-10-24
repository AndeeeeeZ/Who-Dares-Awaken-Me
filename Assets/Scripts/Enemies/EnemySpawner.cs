using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject target;
    [SerializeField, Min(0.01f)] private float spawnInterval = 1f;
    [SerializeField] private Transform[] spawnPoints;

    public float SpawnInterval => Mathf.Max(0.01f, spawnInterval);
    public bool IsConfigured => enemyPrefab != null && enemyPrefab.GetComponent<Enemy>() != null &&
                                target != null && spawnPoints != null && spawnPoints.Length > 0 &&
                                HasOnlyAssignedSpawnPoints();

    public Enemy SpawnEnemy()
    {
        if (!IsConfigured)
        {
            Debug.LogError("EnemySpawner needs an Enemy prefab, a target, and assigned spawn points.", this);
            return null;
        }

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject instance = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity, transform);
        Enemy enemy = instance.GetComponent<Enemy>();
        enemy.FindTarget(target);
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
