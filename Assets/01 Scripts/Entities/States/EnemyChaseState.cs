using UnityEngine;

public class EnemyChaseState : EnemyState
{
    private float repathTimer;      // 경로 찾는 시간
    private const float RepathInterval = 0.2f;  //  재경로 탐색 시간
    public EnemyChaseState(EnemyController enemyController, EnemyStateMachine enemyStateMachine) : base(enemyController, enemyStateMachine)
    {
    }

    public override void Enter()
    {
        repathTimer = 0f;
        EnemyController.Movement.Resume();
        EnemyController.Enemy.Animator.CrossFade(EnemyAnimationsData.Run, 0.1f, 0);
    }

    public override void Update()
    {
        Transform target = EnemyController.Detector.Target;

        // 플레이어를 놓친 경우
        if (target == null)
        {
            EnemyStateMachine.ChangeState(
                EnemyController.IdleState
            );

            return;
        }

        float distance = GetHorizontalDistance(
            EnemyController.transform.position,
            target.position
        );

        // 공격 사거리 진입
        if (distance <= EnemyController.Enemy.AttackRange)
        {
            EnemyStateMachine.ChangeState(
                EnemyController.AttackState
            );

            return;
        }

        // 일정 간격으로 목적지 갱신
        repathTimer -= Time.deltaTime;

        if (repathTimer <= 0f)
        {
            EnemyController.Movement.SetDestination(target.position);

            repathTimer = RepathInterval;
        }
    }

    public override void Exit()
    {
        EnemyController.Movement.Stop();
    }
}
