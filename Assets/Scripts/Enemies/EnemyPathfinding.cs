using UnityEngine;
using UnityEngine.AI;

public class EnemyPathfinding : MonoBehaviour
{
    private HasDurability target;
    private GameController gameController;

    [SerializeField]
    private float intervalPerCheckTarget;
    [SerializeField, Min(0f)]
    private float blockedDoorSearchDistance = 5f;
    private NavMeshAgent agent;
    private NavMeshPath path;
    private Door blockingDoor;
    private HasDurability destinationTarget;
    private Vector3 destinationPosition;
    private float timer;
    public HasDurability CurrentTarget => target;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        path = new NavMeshPath();
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= intervalPerCheckTarget)
        {
            if (blockingDoor != null)
            {
                if (!blockingDoor.IsDefending || blockingDoor.AttackCollider == null)
                {
                    blockingDoor = null;
                    target = gameController != null ? gameController.GetEnemyTarget() : null;
                }
            }
            else if (gameController != null && !gameController.IsEnemyTargetActive(target))
            {
                target = gameController.GetEnemyTarget();
            }

            if (blockingDoor == null && target is Tombstone && agent.enabled && agent.isOnNavMesh)
                FindBlockingDoor();

            UpdateDestination();
            timer %= intervalPerCheckTarget; 
        }
    }

    public void SetTarget(HasDurability t, GameController controller)
    {
        target = t;
        gameController = controller;
        blockingDoor = null;
        // Keep the agent moving to the target edge; Enemy handles contact and door reach.
        agent.stoppingDistance = 0f;
        UpdateDestination();
    }

    public void RefreshDestination()
    {
        destinationTarget = null;
        UpdateDestination();
    }

    private void UpdateDestination()
    {
        if (!agent.enabled || !agent.isOnNavMesh)
            return;

        if (target == null)
        {
            if (destinationTarget != null)
                agent.ResetPath();
            destinationTarget = null;
            return;
        }

        Vector3 position = target.transform.position;
        if (destinationTarget == target &&
            (position - destinationPosition).sqrMagnitude < 0.01f)
            return;

        if (agent.SetDestination(position))
        {
            destinationTarget = target;
            destinationPosition = position;
        }
    }

    private void FindBlockingDoor()
    {
        if (gameController == null)
            return;

        path.ClearCorners();
        bool hasPath = agent.CalculatePath(target.transform.position, path);
        if (hasPath && path.status == NavMeshPathStatus.PathComplete)
            return;

        Vector3 reachablePoint = transform.position;
        if (path.corners.Length > 0)
            reachablePoint = path.corners[path.corners.Length - 1];

        Door nearestDoor = null;
        float nearestDistance = blockedDoorSearchDistance;
        foreach (Door door in gameController.ConfiguredDoors)
        {
            if (door == null || !door.IsDefending || door.AttackCollider == null)
                continue;

            float distance = Vector3.Distance(reachablePoint,
                door.AttackCollider.ClosestPoint(reachablePoint));
            if (distance >= nearestDistance)
                continue;

            nearestDoor = door;
            nearestDistance = distance;
        }

        if (nearestDoor == null)
            return;

        blockingDoor = nearestDoor;
        target = nearestDoor;
    }
}
