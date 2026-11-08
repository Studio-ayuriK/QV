using UnityEngine;

public class EnemyState
{
    protected EnemyController EnemyController;
    protected EnemyStateMachine EnemyStateMachine;

    public virtual bool IsTerminal => false;    // 최종 상태 여부 확인

    protected EnemyState(
        EnemyController enemyController, EnemyStateMachine enemyStateMachine)
    {
        EnemyController = enemyController;
        EnemyStateMachine = enemyStateMachine;
    }
    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }

    // 공통 수평 거리 계산
    protected float GetHorizontalDistance(Vector3 a, Vector3 b)
    {
        a.y = 0f;
        b.y = 0f;

        return Vector3.Distance(a, b);
    }
}
