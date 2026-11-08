using UnityEngine;

[RequireComponent(typeof(EnemyDetector))]
[RequireComponent(typeof(EnemyCombat))]
public class EnemyController : MonoBehaviour
{
    #region Enemy AI
    public Enemy Enemy { get; private set; }
    public EnemyDetector Detector { get; private set; }     
    public IEnemyMovement Movement { get; private set; }
    public EnemyStateMachine StateMachine { get; private set; }
    public EnemyCombat Combat { get; private set; }
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
        Combat = GetComponent<EnemyCombat>();   
        Movement = GetComponent<NavMeshMovement>();

        // 필수 컴포넌트 확인
        if (Enemy == null || Detector == null ||
            Combat == null || Movement == null)
        {
            Debug.LogError($"{name}: 필요한 컴포넌트가 없습니다.", this);

            enabled = false;
            return;
        }

        if (!Enemy.Initialize(this))
        {
            enabled = false;
            return;
        }

        if(!Combat.Initialize(Enemy))
        {
            enabled = false;
            return;
        }

        // 상태 머신 초기화
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
