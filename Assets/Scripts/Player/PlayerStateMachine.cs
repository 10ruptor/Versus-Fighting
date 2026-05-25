public class PlayerStateMachine
{
    public PlayerState CurrentState { get; private set; }

    readonly Player player;

    public PlayerStateMachine(Player player)
    {
        this.player = player;
    }

    public void Initialize(PlayerState startState)
    {
        ChangeState(startState);
    }

    public void ChangeState(PlayerState newState)
    {
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
        player.SetCurrentStateName(CurrentState.GetType().Name);
    }
}
