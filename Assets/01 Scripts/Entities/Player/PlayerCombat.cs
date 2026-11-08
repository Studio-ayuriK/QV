using System.Collections.Generic;
using UnityEngine;
using static AttackStrategies;

public class PlayerCombat : CombatBase
{
    #region Attack Settings

    [Header("Attack Settings")]
    [SerializeField, Min(0)]
    private int attackDamage = 10;

    [SerializeField]
    private LayerMask targetLayers;

    #endregion

    #region Components

    private Player player;

    #endregion

    #region Strategy Pattern

    private IAttackStrategy meleeStrategy;

    #endregion

    protected override void Awake()
    {
        base.Awake();

        player = owner as Player;

        if (player == null)
        {
            Debug.LogError(
                $"{name}: Player 컴포넌트를 찾을 수 없습니다.",
                this
            );

            enabled = false;
            return;
        }

        // 근접 공격 전략 초기화
        meleeStrategy = new MeleeAttackStrategy();

        // 기본 공격 대상 Layer 설정
        if (targetLayers.value == 0)
        {
            targetLayers = LayerMask.GetMask("Enemies");
        }
    }

    #region Animation Events

    // 근접 공격 Animation Event
    public void AnimationEvent_MeleeAttack()
    {
        if (owner == null || owner.IsDead)
            return;

        AttackOneHand();
    }

    #endregion

    #region Melee Attack

    // 기존 Player의 AttackOneHand()에서 이동한 공격 판정
    private void AttackOneHand()
    {
        if (player == null || meleeStrategy == null)
            return;

        //Debug.Log("Player Attack");

        Vector3 attackPoint =
            player.transform.position + 
            (player.LookDirection + player.transform.rotation * player.AttackPointOffset);

        Vector3 halfExtents =
            player.AttackRange / 2f;

        Collider[] enemies = Physics.OverlapBox(
            attackPoint,
            halfExtents,
            player.transform.rotation,
            targetLayers
        );

        // 동일한 적의 여러 Collider에 중복 피해 방지
        HashSet<SkeletalEntity> hitTargets =
            new HashSet<SkeletalEntity>();

        foreach (Collider hit in enemies)
        {
            SkeletalEntity target =
                hit.GetComponentInParent<SkeletalEntity>();

            if (target == null)
                continue;

            if (target == owner || target.IsDead)
                continue;

            if (!hitTargets.Add(target))
                continue;

            // 공통 공격 전략 실행
            ExecuteAttack(
                meleeStrategy,
                target,
                attackDamage
            );

            //Debug.Log($"{target.name}에게 {attackDamage}의 피해를 입혔습니다.");
        }

        // 공격 범위 디버그 표시
        Utils.DebugDrawCube(
            attackPoint,
            player.transform.rotation,
            halfExtents
        );
    }

    #endregion
}