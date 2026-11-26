using UnityEngine;

public class EnemyStateMachine
{
    public EnemyState CurrentState { get; private set; }
    public bool IsInitialized { get; private set; }

    public string CurrentStateName =>
        CurrentState != null
            ? CurrentState.GetType().Name
            : "None";


    public void Initialize(EnemyState startingState)
    {
        if (startingState == null)
        {
            return;
        }

        if (IsInitialized)
        {
            CurrentState?.Exit();
        }

        // 기존 상태가 존재하면 종료
        if (IsInitialized) CurrentState?.Exit();

        CurrentState = startingState;
        IsInitialized = true;

        CurrentState.Enter();
    }

    // 상태 변경
    public void ChangeState(EnemyState newState)
    {
        if (!IsInitialized)
        {
            return;
        }

        if (newState == null)
        {
            return;
        }

        // 동일한 상태로 변경 방지
        if (CurrentState == newState)
        {
            return;
        }

        // 최종 상태에서는 다른 상태로 전환 불가
        if (CurrentState != null && CurrentState.IsTerminal)
        {
            return;
        }

        string previousState = CurrentStateName;
        string nextState = newState.GetType().Name;

        CurrentState?.Exit();

        CurrentState = newState;

        CurrentState.Enter();
    }

    // 현재 상태 업데이트
    public void Update()
    {
        if (!IsInitialized || CurrentState == null)
            return;

        CurrentState.Update();
    }
}
