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
    }

    public override void Update()
    {
        if (EnemyController.Detector.Target == null)
        {
            EnemyStateMachine.ChangeState(EnemyController.IdleState);
            return;
        }

        Vector3 enemyPosition = EnemyController.transform.position;
        Vector3 targetPosition = EnemyController.Detector.Target.position;

        enemyPosition.y = 0f;
        targetPosition.y = 0f;

        float distance = Vector3.Distance(enemyPosition, targetPosition);

        if (distance <= EnemyController.Combat.AttackRange)
        {
            EnemyStateMachine.ChangeState(EnemyController.AttackState);
            return;
        }
        repathTimer -= Time.deltaTime;
        if (repathTimer <= 0f)
        {
            EnemyController.Movement.SetDestination(EnemyController.Detector.Target.position);

            repathTimer = RepathInterval;
        }
    }
}
