using UnityEngine;
using UnityEngine.AI;

public class EnemyPathfinding : MonoBehaviour
{
    [SerializeField]
    private GameObject target;

    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>(); 
    }
    private void Update()
    {
        agent.SetDestination(target.transform.position); 
    }
}
