using Unity.VisualScripting;
using UnityEngine;

public class EnemyIdleState : EnemyState
{
    public EnemyIdleState(EnemyController enemyController, EnemyStateMachine enemyStateMachine)
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
