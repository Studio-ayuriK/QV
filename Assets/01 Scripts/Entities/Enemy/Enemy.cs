using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyController))]
public class Enemy : SkeletalEntity
{
    #region Enemy Data

    [Header("Enemy Data")]
    [SerializeField] private EnemyDataSO enemyData;

    private int attackDamage;

    private float enemyMoveSpeed;
    private float attackRange;
    private float attackCooldown;

    public int AttackDamage => attackDamage;
    public float MoveSpeed => enemyMoveSpeed;
    public float AttackRange => attackRange;
    public float AttackCooldown => attackCooldown;
    #endregion

    private EnemyController enemyController;
    private bool isInitialized;

    [Header("Gizmo Settings")]
    [SerializeField]
    private Color attackRangeColor = Color.cyan;

    // 초기화
    public bool Initialize(EnemyController controller)
    {
        if (isInitialized)
            return true;

        if (controller == null || enemyData == null)
        {
            Debug.LogError(
                $"{name}: EnemyController 또는 EnemyDataSO가 없습니다.", this );

            return false;
        }

        enemyController = controller;

        if (_animator == null)
        {
            _animator = GetComponent<Animator>();

            if (_animator == null)
                _animator = GetComponentInChildren<Animator>();
        }

        if (_animator == null)
        {
            Debug.LogError(
                $"{name}: Animator가 없습니다.", this );

            return false;
        }

        EnemyCopyData();

        // 부모 클래스의 공통 체력 초기화
        InitializeHealth(enemyData.MaxHealth);

        isInitialized = true;

        return true;
    }

    // SO 데이터 복사
    private void EnemyCopyData()
    {
        attackDamage = enemyData.AttackDamage;

        enemyMoveSpeed = enemyData.MoveSpeed;
        attackRange = enemyData.AttackRange;
        attackCooldown = enemyData.AttackCooldown;
    }

    // 사망 처리
    protected override void Die()
    {
        if (IsDead) return;

        base.Die();

        // 몬스터 FSM을 죽은 상태로 전환
        enemyController?.EnterDeadState();
    }

    public override void TakeDamage(int damage)
    {
        if(IsDead || damage <= 0) return;

        int previousHealth = CurrentHealth;

        // 공통 체력 감소 처리
        base.TakeDamage(damage);

        // 체력이 감소하지 않았거나 사망한 경우
        if (CurrentHealth >= previousHealth || IsDead) return;

        // 피격 상태 진입
        enemyController?.EnterHitState();
    }

    private void OnDrawGizmosSelected()
    {
        if (enemyData == null)
            return;

        DrawAttackRange(
            transform.position,
            enemyData.AttackRange
        );
    }

    private void DrawAttackRange(Vector3 center, float radius)
    {
        Gizmos.color = attackRangeColor;

        int segments = 48;

        Vector3 previousPoint =
            center + new Vector3(radius, 0f, 0f);

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;

            Vector3 nextPoint = center + new Vector3(
                Mathf.Cos(angle) * radius,
                0f,
                Mathf.Sin(angle) * radius
            );

            Gizmos.DrawLine(previousPoint, nextPoint);

            previousPoint = nextPoint;
        }
    }
}