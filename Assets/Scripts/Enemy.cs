using UnityEngine;
using UnityEngine.Animations;

public class Enemy : MonoBehaviour
{
    private EnemySpawner spawner;
    private EnemyPathfinding pathfinding;

    private void Start()
    {
        pathfinding = GetComponent<EnemyPathfinding>();
    }

    public void FindTarget(GameObject target)
    {
        if (target != null)
        {
            pathfinding.SetTarget(target);
        }
    }
    
    public void SetSpawner(EnemySpawner parent)
    {
        spawner = parent; 
    }
}
