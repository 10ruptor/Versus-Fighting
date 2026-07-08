public class PlayerLandingState : PlayerAirState
{
    public PlayerLandingState(PlayerGameplay playerGameplay) : base(playerGameplay){}
    protected override string StateAnimationName => "Landing";
        
    public override void RegisterTransition()
    {
        AddTransition(() => IsLanding && playerGameplay.IsGrounded && playerGameplay.PlayerInputManager.HasWalkInput,playerGameplay.playerMoveState);
        AddTransition(() => IsLanding  && playerGameplay.IsGrounded && !playerGameplay.PlayerInputManager.HasWalkInput,playerGameplay.playerIdleState);
        AddTransition(() => playerGameplay.PlayerInputManager.jump && playerGameplay.JumpController.CanJump, playerGameplay.PlayerJumpingState);
        AddTransition(() => !playerGameplay.IsGrounded && playerGameplay.PlayerInputManager.attack,playerGameplay.playerAirAttackState);
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