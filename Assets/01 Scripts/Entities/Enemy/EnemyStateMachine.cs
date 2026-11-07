using UnityEngine;

public class EnemyStateMachine
{
    public EnemyState CurrentState { get; private set; }
    public bool IsInitialized { get; private set; }
    public void Initialize(EnemyState startingState)
    {
        // 기존 상태가 존재하면 종료
        if(IsInitialized) CurrentState?.Exit();

        CurrentState = startingState;
        IsInitialized = true;

        CurrentState.Enter();
    }

    // 상태 변경
    public void ChangeState(EnemyState newState)
    {
        if (!IsInitialized || newState == null)
            return;

        if (CurrentState == newState)
            return;

        // 최종 상태에서 다른 상태로 전환 불가
        if(CurrentState.IsTerminal)
            return;

        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    public void Update()
    {
        CurrentState?.Update();
    }
}
