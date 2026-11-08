using UnityEngine;
using static AttackStrategies;

public abstract class CombatBase : MonoBehaviour
{
    protected SkeletalEntity owner;

    protected virtual void Awake()
    {
        CacheOwner();
    }

    // 소유 캐릭터 초기화
    protected void CacheOwner()
    {
        if (owner != null)
            return;

        owner = GetComponent<SkeletalEntity>();
    }

    // 공통 피해 적용
    protected void ExecuteAttack(IAttackStrategy strategy,SkeletalEntity target, int damage)
    {
        if (owner == null || owner.IsDead)
            return;

        if (target == null || target == owner)
            return;

        strategy?.ExecuteAttack(owner, target, damage);
    }
}