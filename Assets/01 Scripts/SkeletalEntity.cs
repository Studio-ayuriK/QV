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
        if (_animator == null)
            _animator = GetComponentInChildren<Animator>();
    }

    // Common methods
    #region Attack Methods
    protected virtual void MeleeAttack() { }
    protected virtual void RangedAttack() { }
    #endregion

    #region Damage Methods

    // 피해 처리
    public virtual void TakeDamage(int damage) { }

    #endregion
}
