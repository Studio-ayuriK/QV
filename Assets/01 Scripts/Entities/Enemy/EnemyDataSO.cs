using UnityEngine;

[CreateAssetMenu (fileName = "EnemyDataSO", menuName = "SOData/Enemy DataSO")]
public class EnemyDataSO : ScriptableObject
{
    [Header("기본 능력치")]
    [SerializeField, Min(1)] private int maxHealth = 100;
    [SerializeField, Min(0)] private int attackDamage = 10;
    [SerializeField, Min(0)] private int moveSpeed = 3;

    [Header("전투 설정")]
    [SerializeField, Min(0)] private int attackRange = 2;
    [SerializeField, Min(0)] private int attackCooldown = 1;

    #region 기본 능력치
    public int MaxHealth => maxHealth;
    public int AttackDamage => attackDamage;
    public int MoveSpeed => moveSpeed;
    #endregion

    #region 전투 설정
    public int AttackRange => attackRange;
    public int AttackCooldown => attackCooldown;
    #endregion
}
