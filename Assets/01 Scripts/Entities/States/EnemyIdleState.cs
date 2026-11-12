using Unity.VisualScripting;
using UnityEngine;

public class EnemyIdleState : EnemyState
{
    public EnemyIdleState(EnemyController enemyController, EnemyStateMachine enemyStateMachine)
        : base(enemyController, enemyStateMachine) { }

    public override void Enter()
    {
        EnemyController.Movement.Stop();
        EnemyController.Enemy.Animator.CrossFade(EnemyAnimationsData.Idle, 0.1f,0);
    }

    public override void Update()
    {
        Transform target = EnemyController.Detector.Target;

        if (target == null) return;

        float distance = GetHorizontalDistance(EnemyController.transform.position, target.position);

        if (distance <= EnemyController.Enemy.AttackRange)
        {
            EnemyStateMachine.ChangeState(EnemyController.AttackState);
            return;
        }

        if (EnemyController.Detector.Target != null)
        {
            EnemyStateMachine.ChangeState(EnemyController.ChaseState);
        }
    }
}
