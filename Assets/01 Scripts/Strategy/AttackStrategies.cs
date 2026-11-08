using UnityEngine;

public static class AttackStrategies
{
    //공격 종류
    public enum AttackType
    {
        Melee,
        Ranged,
        Magic,
    }

    // 공격 방식의 공통 규칙 (전략 패턴)
    public interface IAttackStrategy
    {
        void ExecuteAttack(SkeletalEntity atttacker, SkeletalEntity target, int damage);
    }

    // 근접 공격
    public class MeleeAttackStrategy : IAttackStrategy
    {
        public void ExecuteAttack(SkeletalEntity attacker, SkeletalEntity target, int damage)
        {
            if (attacker == null || target == null) return;

            // 근접 공격 로직
            attacker.DealDamage(target, damage);
            Debug.Log($"{attacker.name}이 {target.name}에게 근접 공격을 수행합니다. 피해량: {damage}");
        }
    }

    // 원거리 공격
    public class RangedAttackStrategy : IAttackStrategy
    {
        private readonly AttackProjectile attackProjectile;
        private readonly Transform spawnPoint;
        private readonly float aimHeight;
        // 생성자
        public RangedAttackStrategy(AttackProjectile attackProjectile, Transform spawnPoint, float aimHeight)
        {
            this.attackProjectile = attackProjectile;
            this.spawnPoint = spawnPoint;
            this.aimHeight = aimHeight;
        }

        public void ExecuteAttack(SkeletalEntity attacker, SkeletalEntity target, int damage)
        {
            if (attacker == null || target == null)
                return;

            if (attackProjectile == null || spawnPoint == null)
                return;

            Vector3 targetPosition = target.transform.position + Vector3.up * aimHeight;

            Vector3 direction =targetPosition - spawnPoint.position;

            if (direction.sqrMagnitude <= 0.001f)
                return;

            direction.Normalize();

            Quaternion rotation =
                Quaternion.LookRotation(direction);

            AttackProjectile projectile = Object.Instantiate(
                attackProjectile, spawnPoint.position,rotation
            );

            projectile.Initialize(
                attacker,
                damage,
                direction
            );
        }
    }

    // 마법 공격
    public class MagicAttackStrategy : IAttackStrategy
    {
        public void ExecuteAttack(SkeletalEntity attacker, SkeletalEntity target, int damage)
        {
            // 마법 공격 로직
            Debug.Log($"{attacker.name}이 {target.name}에게 마법 공격을 수행합니다. 피해량: {damage}");
            attacker.DealDamage(target, damage);
        }
    }
}