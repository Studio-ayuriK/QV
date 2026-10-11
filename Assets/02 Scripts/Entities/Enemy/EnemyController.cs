using UnityEngine;

public class EnemyController : MonoBehaviour
{
    #region Enemy AI
    public EnemyDetector Detector { get; private set; }     // 플레이어 탐지기
    public IEnemyMovement Movement { get; private set; }    //  Enemy 움직임 상태
    public EnemyCombat Combat { get; private set; }         //  Enemy 전투상태
    #endregion

    public EnemyStateMachine StateMachine { get; private set; }

    #region Enemy State
    public IdleState IdleState { get; private set; }        // 대기 상태
    public ChaseState ChaseState { get; private set; }      // 추격 상태
    public AttackState AttackState { get; private set; }    // 공격 상태
    public DeadState DeadState { get; private set; }        // 죽은 상태


    #endregion

    private void Awake()
    {
        Detector = GetComponent<EnemyDetector>();
        Combat = GetComponent<EnemyCombat>();
        Movement = GetComponent<NavMeshMovement>();

        Debug.Log($"Detector: {Detector}");
        Debug.Log($"Combat: {Combat}");
        Debug.Log($"Movement: {Movement}");

        StateMachine = new EnemyStateMachine();

        IdleState = new IdleState(this, StateMachine);
        ChaseState = new ChaseState(this, StateMachine);
        AttackState = new AttackState(this, StateMachine);
        DeadState = new DeadState(this, StateMachine);
    }

    private void Start()
    {
        StateMachine.Initialize(IdleState);
    }

    private void Update()
    {
        StateMachine.Update();
    }
}
