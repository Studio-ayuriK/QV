using UnityEngine;

public class EnemyController : MonoBehaviour
{
    #region Enemy AI
    public EnemyDetector Detector { get; private set; }     // �÷��̾� Ž����
    public IEnemyMovement Movement { get; private set; }    //  Enemy ������ ����
    public EnemyCombat Combat { get; private set; }         //  Enemy ��������
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
        Detector = GetComponent<EnemyDetector>();
        Combat = GetComponent<EnemyCombat>();
        Movement = GetComponent<NavMeshMovement>();

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
}
