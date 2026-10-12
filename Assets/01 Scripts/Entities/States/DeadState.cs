using UnityEngine;

public class DeadState : EnemyState
{
    public DeadState(
        EnemyController enemyController,
        EnemyStateMachine enemyStateMachine)
        : base(enemyController, enemyStateMachine)
    {
    }

    public override void Enter()
    {
        EnemyController.Movement.Stop();

        // 추후 사망 애니메이션,
        // 충돌 비활성화 등의 처리를 추가.
    }

    public override void Update()
    {
        // 사망 상태에서는 일반 AI 행동을 수행하지 않음.
    }
}