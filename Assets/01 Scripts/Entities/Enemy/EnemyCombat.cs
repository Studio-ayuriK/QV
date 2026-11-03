using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    public enum AttackType
    {
        Melee,
        Ranged
    }

    [Header("Combat Settings")]
    [SerializeField] private float attackRange;
    [SerializeField] private int attackDamage;
    [SerializeField] private float attackCooldown;

    private Enemy enemy;
    private EnemyDetector detector;

    private AttackType attackType;
    private bool isAttacking;
    private bool hasAppliedDamage;

    private float attackTimer;

    public float AttackRange => attackRange;
    public float AttackDamage => attackDamage;

    public void Initialize(EnemyDataSO data, Enemy enemy)
    {
        this.enemy = enemy;
        detector = GetComponent<EnemyDetector>();

        attackDamage = data.AttackDamage;
        attackRange = data.AttackRange;
        attackCooldown = data.AttackCooldown;


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
    // 공격 시작
    public void BeginAttack(AttackType type)
    {
        if (isAttacking)
            return;

        attackType = type;
        isAttacking = true;
        attackTimer = 0f;
    }
    // 공격 종료
    public void EndAttack()
    {
        isAttacking = false;
        hasAppliedDamage = false;
        attackTimer = 0f;
    }
    // 공격 수행
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

        hasAppliedDamage = false;

        Debug.Log($"{gameObject.name}이(가) 플레이어 {target.name}을(를) 공격했습니다!");

        switch (attackType)
        {
            case AttackType.Melee:
                enemy.ExecuteMeleeAttack();
                break;
            case AttackType.Ranged:
                enemy.ExecuteRangedAttack();
                break;
        }
    }

    public void ApplyMeleeDamage()
    {
        Transform target = detector.Target;

        float distance = GetHorizontalDistance(
            transform.position,
            target.position
        );

        // 타격 순간에 사거리 재확인
        if (distance > attackRange)
            return;

        SkeletalEntity targetEntity = target.GetComponentInParent<SkeletalEntity>();

        targetEntity.TakeDamage(attackDamage);

        hasAppliedDamage = true;

        Debug.Log($"{target.name}에게 {attackDamage}의 피해를 입혔습니다.");

    }

    private float GetHorizontalDistance(Vector3 a, Vector3 b)
    {
        a.y = 0f;
        b.y = 0f;

        return Vector3.Distance(a, b);
    }
}