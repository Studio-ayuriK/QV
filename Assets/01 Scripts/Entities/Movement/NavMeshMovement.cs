using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshMovement : MonoBehaviour, IEnemyMovement
{
    private NavMeshAgent agent;
    [Tooltip("¸ØÃß´Â °Å¸®")]
    [SerializeField] private float stoppingDistance = 1.5f;
    public bool HasReachedDestination =>
        !agent.pathPending && agent.hasPath && agent.remainingDistance <= agent.stoppingDistance;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = stoppingDistance;
    }

    public void SetDestination(Vector3 destination)
    {
        if(!agent.isOnNavMesh) return;

        agent.isStopped = false;
        agent.SetDestination(destination);
    }
    
    public void Resume()
    {
        if (agent.isOnNavMesh)
            agent.isStopped = false;
    }

   
    public void Stop()
    {
        if(agent.isOnNavMesh)
            agent.isStopped = true;
    }
}
