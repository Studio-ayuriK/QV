using UnityEngine;

public static class EnemyAnimationsData
{
    // 기본 동작
    public static readonly int Idle = Animator.StringToHash("Base Layer.Idle");
    public static readonly int Walk = Animator.StringToHash("Base Layer.Walking");
    public static readonly int Run = Animator.StringToHash("Base Layer.Running");
    public static readonly int Death = Animator.StringToHash("Base Layer.Death");

    // 전투 동작
    public static readonly int MeleeAttack = Animator.StringToHash("Upper Layer.Melee_1H_Attack_Slice_Horizontal");
    public static readonly int RangedAttack = Animator.StringToHash("Upper Layer.Ranged_1H_Shooting");

}
