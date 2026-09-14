public class PlayerLandingState : PlayerAirState
{
    protected override string StateAnimationName => "Airborned";

    public override void RegisterTransition()
    {
        AddTransition(() => stateMachine.PlayerGameplay.IsGrounded && stateMachine.PlayerGameplay.PlayerInputController.HasWalkInput,stateMachine.stateLibrary[StateType.Move]);
        AddTransition(() => stateMachine.PlayerGameplay.IsGrounded && !stateMachine.PlayerGameplay.PlayerInputController.HasWalkInput,stateMachine.stateLibrary[StateType.Idle]);
        AddTransition(() => stateMachine.PlayerGameplay.PlayerInputController.JumpBuffered && CanAirJump, stateMachine.stateLibrary[StateType.Jumping]);
        AddTransition(() => !stateMachine.PlayerGameplay.IsGrounded && stateMachine.PlayerGameplay.PlayerInputController.AttackBuffered,stateMachine.stateLibrary[StateType.AirAttack]);
    }
    
    public override void Update()
    {
        base.Update();
        CheckTransitions();
    }
}