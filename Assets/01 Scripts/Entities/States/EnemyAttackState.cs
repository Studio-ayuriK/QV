using UnityEngine;

public class EnemyAttackState : EnemyState
{
    public EnemyAttackState(EnemyController enemyController, EnemyStateMachine enemyStateMachine) : base(enemyController, enemyStateMachine)
    {
    }

    public override void Enter()
    {
        EnemyController.Movement.Stop();
        EnemyController.Combat.BeginAttack();
    }

    public override void Update()
    {
        Transform target = EnemyController.Detector.Target;

        if (target == null)
        {
            EnemyStateMachine.ChangeState(EnemyController.IdleState);
            return;
        }

        float distance = Vector3.Distance(EnemyController.transform.position, target.position);

        if (distance > EnemyController.Combat.AttackRange)
        {
            EnemyStateMachine.ChangeState(EnemyController.ChaseState);
            return;
        }

        //EnemyController.Movement.SetDestination(target.position);
    }

    public override void Exit()
    {
        EnemyController.Combat.EndAttack();
    }
}
