using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    public enum AttackType
    {
        Melee,
        Ranged
    }
    [Header("Attack Settings")]
    [SerializeField] private AttackType attackType;

    private const int UpperBodyLayer = 1;

    [Header("Animation Settings")]
    [SerializeField] private float crossFadeDuration = 0.1f;

    [Header("Ranged Attack Settings")]
    [SerializeField] private EnemyProjectile projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private float projectileAimHeight = 0.8f;

    private Enemy enemy;
    private EnemyDetector detector;
    private Animator animator;

    private bool isAttacking;
    private bool hasAppliedDamage;
    private bool hasFiredProjectile;

    private float attackTimer;

    public AttackType CurrentAttackType => attackType;

    public void Initialize(Enemy enemy)
    {
        this.enemy = enemy;
        detector = GetComponent<EnemyDetector>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!isAttacking)
            return;

        if (enemy == null || enemy.IsDead)
        {
            EndAttack();
            return;

        }

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            PerformAttack();
            attackTimer = enemy.AttackCooldown;
        }
    }

    #region Attack Control
    // Inspector에서 설정한 공격 방식으로 시작
    public void BeginAttack()
    {
        if (isAttacking || enemy == null || enemy.IsDead)
            return;


        isAttacking = true;
        attackTimer = 0f;

        hasAppliedDamage = false;
        hasFiredProjectile = false;
    }

    // 공격 종료
    public void EndAttack()
    {
        isAttacking = false;

        hasAppliedDamage = false;
        hasFiredProjectile = false;

        attackTimer = 0f;
    }

    #endregion

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

        // 공격 시작 전 사거리 확인
        if (distance > enemy.AttackRange)
            return;

        hasAppliedDamage = false;
        hasFiredProjectile = false;

        switch (attackType)
        {
            case AttackType.Melee:
                PlayMeleeAttack();
                break;
            case AttackType.Ranged:
                PlayRangedAttack();
                break;
        }
    }

    // 근접 공격 애니메이션
    private void PlayMeleeAttack()
    {
        animator.CrossFade(EnemyAnimationsData.MeleeAttack, crossFadeDuration, UpperBodyLayer,0f);
    }

    // 원거리 공격 애니메이션
    private void PlayRangedAttack()
    {
        animator.CrossFade(EnemyAnimationsData.RangedAttack, crossFadeDuration, UpperBodyLayer,0f);
    }

    #region Melee Attack

    // 근접 공격 Animation Event
    public void AnimationEvent_MeleeHit()
    {
        if (!isAttacking || attackType != AttackType.Melee)
            return;

        if (hasAppliedDamage)
            return;

        if (!TryGetTarget(out SkeletalEntity targetEntity))
            return;

        // 중복 피해 방지
        hasAppliedDamage = true;

        targetEntity.TakeDamage(enemy.AttackDamage);

        Debug.Log(
            $"{targetEntity.name}에게 {enemy.AttackDamage}의 근접 피해"
        );
    }

    #endregion

    #region Ranged Attack

    // 원거리 공격 Animation Event
    public void AnimationEvent_RangedFire()
    {
        if (!isAttacking || attackType != AttackType.Ranged)
            return;

        if (hasFiredProjectile)
            return;

        if (enemy == null || enemy.IsDead || detector == null)
            return;

        if (projectilePrefab == null || projectileSpawnPoint == null)
        {
            Debug.LogError($"{name}: 투사체 또는 발사 위치가 설정되지 않았습니다.", this);
            return;
        }

        Transform target = detector.Target;

        if (target == null)
            return;

        float distance = GetHorizontalDistance(
            transform.position,
            target.position
        );

        if (distance > enemy.AttackRange)
            return;

        // 플레이어의 상체 방향을 목표로 설정
        Vector3 targetPosition =
            target.position + Vector3.up * projectileAimHeight;

        Vector3 direction =
            targetPosition - projectileSpawnPoint.position;

        if (direction.sqrMagnitude <= 0.001f)
            return;

        Quaternion rotation = Quaternion.LookRotation(direction.normalized);

        // 투사체 생성
        EnemyProjectile projectile = Instantiate(
            projectilePrefab,
            projectileSpawnPoint.position,
            rotation
        );

        // 투사체에 공격력과 발사 방향 전달
        projectile.Initialize(
            enemy,
            enemy.AttackDamage,
            direction.normalized
        );

        hasFiredProjectile = true;

        Debug.Log(
            $"{name} 원거리 투사체 발사! 공격력: {enemy.AttackDamage}"
        );
    }

    #endregion
    // 타깃 확인 및 사거리 검사
    private bool TryGetTarget(out SkeletalEntity targetEntity)
    {
        targetEntity = null;

        if (enemy == null || enemy.IsDead || detector == null)
            return false;

        Transform target = detector.Target;

        if (target == null)
            return false;

        float distance = GetHorizontalDistance(
            transform.position,
            target.position
        );

        if (distance > enemy.AttackRange)
            return false;

        targetEntity = target.GetComponentInParent<SkeletalEntity>();

        return targetEntity != null && targetEntity != enemy;
    }

    private float GetHorizontalDistance(Vector3 a, Vector3 b)
    {
        a.y = 0f;
        b.y = 0f;

        return Vector3.Distance(a, b);
    }
}