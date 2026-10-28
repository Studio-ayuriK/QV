using UnityEngine;

public class EnemyController : MonoBehaviour
{
    #region Enemy AI
    public Enemy Enemy { get; private set; }

    public EnemyDetector Detector { get; private set; }     
    public EnemyCombat Combat { get; private set; }
    public IEnemyMovement Movement { get; private set; }

    #endregion

    public EnemyStateMachine StateMachine { get; private set; }

    [Header("Enmey Data")]
    [SerializeField] private EnemyDataSO enemyData;
    public EnemyDataSO Data => enemyData;

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

        Enemy.Initialize(this);
        Combat.Initialize(Data);

        if (Movement is NavMeshMovement navMeshMovement) navMeshMovement.Initialize(Data);

        StateMachine = new EnemyStateMachine();

        IdleState = new EnemyIdleState(this, StateMachine);
        ChaseState = new EnemyChaseState(this, StateMachine);
        AttackState = new EnemyAttackState(this, StateMachine);
        DeadState = new EnemyDeadState(this, StateMachine);

    }

    private void Start()
    {
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
