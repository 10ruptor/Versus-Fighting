public abstract class PlayerGroundedState : PlayerState
{
    protected PlayerGroundedState(PlayerStateMachine stateMachine) : base(stateMachine){}

    public override void RegisterTransition()
    {
        AddTransition(() => !playerGameplay.IsGrounded, stateMachine.Landing);
        AddTransition(() => playerGameplay.IsGrounded && playerGameplay.PlayerInputController.JumpBuffered && playerGameplay.JumpController.CanJump, stateMachine.Jumping);
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
