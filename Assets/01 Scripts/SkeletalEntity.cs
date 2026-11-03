using UnityEngine;

public abstract class SkeletalEntity : MonoBehaviour {
    public float moveSpeed = 6.0f;
    public float rotationSpeed = 20.0f;

    protected Animator _animator;

    protected Vector3 _moveDirVec;
    protected Vector3 _velocity;
    protected Quaternion _targetRotation;

    protected virtual void Awake() {
        _animator = GetComponent<Animator>();
    }

    // Common methods
    #region Attack Methods
    // 외부에서 근접 공격 실행
    public void ExecuteMeleeAttack()
    {
        MeleeAttack();
    }

    // 외부에서 원거리 공격 실행
    public void ExecuteRangedAttack()
    {
        RangedAttack();
    }
    protected virtual void MeleeAttack() { }
    protected virtual void RangedAttack() { }
    #endregion

    #region Damage Methods

    // 피해 처리
    public virtual void TakeDamage(int damage) { }

    #endregion
}
