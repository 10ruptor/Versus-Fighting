public abstract class PlayerGroundedState : PlayerState
{

    public override void RegisterTransition()
    {
        AddTransition(() => !stateMachine.PlayerGameplay.IsGrounded, stateMachine.stateLibrary[StateType.Landing]);
        AddTransition(() => stateMachine.PlayerGameplay.IsGrounded && stateMachine.PlayerGameplay.PlayerInputController.JumpBuffered && stateMachine.PlayerGameplay.JumpController.CanJump, stateMachine.stateLibrary[StateType.Jumping]);
    }
    
    public override void Update()
    {
        base.Update();
        CheckTransitions();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        CheckTransitions();
    }
}
