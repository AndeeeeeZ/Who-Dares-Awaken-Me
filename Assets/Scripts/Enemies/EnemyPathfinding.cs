using UnityEngine;
using UnityEngine.AI;

public class EnemyPathfinding : MonoBehaviour
{
    private Tombstone target;
    private GameController gameController;

    [SerializeField]
    private float intervalPerCheckTarget;
    private NavMeshAgent agent;
    private float timer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= intervalPerCheckTarget)
        {
            if (target == null || target.GetCurrentDurability() <= 0)
                target = gameController != null ? gameController.GetEnemyTarget() : null;

            if (agent.enabled && target != null)
                agent.SetDestination(target.transform.position);
            timer %= intervalPerCheckTarget; 
        }
    }

    public void SetTarget(Tombstone t, GameController controller)
    {
        target = t;
        gameController = controller;
        if (agent.enabled && target != null)
            agent.SetDestination(target.transform.position);
    }
}
