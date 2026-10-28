using UnityEngine;
using UnityEngine.AI;

public class EnemyPathfinding : MonoBehaviour
{
    private HasDurability target;
    private GameController gameController;

    [SerializeField]
    private float intervalPerCheckTarget;
    [SerializeField, Min(0.1f)]
    private float targetSampleDistance = 8f;
    private NavMeshAgent agent;
    private NavMeshPath path;
    private HasDurability destinationTarget;
    private Vector3 desiredPosition;
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
        if (timer < intervalPerCheckTarget)
            return;

        timer %= intervalPerCheckTarget;
        if (!agent.enabled || !agent.isOnNavMesh)
            return;

        SelectTarget();
        UpdateDestination();
    }

    public void SetTarget(HasDurability initialTarget, GameController controller)
    {
        target = initialTarget;
        gameController = controller;
        agent.stoppingDistance = 0f;

        if (agent.enabled && agent.isOnNavMesh)
        {
            SelectTarget();
            UpdateDestination();
        }
    }

    public void RefreshDestination()
    {
        destinationTarget = null;
        if (!agent.enabled || !agent.isOnNavMesh)
            return;

        SelectTarget();
        UpdateDestination();
    }

    private void SelectTarget()
    {
        if (gameController == null)
            return;

        if (target is Tombstone currentTombstone && currentTombstone.GetCurrentDurability() > 0 &&
            TryGetReachableTombstonePoint(currentTombstone, out desiredPosition))
            return;

        Tombstone nearestTombstone = null;
        Vector3 nearestPoint = default;
        float nearestDistance = float.PositiveInfinity;
        foreach (Tombstone tombstone in gameController.TrackedTombstones)
        {
            if (tombstone == null || tombstone.GetCurrentDurability() <= 0 ||
                !TryGetReachableTombstonePoint(tombstone, out Vector3 point))
                continue;

            float distance = (transform.position - point).sqrMagnitude;
            if (distance >= nearestDistance)
                continue;

            nearestTombstone = tombstone;
            nearestPoint = point;
            nearestDistance = distance;
        }

        if (nearestTombstone != null)
        {
            target = nearestTombstone;
            desiredPosition = nearestPoint;
            return;
        }

        if ((target is Door currentDoor && currentDoor.IsDefending && currentDoor.AttackCollider != null) ||
            (target is Wall currentWall && currentWall.IsDefending && currentWall.AttackCollider != null))
            return;

        SelectClosestDefense();
    }

    private bool TryGetReachableTombstonePoint(Tombstone tombstone, out Vector3 point)
    {
        point = default;
        if (!NavMesh.SamplePosition(tombstone.transform.position, out NavMeshHit hit,
                targetSampleDistance, agent.areaMask))
            return false;

        path.ClearCorners();
        if (!agent.CalculatePath(hit.position, path) ||
            path.status != NavMeshPathStatus.PathComplete)
            return false;

        point = hit.position;
        return true;
    }

    private void SelectClosestDefense()
    {
        HasDurability nearestDefense = null;
        Collider nearestCollider = null;
        float nearestDistance = float.PositiveInfinity;

        foreach (Door door in gameController.ConfiguredDoors)
        {
            if (door == null || !door.IsDefending || door.AttackCollider == null)
                continue;

            float distance = (transform.position - door.AttackCollider.ClosestPoint(transform.position)).sqrMagnitude;
            if (distance >= nearestDistance)
                continue;

            nearestDefense = door;
            nearestCollider = door.AttackCollider;
            nearestDistance = distance;
        }

        foreach (Wall wall in gameController.ConfiguredWalls)
        {
            if (wall == null || !wall.IsDefending || wall.AttackCollider == null)
                continue;

            float distance = (transform.position - wall.AttackCollider.ClosestPoint(transform.position)).sqrMagnitude;
            if (distance >= nearestDistance)
                continue;

            nearestDefense = wall;
            nearestCollider = wall.AttackCollider;
            nearestDistance = distance;
        }

        target = nearestDefense;
        if (nearestCollider == null)
            return;

        Vector3 approachPoint = nearestCollider.ClosestPoint(transform.position);
        desiredPosition = NavMesh.SamplePosition(approachPoint, out NavMeshHit hit,
            targetSampleDistance, agent.areaMask) ? hit.position : approachPoint;
    }

    private void UpdateDestination()
    {
        if (target == null)
        {
            if (destinationTarget != null)
                agent.ResetPath();
            destinationTarget = null;
            return;
        }

        if (destinationTarget == target &&
            (desiredPosition - destinationPosition).sqrMagnitude < 0.01f)
            return;

        if (agent.SetDestination(desiredPosition))
        {
            destinationTarget = target;
            destinationPosition = desiredPosition;
        }
    }
}
