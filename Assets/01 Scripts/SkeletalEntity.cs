using UnityEngine;

public abstract class SkeletalEntity : MonoBehaviour {
    public float moveSpeed = 6.0f;
    public float rotationSpeed = 20.0f;

    protected Animator _animator;

    protected Vector3 _moveDir;
    protected Vector3 _velocity;
    protected Quaternion _targetRotation;
}
