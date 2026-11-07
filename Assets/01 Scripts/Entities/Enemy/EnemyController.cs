using UnityEngine;

public class EnemyController : MonoBehaviour
{
    #region Enemy AI
    public Enemy Enemy { get; private set; }
    public EnemyDetector Detector { get; private set; }     
    public IEnemyMovement Movement { get; private set; }
    public EnemyStateMachine StateMachine { get; private set; }

    #endregion


    #region Enemy State
    public EnemyIdleState IdleState { get; private set; }        // 대기
    public EnemyChaseState ChaseState { get; private set; }      // 추격
    public EnemyAttackState AttackState { get; private set; }    // 공격
    public EnemyDeadState DeadState { get; private set; }        // 죽음
    #endregion

    private void Awake()
    {
        Enemy = GetComponent<Enemy>();
        Detector = GetComponent<EnemyDetector>();
        Movement = GetComponent<NavMeshMovement>();

        if (!Enemy.Initialize(this))
        {
            enabled = false;
            return;
        }

        StateMachine = new EnemyStateMachine();

        IdleState = new EnemyIdleState(this, StateMachine);
        ChaseState = new EnemyChaseState(this, StateMachine);
        AttackState = new EnemyAttackState(this, StateMachine);
        DeadState = new EnemyDeadState(this, StateMachine);

    }

    private void Start()
    {
        if (Movement is NavMeshMovement navMeshMovement) navMeshMovement.Initialize(Enemy.MoveSpeed);

        StateMachine.Initialize(IdleState);
    }

    private void Update()
    {
        StateMachine.Update();
    }
    

    // 죽은 상태 호출
    public void EnterDeadState()
    {
        if (StateMachine == null || DeadState == null)
            return;

        StateMachine.ChangeState(DeadState);
    }
}
