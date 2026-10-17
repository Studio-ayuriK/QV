using UnityEngine;

public class Enemy : MonoBehaviour
{
    public interface IDamageable
    {
        void TakeDamage(float damage);
    }

    private EnemyController enemy;
    private int currentHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemy = GetComponent<EnemyController>();

        if (enemy.Data != null)
            currentHealth = enemy.Data.MaxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth == 0)
        {
            enemy.DeadState.Enter();
        }
    }
}
