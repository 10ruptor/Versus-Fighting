public abstract class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(PlayerGameplay playerGameplay) : base(playerGameplay){}
    protected override string StateAnimationName => "Grounded";
        
    public override void RegisterTransition()
    {
        AddTransition(() => !playerGameplay.IsGrounded && !playerGameplay.PlayerInputManager.Jump , playerGameplay.playerLandingState);
        AddTransition(() => playerGameplay.PlayerInputManager.Jump && playerGameplay.IsGrounded && playerGameplay.JumpController.CanJump, playerGameplay.PlayerJumpingState);
    }
    
    public override void Update()
    {
        CheckTransitions();
    }
    
    public override void Enter()
    {

    }

    public override void Exit()
    {

    }
}