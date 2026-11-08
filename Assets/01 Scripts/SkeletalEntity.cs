using UnityEngine;

public abstract class SkeletalEntity : MonoBehaviour {
    public float moveSpeed = 6.0f;
    public float rotationSpeed = 20.0f;

    protected Animator _animator;

    protected Vector3 _moveDirVec;
    protected Vector3 _velocity;
    protected Quaternion _targetRotation;

    #region Common Methods
    public Animator Animator => _animator;

    public int MaxHealth { get; protected set; }
    public int CurrentHealth { get; protected set; }

    public bool IsDead { get; protected set; }

    private bool isHealthInitialized;

    #endregion

    protected virtual void Awake() {
        _animator = GetComponent<Animator>();
        if (_animator == null)
            _animator = GetComponentInChildren<Animator>();
    }


    #region Health

    // 체력 초기화
    protected void InitializeHealth(int maxHealth)
    {
        // 체력 초기화 여부 확인
        MaxHealth = Mathf.Max(1, maxHealth);
        CurrentHealth = MaxHealth;

        IsDead = false;
        isHealthInitialized = true;
    }

    // 피해 받기
    public virtual void TakeDamage(int damage)
    {
        if (!isHealthInitialized || IsDead || damage <= 0)
            return;

        CurrentHealth = Mathf.Max(0, CurrentHealth - damage);

        //Debug.Log($"{name}이 {damage}의 피해를 받았습니다. 남은 체력: {CurrentHealth}");

        if (CurrentHealth <= 0)
            Die();
    }

    // 피해 주기
    public virtual void DealDamage(SkeletalEntity target, int damage)
    {
        if (IsDead || target == null || target == this || damage <= 0)
            return;

        target.TakeDamage(damage);
    }

    // 공통 사망 처리
    protected virtual void Die()
    {
        IsDead = true;
    }
    #endregion
}
