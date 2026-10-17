using UnityEngine;

public class PublicDefines {
    //==================== Animation Hash (AH)
    public static readonly int AHIdle = Animator.StringToHash("Idle");
    public static readonly int AHRun = Animator.StringToHash("Running");
    public static readonly int AHWalkBack = Animator.StringToHash("Walking_Backward");
    public static readonly int AHRunRight = Animator.StringToHash("Running_Right");
    public static readonly int AHRunLeft = Animator.StringToHash("Running_Left");
    public static readonly int AH1HAttackSliceDiagonal = Animator.StringToHash("1H_Attack_Slice_Diagonal");
    public static readonly int AH1HAttackSliceHorizontal = Animator.StringToHash("1H_Attack_Slice_Horizontal");


    //==================== Transform Path (TP)
    public static readonly string TPWeaponslotRight = "Rig_Medium/root/hips/spine/chest/upperarm.r/lowerarm.r/wrist.r/hand.r/weaponslot.r";
    public static readonly string TPSpineTransform = "Rig_Medium/root/hips/spine";
}
