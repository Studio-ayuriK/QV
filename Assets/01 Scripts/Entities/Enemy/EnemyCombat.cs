using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 1.5f;

    private EnemyDetector detector;
    private bool isAttacking;
    private float attackTimer;

    public float AttackRange => attackRange;

    private void Awake()
    {
        detector = GetComponent<EnemyDetector>();
    }

    private void Update()
    {
        if (!isAttacking)
            return;

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            PerformAttack();
            attackTimer = attackCooldown;
        }
    }

    public void BeginAttack()
    {
        if (isAttacking)
            return;

        isAttacking = true;
        attackTimer = 0f;
    }

    public void EndAttack()
    {
        isAttacking = false;
    }

    private void PerformAttack()
    {
        Transform target = detector.Target;

        if (target == null)
            return;

        float distance = GetHorizontalDistance(
            transform.position,
            target.position
        );

        // 공격 실행 직전에 사거리 재확인
        if (distance > attackRange)
            return;

        // 대상 또는 상위 오브젝트에서 피해 처리 컴포넌트 검색
        MonoBehaviour[] components =
            target.GetComponentsInParent<MonoBehaviour>();

        foreach (MonoBehaviour component in components)
        {
            if (component is IDamageable damageable)
            {
                damageable.TakeDamage(attackDamage);
                return;
            }
        }
    }

    private float GetHorizontalDistance(Vector3 a, Vector3 b)
    {
        a.y = 0f;
        b.y = 0f;

        return Vector3.Distance(a, b);
    }
}