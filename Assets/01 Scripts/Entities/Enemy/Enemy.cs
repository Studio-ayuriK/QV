using UnityEngine;

public class Enemy : MonoBehaviour, IEnemyDamageable
{
    public float CurrentHealth { get; private set; }
    public float MaxHealth { get; private set; }
    public bool IsDead { get; private set; }

    private EnemyController enemyController;

    void Awake()
    {
        enemyController = GetComponent<EnemyController>();

        if (enemyController == null || enemyController.Data == null)
        {
            enabled = false;
            return;
        }

        MaxHealth = enemyController.Data.MaxHealth;
        CurrentHealth = MaxHealth;
    }
    // 받는 데미지
    public void TakeDamage(float damage)
    {
        // 이미 죽었거나 잘못된 피해량이면 무시하기
        if (IsDead || damage <= 0f) return;

        // 체력이 음수가 되지 않게 보정
        CurrentHealth = Mathf.Max(0, CurrentHealth - damage);

        Debug.Log(CurrentHealth);

        if (CurrentHealth <= 0f)
            Die();
    }

    private void Die()
    {
        if(IsDead) return;

        IsDead = true;

        enemyController.EnterDeadState();

    }
}
