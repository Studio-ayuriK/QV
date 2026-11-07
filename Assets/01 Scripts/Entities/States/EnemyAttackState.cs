using UnityEngine;

public class EnemyAttackState : EnemyState
{
    public EnemyAttackState(EnemyController enemyController, EnemyStateMachine enemyStateMachine) : base(enemyController, enemyStateMachine)
    {
    }

    public override void Enter()
    {
        EnemyController.Movement.Stop();
        EnemyController.Enemy.BeginAttack();
    }

    public override void Update()
    {
        Transform target = EnemyController.Detector.Target;

        // 플레이어를 놓친 경우
        if (target == null)
        {
            EnemyStateMachine.ChangeState(EnemyController.IdleState);
            return;
        }

        float distance = GetHorizontalDistance(
            EnemyController.transform.position,
            target.position
        );
        // 공격 사거리 이탈
        if (distance > EnemyController.Enemy.AttackRange)
        {
            EnemyStateMachine.ChangeState(EnemyController.ChaseState);
            return;
        }
    }

    public override void Exit()
    {
        EnemyController.Enemy.EndAttack();
    }
}
