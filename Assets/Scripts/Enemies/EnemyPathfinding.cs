using UnityEngine;
using UnityEngine.AI;

public class EnemyPathfinding : MonoBehaviour
{
    private HasDurability target;
    private GameController gameController;

    [SerializeField]
    private float intervalPerCheckTarget;
    private NavMeshAgent agent;
    private float timer;
    public HasDurability CurrentTarget => target;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= intervalPerCheckTarget)
        {
            if (gameController != null && !gameController.IsEnemyTargetActive(target))
                target = gameController.GetEnemyTarget();

            if (agent.enabled && target != null)
                agent.SetDestination(target.transform.position);
            timer %= intervalPerCheckTarget; 
        }
    }

    public void SetTarget(HasDurability t, GameController controller)
    {
        target = t;
        gameController = controller;
        // Impact is triggered by collider contact, including for the leg enemy prefab.
        agent.stoppingDistance = 0f;
        if (agent.enabled && target != null)
            agent.SetDestination(target.transform.position);
    }
}
