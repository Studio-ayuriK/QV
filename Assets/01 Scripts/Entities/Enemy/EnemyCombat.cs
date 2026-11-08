using UnityEngine;
using static AttackStrategies;

public class EnemyCombat : CombatBase
{
    #region Attack Settings

    [Header("Attack Settings")]
    [SerializeField]
    private AttackType attackType = AttackType.Melee;

    private const int UpperBodyLayer = 1;

    [Header("Animation Settings")]
    [SerializeField]
    private float crossFadeDuration = 0.1f;

    [Header("Ranged Attack Settings")]
    [SerializeField]
    private AttackProjectile projectilePrefab;

    [SerializeField]
    private Transform projectileSpawnPoint;

    [SerializeField]
    private float projectileAimHeight = 0.8f;

    #endregion

    #region Components

    private Enemy enemy;
    private EnemyDetector detector;
    private Animator animator;

    #endregion

    #region Strategy Pattern
    private IAttackStrategy meleeStrategy;
    private IAttackStrategy rangedStrategy;
    #endregion

    #region Attack Status

    private bool isAttacking;               // 현재 공격 중인지 여부
    private bool isAttackAnimationPlaying;  // 공격 애니메이션이 재생 중인지 여부
    private bool hasExecutedAttack;         // 공격이 실제로 수행되었는지 여부

    private float attackTimer;

    public AttackType CurrentAttackType => attackType;
    public bool IsAttacking => isAttacking;

    private bool isInitialized;

    #endregion

    // 초기화
    public bool Initialize(Enemy enemy)
    {
        if (isInitialized) return true;

        if (enemy == null) return false;
        
        this.enemy = enemy;
        owner = enemy;
        
        detector = enemy.GetComponent<EnemyDetector>();
        animator = enemy.Animator;

        if (detector == null || animator == null) return false;

        // 근접 공격 전략 초기화
        meleeStrategy = new MeleeAttackStrategy();

        // 원거리 공격 전략 초기화
        if (attackType == AttackType.Ranged)
        {
            if (projectilePrefab == null || projectileSpawnPoint == null)
            {
                Debug.LogError($"{name}: 투사체 또는 생성 위치가 설정되지 않았습니다.", this);
                return false;
            }

            rangedStrategy = new RangedAttackStrategy(
                projectilePrefab,
                projectileSpawnPoint,
                projectileAimHeight
            );
        }

        isInitialized = true;

        return true;
    }

    private void Update()
    {
        if (!isInitialized || !isAttacking)
            return;

        if (enemy == null || enemy.IsDead)
        {
            EndAttack();
            return;
        }

        if (attackTimer > 0f)
            attackTimer -= Time.deltaTime;

        if (isAttackAnimationPlaying) return;
        
        if (attackTimer <= 0f)
        {
            if (PerformAttack())
            {
                // 쿨다운이 0일 때 매 프레임 공격하는 것 방지
                attackTimer = Mathf.Max(enemy.AttackCooldown, 0.05f);
            }
            else attackTimer = 0.1f; // 공격 실패 시 짧은 딜레이를 줘서 반복 공격 방지
        }
    }

    #region Attack Control

    // 공격 시작
    public void BeginAttack()
    {
        if (!isInitialized || isAttacking || enemy.IsDead)
            return;

        isAttacking = true;
        
        attackTimer = 0f;

        hasExecutedAttack = false;
        isAttackAnimationPlaying = false;
    }

    // 공격 종료
    public void EndAttack()
    {
        isAttacking = false;

        hasExecutedAttack = false;
        isAttackAnimationPlaying = false;

        attackTimer = 0f;
    }

    #endregion

    #region Attack Execution

    // 공격 수행
    private bool PerformAttack()
    {
        if (detector == null || animator == null)
            return false;

        if (!TryGetTarget(out SkeletalEntity target))
            return false;

        //Debug.Log($"{name}: 공격 시작 - {target.name}");

        hasExecutedAttack = false;
        isAttackAnimationPlaying = true;

        switch (attackType)
        {
            case AttackType.Melee:
                PlayMeleeAttack();
                break;

            case AttackType.Ranged:
                PlayRangedAttack();
                break;

            default:
                isAttackAnimationPlaying = false;
                return false;
        }

        return true;
    }

    // 근접 공격 애니메이션
    private void PlayMeleeAttack()
    {
        animator.CrossFade(
            EnemyAnimationsData.MeleeAttack, crossFadeDuration, GetAttackAnimationLayer(), 0f);
    }

    // 원거리 공격 애니메이션
    private void PlayRangedAttack()
    {
        animator.CrossFade(
            EnemyAnimationsData.RangedAttack, crossFadeDuration, GetAttackAnimationLayer(), 0f);
    }

    // Animator 레이어 확인
    private int GetAttackAnimationLayer()
    {
        return animator.layerCount > UpperBodyLayer ? UpperBodyLayer : 0;
    }
    #endregion

    #region Animation Events

    // 근접 공격 Animation Event
    public void AnimationEvent_MeleeAttack()
    {
        Debug.Log($"{name}: 근접 공격 Animation Event 실행");


        if (!isAttacking || 
            !isAttackAnimationPlaying ||
            attackType != AttackType.Melee ||
            hasExecutedAttack)
            return;

        MeleeAttack();
    }

    // 원거리 공격 Animation Event
    public void AnimationEvent_RangedAttack()
    {
        //Debug.Log($"{name}: 원거리 공격 Animation Event 실행");

        if (!isAttacking || 
            !isAttackAnimationPlaying ||
            attackType != AttackType.Ranged ||
            hasExecutedAttack)
            return;

        RangedAttack();
    }

    // 공격 애니메이션 종료 Animation Event
    public void AnimationEvent_AttackFinished()
    {
        isAttackAnimationPlaying = false;
    }

    #endregion

    #region Melee Attack

    // 실제 근접 공격
    private void MeleeAttack()
    {
        if (enemy.IsDead || hasExecutedAttack) return;

        if (!TryGetTarget(out SkeletalEntity target)) return;

        if (meleeStrategy == null) return;

        hasExecutedAttack = true;

        ExecuteAttack(meleeStrategy, target, enemy.AttackDamage);
    }

    #endregion

    #region Ranged Attack

    // 실제 원거리 공격
    private void RangedAttack()
    {
        if (enemy.IsDead || hasExecutedAttack) return;

        if (!TryGetTarget(out SkeletalEntity target))
            return;
        if (rangedStrategy == null) return;

        hasExecutedAttack = true;

        ExecuteAttack(rangedStrategy, target, enemy.AttackDamage);
    }

    #endregion

    #region Target & Distance

    // 타깃 확인 및 사거리 검사
    private bool TryGetTarget(out SkeletalEntity targetEntity)
    {
        targetEntity = null;

        if (enemy == null || enemy.IsDead || detector == null)
            return false;

        Transform target = detector.Target;

        if (target == null)
            return false;

        SkeletalEntity foundTarget =
            target.GetComponentInParent<SkeletalEntity>();

        if (foundTarget == null || foundTarget == enemy) return false;

        if (foundTarget.IsDead) return false;

        float distance = GetHorizontalDistance(
            transform.position,
            foundTarget.transform.position
        );
        if (distance > enemy.AttackRange) return false;

        targetEntity = foundTarget;

        return true;
    }

    // 수평 거리 계산
    private float GetHorizontalDistance(Vector3 a, Vector3 b)
    {
        a.y = 0f;
        b.y = 0f;

        return Vector3.Distance(a, b);
    }

    #endregion
}