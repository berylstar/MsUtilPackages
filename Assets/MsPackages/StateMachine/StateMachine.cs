using System.Collections.Generic;

[System.Serializable]
public abstract class StateMachine<TState> where TState : System.Enum
{
    protected readonly Dictionary<TState, IState> _states = new Dictionary<TState, IState>(8, EqualityComparer<TState>.Default);

    public IState CurrentState { get; private set; } = null;

    public TState CurrentStateType { get; private set; }

    /// <summary>
    /// 상태 변환
    /// </summary>
    public void ChangeState(TState stateType)
    {
        if (_states.TryGetValue(stateType, out IState nextState) == false)
            return;

        // 변환할 상태가 현재 상태와 다를 때만 변환
        if (CurrentState == nextState)
            return;

        CurrentState?.Exit();

        CurrentState = nextState;
        CurrentStateType = stateType;

        CurrentState?.Enter();
    }

    /// <summary>
    /// 상태 머신의 Update
    /// </summary>
    public void Update()
    {
        CurrentState?.Update();
    }

    /// <summary>
    /// 상태 머신의 FixedUpdate
    /// </summary>
    public void FixedUpdate()
    {
        CurrentState?.FixedUpdate();
    }
}