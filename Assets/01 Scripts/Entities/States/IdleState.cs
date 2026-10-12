using Unity.VisualScripting;
using UnityEngine;

public class IdleState : EnemyState
{
    public IdleState(EnemyController enemyController, EnemyStateMachine enemyStateMachine)
        : base(enemyController, enemyStateMachine) { }

    public override void Enter()
    {
        EnemyController.Movement.Stop();
    }

    public override void Update()
    {
        if (EnemyController.Detector.Target != null)
        {
            EnemyStateMachine.ChangeState(EnemyController.ChaseState);
        }
    }
}
