using UnityEngine;

public class EnemyState
{
    protected EnemyController EnemyController;
    protected EnemyStateMachine EnemyStateMachine;

    protected EnemyState(
        EnemyController enemyController, EnemyStateMachine enemyStateMachine)
    {
        EnemyController = enemyController;
        EnemyStateMachine = enemyStateMachine;
    }
    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}
