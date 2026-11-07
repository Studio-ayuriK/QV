using UnityEngine;

public class Enemy : SkeletalEntity
{
    public enum AttackType
    {
        Melee,
        Ranged
    }
    #region Enemy Data

    [Header("Enemy Data")]
    [SerializeField] private EnemyDataSO enemyData;

    private int maxHealth;
    private int attackDamage;

    private float enemyMoveSpeed;
    private float attackRange;
    private float attackCooldown;

    public int MaxHealth => maxHealth;
    public int AttackDamage => attackDamage;
    public float MoveSpeed => enemyMoveSpeed;
    public float AttackRange => attackRange;
    public float AttackCooldown => attackCooldown;

    #endregion

    #region Attack Settings

    [Header("Attack Settings")]
    [SerializeField] private AttackType attackType = AttackType.Melee;

    private const int UpperBodyLayer = 1;

    [Header("Animation Settings")]
    [SerializeField] private float crossFadeDuration = 0.1f;

    [Header("Ranged Attack Settings")]
    [SerializeField] private EnemyProjectile projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private float projectileAimHeight = 0.8f;

    private bool isAttacking;
    private bool hasAppliedDamage;
    private bool hasFiredProjectile;

    private float attackTimer;

    #endregion

    #region Enemy Status

    public int CurrentHealth { get; private set; }
    public bool IsDead { get; private set; }

    #endregion

    private EnemyController enemyController;
    private EnemyDetector detector;

    private bool isInitialized;

    protected override void Awake()
    {
        base.Awake();

        detector = GetComponent<EnemyDetector>();
    }

    // 초기화
    public bool Initialize(EnemyController controller)
    {
        if (controller == null || enemyData == null)
        {
            Debug.LogError(
                $"{name}: EnemyController 또는 EnemyDataSO가 없습니다.",
                this
            );

            return false;
        }

        enemyController = controller;

        // 다른 컴포넌트의 Awake 실행 순서에 대비
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();

            if (_animator == null)
                _animator = GetComponentInChildren<Animator>();
        }

        if (detector == null)
            detector = GetComponent<EnemyDetector>();

        if (_animator == null || detector == null)
        {
            Debug.LogError(
                $"{name}: Animator 또는 EnemyDetector가 없습니다.",
                this
            );

            return false;
        }

        EnemyCopyData();

        CurrentHealth = MaxHealth;
        IsDead = false;

        isInitialized = true;

        return true;
    }

    // SO 데이터 복사
    private void EnemyCopyData()
    {
        maxHealth = enemyData.MaxHealth;
        attackDamage = enemyData.AttackDamage;

        enemyMoveSpeed = enemyData.MoveSpeed;
        attackRange = enemyData.AttackRange;
        attackCooldown = enemyData.AttackCooldown;
    }

    #region Attack Control

    // 공격 시작
    public void BeginAttack()
    {
        if (!isInitialized || isAttacking || IsDead)
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

    // 공격 대기 및 반복 실행
    private void Update()
    {
        if (!isInitialized || !isAttacking)
            return;

        if (IsDead)
        {
            EndAttack();
            return;
        }

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            PerformAttack();

            // 쿨다운이 0일 때 매 프레임 공격하는 것 방지
            attackTimer = Mathf.Max(AttackCooldown, 0.05f);
        }
    }

    #endregion

    #region Attack Execution

    // 공격 방식에 따라 애니메이션 실행
    private void PerformAttack()
    {
        if (detector == null || _animator == null)
            return;

        if (!TryGetTarget(out SkeletalEntity target))
            return;

        hasAppliedDamage = false;
        hasFiredProjectile = false;

        switch (attackType)
        {
            case AttackType.Melee:
                PlayMeleeAttackAnimation();
                break;

            case AttackType.Ranged:
                PlayRangedAttackAnimation();
                break;
        }
    }

    // 근접 공격 애니메이션
    private void PlayMeleeAttackAnimation()
    {
        _animator.CrossFade(
            EnemyAnimationsData.MeleeAttack,
            crossFadeDuration,
            UpperBodyLayer,
            0f
        );
    }

    // 원거리 공격 애니메이션
    private void PlayRangedAttackAnimation()
    {
        _animator.CrossFade(
            EnemyAnimationsData.RangedAttack,
            crossFadeDuration,
            UpperBodyLayer,
            0f
        );
    }

    #endregion

    #region Animation Events

    // 근접 공격 클립의 타격 프레임에 등록
    public void AnimationEvent_MeleeAttack()
    {
        if (!isAttacking || attackType != AttackType.Melee)
            return;

        MeleeAttack();
    }

    // 원거리 공격 클립의 발사 프레임에 등록
    public void AnimationEvent_RangedAttack()
    {
        if (!isAttacking || attackType != AttackType.Ranged)
            return;

        RangedAttack();
    }

    #endregion

    #region Melee Attack

    // 실제 근접 공격 로직
    protected override void MeleeAttack()
    {
        if (IsDead || hasAppliedDamage)
            return;

        if (!TryGetTarget(out SkeletalEntity target))
            return;

        // 중복 피해 방지
        hasAppliedDamage = true;

        target.TakeDamage(AttackDamage);

        Debug.Log(
            $"{target.name}에게 {AttackDamage}의 근접 피해를 입혔습니다."
        );
    }

    #endregion

    #region Ranged Attack

    // 실제 원거리 공격 로직
    protected override void RangedAttack()
    {
        if (IsDead || hasFiredProjectile)
            return;

        if (projectilePrefab == null || projectileSpawnPoint == null)
        {
            Debug.LogError(
                $"{name}: Projectile Prefab 또는 Spawn Point가 설정되지 않았습니다.",
                this
            );

            return;
        }

        if (!TryGetTarget(out SkeletalEntity target))
            return;

        // 플레이어 상체 방향을 조준
        Vector3 targetPosition =
            target.transform.position +
            Vector3.up * projectileAimHeight;

        Vector3 direction =
            targetPosition - projectileSpawnPoint.position;

        if (direction.sqrMagnitude <= 0.001f)
            return;

        Quaternion rotation =
            Quaternion.LookRotation(direction.normalized);

        // 투사체 생성
        EnemyProjectile projectile = Instantiate(
            projectilePrefab,
            projectileSpawnPoint.position,
            rotation
        );

        // 투사체에 공격자, 공격력, 방향 전달
        projectile.Initialize(
            this,
            AttackDamage,
            direction.normalized
        );

        hasFiredProjectile = true;

        Debug.Log(
            $"{target.name}을 향해 원거리 공격을 실행했습니다."
        );
    }

    #endregion

    #region Target & Distance

    // 타깃 확인 및 사거리 검사
    private bool TryGetTarget(out SkeletalEntity targetEntity)
    {
        targetEntity = null;

        if (IsDead || detector == null)
            return false;

        Transform target = detector.Target;

        if (target == null)
            return false;

        targetEntity = target.GetComponentInParent<SkeletalEntity>();

        if (targetEntity == null || targetEntity == this)
            return false;

        float distance = GetHorizontalDistance(
            transform.position,
            targetEntity.transform.position
        );

        return distance <= AttackRange;
    }

    // 수평 거리 계산
    private float GetHorizontalDistance(Vector3 a, Vector3 b)
    {
        a.y = 0f;
        b.y = 0f;

        return Vector3.Distance(a, b);
    }

    #endregion

    #region Damage

    // 피해 처리
    public override void TakeDamage(int damage)
    {
        if (IsDead || damage <= 0)
            return;

        CurrentHealth = Mathf.Max(0, CurrentHealth - damage);

        Debug.Log($"{name} 남은 체력: {CurrentHealth}");

        if (CurrentHealth <= 0)
            Die();
    }

    // 사망 처리
    private void Die()
    {
        if (IsDead)
            return;

        IsDead = true;

        EndAttack();

        if (enemyController != null)
            enemyController.EnterDeadState();
    }
    #endregion
}