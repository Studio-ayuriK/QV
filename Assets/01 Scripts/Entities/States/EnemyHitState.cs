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
        if (!isAnimationFinished) return;

        // 사망 상태 확인
        if (EnemyController.Enemy.IsDead)
        {
            EnemyStateMachine.ChangeState(EnemyController.DeadState);
            return;
        }
        
        // Player를 찾지 못한 경우
        Transform player = EnemyController.Detector.Target;
        if (player == null)
        {
            // 걷기 상태
            EnemyStateMachine.ChangeState(EnemyController.IdleState);
            return;
        }

        // 공격 사거리 안에 있는 겨웅
        float distance = GetHorizontalDistance(EnemyController.transform.position, player.position);
        if (distance <= EnemyController.Enemy.AttackRange)
        {
            EnemyStateMachine.ChangeState(EnemyController.AttackState);
        }
        else // 공격 사거리 밖의 경우
        {
            EnemyStateMachine.ChangeState(EnemyController.ChaseState);
        }
    }

    public void AnimationFinished()
    {
        isAnimationFinished = true;
    }
}
