using UnityEngine;

[CreateAssetMenu (fileName = "EnemyDataSO", menuName = "SOData/Enemy DataSO")]
public class EnemyDataSO : ScriptableObject
{
    [Header("기본 능력치")]
    [SerializeField, Min(1)] private int maxHealth = 100;
    [SerializeField, Min(0)] private int attackDamage = 10;
    [SerializeField, Min(0)] private float moveSpeed = 3.5f;

    [Header("전투 설정")]
    [SerializeField, Min(0)] private float attackRange = 2f;
    [SerializeField, Min(0)] private float attackCooldown = 1.5f;

    #region 기본 능력치
    public int MaxHealth => maxHealth;
    public int AttackDamage => attackDamage;
    public float MoveSpeed => moveSpeed;
    #endregion

    #region 전투 설정
    public float AttackRange => attackRange;
    public float AttackCooldown => attackCooldown;
    #endregion
}
