using UnityEngine;

public abstract class SkeletalEntity : MonoBehaviour {
    public float moveSpeed = 6.0f;
    public float rotationSpeed = 20.0f;

    protected Animator _animator;

    protected Vector3 _moveDirVec;
    protected Vector3 _velocity;
    protected Quaternion _targetRotation;

    // 외부에서 공격 실행
    public void ExecuteMeleeAttack() => MeleeAttack();
    public void ExecuteRangedAttack() => RangedAttack();
    // Common methods
    #region Attack Methods
    protected virtual void MeleeAttack() { }
    protected virtual void RangedAttack() { }
    public virtual void TakeDamage(int damage) { }
    #endregion
}
