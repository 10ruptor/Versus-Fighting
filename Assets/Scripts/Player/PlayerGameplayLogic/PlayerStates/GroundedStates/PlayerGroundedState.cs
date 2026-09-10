public abstract class PlayerGroundedState : PlayerState
{
    protected PlayerGroundedState(PlayerGameplay playerGameplay) : base(playerGameplay){}

    public override void RegisterTransition()
    {
        AddTransition(() => !playerGameplay.IsGrounded, playerGameplay.PlayerLandingState);
        AddTransition(() => playerGameplay.IsGrounded && playerGameplay.PlayerInputController.JumpBuffered && playerGameplay.JumpController.CanJump, playerGameplay.PlayerJumpingState);
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
