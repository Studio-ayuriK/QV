using UnityEngine;

public class EnemyAttackState : EnemyState
{
    public const float AttackExitBuffer = 0.3f;
    public EnemyAttackState(EnemyController enemyController, EnemyStateMachine enemyStateMachine) : base(enemyController, enemyStateMachine) { }

    public override void Enter()
    {
        EnemyController.Movement.Stop();

        // Base Layer의 Hit 애니메이션 자세에서 벗어나기
        Animator animator = EnemyController.Enemy.Animator;

        if (animator != null)
        {
            animator.CrossFade(
                EnemyAnimationsData.Idle,
                0.1f,
                0
            );
        }

        EnemyController.Combat.BeginAttack();
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

        if (EnemyController.Combat.IsAttackAnimationPlaying) return;

        float distance = GetHorizontalDistance(
            EnemyController.transform.position,
            target.position
        );

        float exitRange = EnemyController.Enemy.AttackRange + AttackExitBuffer;
        // 공격 사거리 이탈
        //if (distance > EnemyController.Enemy.AttackRange)
        if (distance > exitRange)
        {
            EnemyStateMachine.ChangeState(EnemyController.ChaseState);
            return;
        }
    }

    public override void Exit()
    {
        EnemyController.Combat.EndAttack();
    }
}
