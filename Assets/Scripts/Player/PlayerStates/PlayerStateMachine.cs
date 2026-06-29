using UnityEngine;

public class PlayerStateMachine
{
    public PlayerState CurrentState { get; private set; }
    readonly PlayerGameplay _playerGameplay;

    public PlayerStateMachine(PlayerGameplay playerGameplay)
    {
        this._playerGameplay = playerGameplay;
    }

    public void Initialize(PlayerState startState)
    {
        ChangeState(startState);
    }

    public void ChangeState(PlayerState newState)
    {
        CurrentState?.Exit();
        Debug.Log("Changing state : " + newState);
        CurrentState = newState;
        CurrentState?.Enter();
        _playerGameplay.SetCurrentStateName(CurrentState?.GetType().Name);
    }
}
