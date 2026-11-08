using System.Collections.Generic;
using UnityEngine;
using static AttackStrategies;

public class PlayerCombat : CombatBase
{
    [SerializeField]
    private int attackDamage = 10;

    [SerializeField]
    private LayerMask targetLayers;

    private Player player;

    private IAttackStrategy meleeStrategy;

    private readonly HashSet<SkeletalEntity> hitTargets =
        new HashSet<SkeletalEntity>();

    protected override void Awake()
    {
        base.Awake();

        player = owner as Player;

        meleeStrategy = new MeleeAttackStrategy();

        if (targetLayers.value == 0)
            targetLayers = LayerMask.GetMask("Enemies");
    }

    // 기존 Player의 AttackOneHand()에서 옮겨올 공격 판정
    public void AttackOneHand()
    {
        if (player == null || owner.IsDead)
            return;

        Vector3 attackPoint =
            player.transform.position +
            (
                player.LookDirection +
                player.transform.rotation * player.AttackPointOffset
            );

        Collider[] targets = Physics.OverlapBox(
            attackPoint,
            player.AttackRange / 2f,
            player.transform.rotation,
            targetLayers
        );

        hitTargets.Clear();

        foreach (Collider collider in targets)
        {
            SkeletalEntity target =
                collider.GetComponentInParent<SkeletalEntity>();

            if (target == null || target == owner)
                continue;

            if (!hitTargets.Add(target))
                continue;

            ExecuteAttack(
                meleeStrategy,
                target,
                attackDamage
            );
        }
    }
}