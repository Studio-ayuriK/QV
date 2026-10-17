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
        Transform target = EnemyController.Detector.Target;

        if (target == null)
        {
            EnemyStateMachine.ChangeState(EnemyController.IdleState);
            return;
        }

        //float distance = Vector3.Distance(EnemyController.transform.position, target.position);

        Vector3 enemyPosition = EnemyController.transform.position;
        Vector3 targetPosition = target.position;

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
            EnemyController.Movement.SetDestination(target.position);

            repathTimer = RepathInterval;
        }
    }
}
