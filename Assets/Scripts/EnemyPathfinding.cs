using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPathfinding : MonoBehaviour
{
    [SerializeField]
    private GameObject target;

    [SerializeField]
    private float intervalPerCheckTarget;
    private NavMeshAgent agent;
    private float timer;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= intervalPerCheckTarget)
        {
            agent.SetDestination(target.transform.position);
            timer %= intervalPerCheckTarget; 
        }
    }

    public void SetTarget(GameObject t)
    {
        target = t;
        agent.SetDestination(target.transform.position);
    }
}
