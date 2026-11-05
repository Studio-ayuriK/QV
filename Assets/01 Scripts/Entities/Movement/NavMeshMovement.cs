using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshMovement : MonoBehaviour, IEnemyMovement
{
    private NavMeshAgent agent;

    [Tooltip("멈추는 거리")]
    [SerializeField] private float stoppingDistance = 1.5f;
    public bool HasReachedDestination =>
        agent != null &&
        agent.isOnNavMesh &&
        !agent.pathPending &&
        agent.hasPath &&
        agent.remainingDistance <= agent.stoppingDistance;

    private void Awake()
    {
        InitializeAgent();
    }

    private void InitializeAgent()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (agent != null)
            agent.stoppingDistance = stoppingDistance;
    }
    // Enemy에서 전달받은 이동 속도 적용
    public void Initialize(float moveSpeed)
    {
        InitializeAgent();

        if (agent == null)
        {
            Debug.LogError($"{name}: NavMeshAgent를 찾을 수 없습니다.", this);
            return;
        }

        agent.speed = moveSpeed;
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
