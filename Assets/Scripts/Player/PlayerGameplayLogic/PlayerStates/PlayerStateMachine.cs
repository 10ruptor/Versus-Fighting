using UnityEngine;

public class PlayerStateMachine
{
    public PlayerState CurrentState { get; private set; }
    private PlayerGameplay playerGameplay;

    public PlayerStateMachine(PlayerGameplay playerGameplay)
    {
        this.playerGameplay = playerGameplay;
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
        playerGameplay.SetCurrentStateName(CurrentState?.GetType().Name);
    }
}
