using UnityEngine;

public class EnemyController : MonoBehaviour
{
    #region Enemy AI
    public EnemyDetector Detector { get; private set; }     // �÷��̾� Ž����
    public IEnemyMovement Movement { get; private set; }    //  Enemy ������ ����
    public EnemyCombat Combat { get; private set; }         //  Enemy ��������
    #endregion

    public EnemyStateMachine StateMachine { get; private set; }

    #region Enemy State
    public IdleState IdleState { get; private set; }        // ��� �aㅁㄴㅇㄹ
    public ChaseState ChaseState { get; private set; }      // �߰� ����
    public AttackState AttackState { get; private set; }    // ���� ����
    public DeadState DeadState { get; private set; }        // ���� ����


    #endregion

    private void Awake()
    {
        Detector = GetComponent<EnemyDetector>();
        Combat = GetComponent<EnemyCombat>();
        Movement = GetComponent<NavMeshMovement>();

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
