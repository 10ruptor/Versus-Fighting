public class PlayerJumpingState : PlayerAirState
{
    protected override string StateAnimationName => "Jump";

    public override void RegisterTransition()
    {
        AddTransition(() => IsLanding && stateMachine.PlayerGameplay.IsGrounded && stateMachine.PlayerGameplay.PlayerInputController.HasWalkInput,stateMachine.stateLibrary[StateType.Move]);
        AddTransition(() => IsLanding  && stateMachine.PlayerGameplay.IsGrounded && !stateMachine.PlayerGameplay.PlayerInputController.HasWalkInput,stateMachine.stateLibrary[StateType.Idle]);
        AddTransition(() => !stateMachine.PlayerGameplay.IsGrounded && stateMachine.PlayerGameplay.PlayerInputController.AttackBuffered,stateMachine.stateLibrary[StateType.AirAttack]);
        AddTransition(() => IsLanding &&!stateMachine.PlayerGameplay.IsGrounded ,stateMachine.stateLibrary[StateType.Landing]);
    }
    
    public override void Update()
    {
        base.Update();
        CheckTransitions();
    }
    
    public override void OnEnable()
    {
        base.OnEnable();
        stateMachine.PlayerGameplay.PlayerInputController.ConsumeJumpBuffer();
        stateMachine.PlayerGameplay.JumpController.ConsumeJump();
        stateMachine.PlayerGameplay.JumpController.PrepareJump();
    }
}