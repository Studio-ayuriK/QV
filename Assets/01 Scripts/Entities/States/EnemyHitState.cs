using UnityEngine;

public class EnemyHitState : EnemyState
{
    private bool isAnimationFinished;
    public EnemyHitState(EnemyController enemyController,
        EnemyStateMachine enemyStateMachine) : base(enemyController, enemyStateMachine) { }

    public override void Enter()
    {
        isAnimationFinished = false;
        EnemyController.Combat.CancelAttackForHit();     // 공격 정지
        EnemyController.Movement.Stop();        // 이동 멈춤
        EnemyController.Enemy.Animator.CrossFade(EnemyAnimationsData.Hit_A, 0.1f, 0);
    }

    public override void Update()
    {
        if (!isAnimationFinished)
            return;

        if (EnemyController.Enemy.IsDead)
        {
            EnemyStateMachine.ChangeState(EnemyController.DeadState);
            return;
        }

        Transform player = EnemyController.Detector.Target;

        if (player == null)
        {
            EnemyStateMachine.ChangeState(EnemyController.IdleState);
            return;
        }

        float distance = GetHorizontalDistance(EnemyController.transform.position, player.position);

        float attackRange = EnemyController.Enemy.AttackRange;

        if (distance <= attackRange)
        {
            EnemyStateMachine.ChangeState(
                EnemyController.AttackState
            );
        }
        else
        {
            EnemyStateMachine.ChangeState(
                EnemyController.ChaseState
            );
        }
    }
    public void RestartHitAnimation()
    {
        if (EnemyController.Enemy.IsDead) return;

        // 이전 애니메이션 종료 상태 초기화
        isAnimationFinished = false;
        Animator animator = EnemyController.Enemy.Animator;

        if(animator != null)
        {
            animator.Play(EnemyAnimationsData.Hit_A, 0, 0f);
        }
        Debug.Log("hit 애니메이션 재 실행");
    }
    public void AnimationFinished()
    {
        isAnimationFinished = true;
    }
}
