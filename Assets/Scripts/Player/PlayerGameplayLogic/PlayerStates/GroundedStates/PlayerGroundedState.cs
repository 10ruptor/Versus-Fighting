public abstract class PlayerGroundedState : PlayerState
{
    protected PlayerGroundedState(PlayerStateMachine stateMachine) : base(stateMachine){}
    private bool shield =>  playerGameplay.PlayerInputController.Shield;
    public override void RegisterTransition()
    {
        AddTransition(() => !playerGameplay.IsGrounded, stateMachine.Landing);
        AddTransition(() => playerGameplay.IsGrounded && playerGameplay.PlayerInputController.JumpBuffered && playerGameplay.JumpController.CanJump, stateMachine.Jumping);
        AddTransition( () => shield && playerGameplay.IsGrounded, stateMachine.Shield );
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
